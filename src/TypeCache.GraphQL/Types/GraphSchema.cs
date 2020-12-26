// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TypeCache.Common;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types
{
	public class GraphSchema : Schema
	{
		private readonly ITypeCache _TypeCache;

		public GraphSchema(IServiceProvider provider) : base(provider)
		{
			this._TypeCache = provider.GetTypeCache();
			this.Query = new ObjectGraphType();
			this.Mutation = new ObjectGraphType();

			var handlers = provider.GetServices<IGraphHandler>();
			foreach (var handler in handlers)
			{
				var methodCache = this._TypeCache.GetMethodCache(handler.GetType());
				methodCache.Methods.Values.Gather().If(method => method.Attributes.Any<Attribute, GraphQueryAttribute>()).Do(method =>
				{
					if (method.IsVoid)
						throw new NotSupportedException("Queries cannot have a return type that is void, Task or ValueTask.");

					this.Mutation.AddField(this.CreateFieldType(method, handler));
				});

				methodCache.Methods.Values.Gather().If(method => method.Attributes.Any<Attribute, GraphMutationAttribute>()).Do(method =>
				{
					if (method.IsVoid)
						throw new NotSupportedException("Mutations cannot have a return type that is void, Task or ValueTask.");

					this.Query.AddField(this.CreateFieldType(method, handler));
				});
			}
		}

		private FieldType CreateFieldType(IMethodMember method, IGraphHandler handler)
		{
			var graphAttribute = method.Attributes.First<Attribute, GraphAttribute>();
			return new FieldType
			{
				Type = graphAttribute?.Type ?? method.GetGraphType(false),
				Name = graphAttribute?.Name ?? method.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = method.Attributes.First<Attribute, ObsoleteAttribute>()?.Message,
				Resolver = new FieldResolver(context =>
				{
					var arguments = this.GetArguments(method, context).ToArray();
					return method.Invoke(handler, arguments);
				}),
				Arguments = method.ToQueryArguments()
			};
		}

		private IEnumerable<object> GetArguments(IMethodMember method, IResolveFieldContext context)
		{
			foreach (var parameter in method.Parameters)
			{
				var graphAttribute = parameter.Attributes.First<Attribute, GraphAttribute>();
				if (graphAttribute?.Inject == true)
					yield return context;
				else if (parameter.CollectionType == CollectionType.None && parameter.NativeType == NativeType.Object)
				{
					var parameterType = parameter.TypeHandle.ToType();
					var propertyCache = this._TypeCache.GetPropertyCache(parameterType);
					var argument = context.GetArgument<IDictionary<string, object>>(parameter.Name);
					var model = this._TypeCache.Create(parameterType);
					propertyCache.Map(argument, model);
					yield return model;
				}
				else
					yield return context.GetArgument(parameter.TypeHandle.ToType(), parameter.Name); // TODO: Support a default value?
			}
		}
	}
}
