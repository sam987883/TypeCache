// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Resolvers
{
	public class ItemLoaderFieldResolver<T> : IFieldResolver<IDataLoaderResult<T>>
	{
		private readonly InstanceMethodMember _Method;
		private readonly object _Handler;
		private readonly IDataLoaderContextAccessor _DataLoader;

		public ItemLoaderFieldResolver(InstanceMethodMember method, object handler, IDataLoaderContextAccessor dataLoader)
		{
			method.AssertNotNull(nameof(method));
			handler.AssertNotNull(nameof(handler));
			dataLoader.AssertNotNull(nameof(dataLoader));

			if (!method.Return.Type.Is<T>())
				throw new ArgumentException($"{nameof(ItemLoaderFieldResolver<T>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<T>.Name}] instead of [{method.Return.Type.Name}].");

			this._Method = method;
			this._Handler = handler;
			this._DataLoader = dataLoader;
		}

		public IDataLoaderResult<T> Resolve(IResolveFieldContext context)
		{
			context.Source.AssertNotNull($"{nameof(context)}.{nameof(context.Source)}");

			var methodGraphAttribute = this._Method.Attributes.First<GraphAttribute>();
			var parentType = context.Source.GetType();
			var parentGraphAttribute = parentType.GetTypeMember().Attributes.First<GraphAttribute>();
			var child = methodGraphAttribute?.Name ?? this._Method.Name.ToEndpointName();
			var parent = parentGraphAttribute?.Name ?? parentType.Name.ToEndpointName();

			var dataLoader = this._DataLoader!.Context.GetOrAddLoader<T>(
				$"{parent}.{child}",
				() =>
				{
					var arguments = this.GetArguments(context).ToArray();
					var result = this._Method.Invoke!(this._Handler, arguments);
					return result switch
					{
						ValueTask<T> valueTask => valueTask.AsTask(),
						Task<T> task => task,
						_ => Task.FromResult((T)result!)!
					};
				});

			return dataLoader.LoadAsync();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		object IFieldResolver.Resolve(IResolveFieldContext context)
			=> this.Resolve(context);

		private IEnumerable<object?> GetArguments(IResolveFieldContext context)
		{
			foreach (var parameter in this._Method.Parameters)
			{
				var graphAttribute = parameter.Attributes.First<GraphAttribute>();
				if (parameter.Attributes.Any<GraphIgnoreAttribute>())
					continue;

				var parameterType = parameter.Type.Handle.ToType();
				if (parameterType.Is<IResolveFieldContext>() || parameterType.Is(typeof(IResolveFieldContext<>)))
					yield return context;
				else if (parameter.Type.SystemType == SystemType.Unknown)
				{
					var argument = context.GetArgument<IDictionary<string, object?>>(parameter.Name);
					var model = parameterType.Create();
					if (argument is not null)
						model.MapProperties(argument);
					yield return model;
				}
				else
					yield return context.GetArgument(parameterType, parameter.Name); // TODO: Support a default value?
			}
		}
	}
}
