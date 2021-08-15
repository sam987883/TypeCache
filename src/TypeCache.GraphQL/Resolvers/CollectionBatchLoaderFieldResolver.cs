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
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Resolvers
{
	public class CollectionBatchLoaderFieldResolver<PARENT, CHILD, KEY> : IFieldResolver<IDataLoaderResult<IEnumerable<CHILD>>>
	{
		private readonly MethodMember _Method;
		private readonly object _Handler;
		private readonly IDataLoaderContextAccessor _DataLoader;
		private readonly Func<PARENT, KEY> _GetParentKey;
		private readonly Func<CHILD, KEY> _GetChildKey;

		public CollectionBatchLoaderFieldResolver(MethodMember method, object handler, IDataLoaderContextAccessor dataLoader, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
		{
			handler.AssertNotNull(nameof(handler));
			getParentKey.AssertNotNull(nameof(getParentKey));
			getChildKey.AssertNotNull(nameof(getChildKey));

			if (!method.Return.Type.Implements<IEnumerable<CHILD>>())
				throw new ArgumentException($"{nameof(CollectionBatchLoaderFieldResolver<PARENT, CHILD, KEY>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<IEnumerable<CHILD>>.Name}] instead of [{method.Return.Type.Name}].");

			this._Method = method;
			this._Handler = handler;
			this._DataLoader = dataLoader;
			this._GetParentKey = getParentKey;
			this._GetChildKey = getChildKey;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		object IFieldResolver.Resolve(IResolveFieldContext context)
			=> this.Resolve(context);

		public IDataLoaderResult<IEnumerable<CHILD>> Resolve(IResolveFieldContext context)
		{
			context.Source.AssertNotNull($"{nameof(context)}.{nameof(context.Source)}");

			var name = this._Method.Attributes.GraphName() ?? this._Method.Name.TrimStart("Get")!.TrimEnd("Async");
			var dataLoader = this._DataLoader!.Context.GetOrAddCollectionBatchLoader<KEY, CHILD>(
				$"{TypeOf<PARENT>.Name}.{name}",
				async keys =>
				{
					var arguments = context.GetArguments<PARENT>(this._Method, keys).ToArray();
					var result = this._Method.Invoke(this._Handler, arguments);
					return result switch
					{
						ValueTask<IEnumerable<CHILD>> valueTask => await valueTask,
						Task<IEnumerable<CHILD>> task => await task,
						_ => await Task.FromResult((IEnumerable<CHILD>)result!)
					};
				},
				this._GetChildKey);

			return dataLoader.LoadAsync(this._GetParentKey((PARENT)context.Source));
		}
	}
}
