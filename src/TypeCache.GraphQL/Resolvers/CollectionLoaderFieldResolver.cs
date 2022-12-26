// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class CollectionLoaderFieldResolver<PARENT, CHILD, KEY> : FieldResolver
{
	private readonly MethodMember _Method;
	private readonly Func<PARENT, KEY> _GetParentKey;
	private readonly Func<CHILD, KEY> _GetChildKey;

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public CollectionLoaderFieldResolver(MethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
	{
		getParentKey.AssertNotNull();
		getChildKey.AssertNotNull();

		if (!method.Return.Type.Implements<IEnumerable<CHILD>>()
			&& ((method.Return.Task || method.Return.ValueTask) && !method.Return.Type.GenericTypes.First().Implements<IEnumerable<CHILD>>()))
			throw new ArgumentException($"{nameof(CollectionLoaderFieldResolver<PARENT, CHILD, KEY>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<IEnumerable<CHILD>>.Name}] instead of [{method.Return.Type.Name}].");

		this._Method = method;
		this._GetParentKey = getParentKey;
		this._GetChildKey = getChildKey;
	}

	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();
		context.Source.AssertNotNull();

		var dataLoaderAccessor = context.RequestServices.GetRequiredService<IDataLoaderContextAccessor>();
		var dataLoaderContext = dataLoaderAccessor.Context;
		dataLoaderContext.AssertNotNull();

		var loaderKey = Invariant($"{TypeOf<PARENT>.Member.GraphQLName()}.{this._Method.GraphQLName()}");
		var dataLoader = dataLoaderContext.GetOrAddCollectionBatchLoader<KEY, CHILD>(loaderKey, keys =>
		{
			var arguments = context.GetArguments<PARENT>(this._Method, keys).ToArray();
			var controller = !this._Method.Static ? context.RequestServices.GetRequiredService(this._Method.Type) : null;
			var result = this._Method.Invoke(controller, arguments);
			return result switch
			{
				ValueTask<IEnumerable<CHILD>> valueTask => valueTask.AsTask(),
				Task<IEnumerable<CHILD>> task => task,
				IAsyncEnumerable<CHILD> items => Task.FromResult(items.ToBlockingEnumerable()),
				IEnumerable<CHILD> items => Task.FromResult(items),
				_ => Task.FromResult((IEnumerable<CHILD>)Array<CHILD>.Empty)
			};
		}, this._GetChildKey);

		var key = this._GetParentKey((PARENT)context.Source);
		return await dataLoader.LoadAsync(key).GetResultAsync(context.CancellationToken);
	}
}
