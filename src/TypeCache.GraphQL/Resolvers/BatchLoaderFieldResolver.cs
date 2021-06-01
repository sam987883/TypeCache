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
	public class BatchLoaderFieldResolver<PARENT, CHILD, KEY> : IFieldResolver<IDataLoaderResult<CHILD>>
	{
		private readonly InstanceMethodMember _Method;
		private readonly object _Handler;
		private readonly IDataLoaderContextAccessor _DataLoader;
		private readonly Func<PARENT, KEY> _GetParentKey;
		private readonly Func<CHILD, KEY> _GetChildKey;

		public BatchLoaderFieldResolver(InstanceMethodMember method, object handler, IDataLoaderContextAccessor dataLoader, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
		{
			method.AssertNotNull(nameof(method));
			handler.AssertNotNull(nameof(handler));
			getParentKey.AssertNotNull(nameof(getParentKey));
			getChildKey.AssertNotNull(nameof(getChildKey));

			if (!method.Return.Type.Implements<IEnumerable<CHILD>>())
				throw new ArgumentException($"{nameof(BatchLoaderFieldResolver<PARENT, CHILD, KEY>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<IEnumerable<CHILD>>.Name}] instead of [{method.Return.Type.Name}].");

			var graphAttribute = method.Attributes.First<GraphAttribute>();
			var name = graphAttribute?.Name ?? method.Name.ToEndpointName();

			this._Method = method;
			this._Handler = handler;
			this._DataLoader = dataLoader;
			this._GetParentKey = getParentKey;
			this._GetChildKey = getChildKey;
		}

		public IDataLoaderResult<CHILD> Resolve(IResolveFieldContext context)
		{
			context.Source.AssertNotNull($"{nameof(context)}.{nameof(context.Source)}");

			var graphAttribute = this._Method.Attributes.First<GraphAttribute>();
			var name = graphAttribute?.Name ?? this._Method.Name.ToEndpointName();

			var dataLoader = this._DataLoader!.Context.GetOrAddBatchLoader<KEY, CHILD>(
				$"{TypeOf<PARENT>.Name}.{name}",
				keys =>
				{
					var arguments = this.GetArguments(context, keys).ToArray();
					var result = this._Method.Invoke!(this._Handler, arguments);
					return result switch
					{
						ValueTask<IEnumerable<CHILD>> valueTask => valueTask.AsTask(),
						Task<IEnumerable<CHILD>> task => task,
						_ => Task.FromResult((IEnumerable<CHILD>)result!)!
					};
				},
				this._GetChildKey);

			return dataLoader.LoadAsync(this._GetParentKey((PARENT)context.Source));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		object IFieldResolver.Resolve(IResolveFieldContext context)
			=> this.Resolve(context);

		private IEnumerable<object?> GetArguments(IResolveFieldContext context, IEnumerable<KEY> keys)
		{
			foreach (var parameter in this._Method.Parameters)
			{
				var graphAttribute = parameter.Attributes.First<GraphAttribute>();
				if (parameter.Attributes.Any<GraphIgnoreAttribute>())
					continue;

				if (parameter.Type.Is<IResolveFieldContext>() || parameter.Type.Is<IResolveFieldContext<PARENT>>())
					yield return context;
				else if (parameter.Type.Is<IEnumerable<KEY>>())
					yield return keys;
				else if (parameter.Type.Is<PARENT>())
					yield return context.Source;
				else if (parameter.Type.SystemType == SystemType.Unknown)
				{
					var argument = context.GetArgument<IDictionary<string, object?>>(parameter.Name);
					if (argument is not null)
					{
						var model = parameter.Type.Create();
						model.MapProperties(argument);
						yield return model;
					}
					yield return null;
				}
				else
					yield return context.GetArgument(parameter.Type, parameter.Name); // TODO: Support a default value?
			}
		}
	}
}
