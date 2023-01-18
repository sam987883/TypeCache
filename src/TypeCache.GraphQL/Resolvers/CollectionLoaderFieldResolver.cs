// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class CollectionLoaderFieldResolver<PARENT, CHILD, KEY> : FieldResolver
{
	private readonly MethodInfo _MethodInfo;
	private readonly Func<PARENT, KEY> _GetParentKey;
	private readonly Func<CHILD, KEY> _GetChildKey;

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public CollectionLoaderFieldResolver(MethodInfo methodInfo, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
	{
		getParentKey.AssertNotNull();
		getChildKey.AssertNotNull();
		(methodInfo.ReturnType.IsAssignableTo<IEnumerable<CHILD>>()
			|| methodInfo.ReturnType.Is<Task<IEnumerable<CHILD>>>()
			|| methodInfo.ReturnType.Is<ValueTask<IEnumerable<CHILD>>>()).AssertTrue();

		this._MethodInfo = methodInfo;
		this._GetParentKey = getParentKey;
		this._GetChildKey = getChildKey;
	}

	protected override async ValueTask<object?> ResolveAsync(global::GraphQL.IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();
		context.Source.AssertNotNull();

		var dataLoaderAccessor = context.RequestServices.GetRequiredService<IDataLoaderContextAccessor>();
		var dataLoaderContext = dataLoaderAccessor.Context;
		dataLoaderContext.AssertNotNull();

		var loaderKey = Invariant($"{typeof(PARENT).GraphQLName()}.{this._MethodInfo.GraphQLName()}");
		var dataLoader = dataLoaderContext.GetOrAddCollectionBatchLoader<KEY, CHILD>(loaderKey, keys =>
		{
			var arguments = context.GetArguments<PARENT>(this._MethodInfo).ToArray();
			var sourceType = !this._MethodInfo.IsStatic ? this._MethodInfo.DeclaringType : null;
			var controller = sourceType is not null ? context.RequestServices.GetRequiredService(sourceType) : null;
			var result = this._MethodInfo.Invoke(controller, arguments);
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
