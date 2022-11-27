// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Resolvers;

public sealed class BatchLoaderFieldResolver<PARENT, CHILD, KEY> : IFieldResolver
{
	private readonly MethodMember _Method;
	private readonly object? _Controller;
	private readonly IDataLoaderContextAccessor _DataLoader;
	private readonly Func<PARENT, KEY> _GetParentKey;
	private readonly Func<CHILD, KEY> _GetChildKey;

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public BatchLoaderFieldResolver(
		MethodMember method,
		object? controller,
		IDataLoaderContextAccessor dataLoader,
		Func<PARENT, KEY> getParentKey,
		Func<CHILD, KEY> getChildKey)
	{
		dataLoader.AssertNotNull();

		if (!method.Static)
			controller.AssertNotNull();

		getParentKey.AssertNotNull();
		getChildKey.AssertNotNull();

		if (!method.Return.Type.Implements<IEnumerable<CHILD>>()
			&& ((method.Return.Task || method.Return.ValueTask) && !method.Return.Type.GenericTypes.First().Implements<IEnumerable<CHILD>>()))
			throw new ArgumentException($"{nameof(BatchLoaderFieldResolver<PARENT, CHILD, KEY>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<IEnumerable<CHILD>>.Name}] instead of [{method.Return.Type.Name}].");

		this._Method = method;
		this._Controller = controller;
		this._DataLoader = dataLoader;
		this._GetParentKey = getParentKey;
		this._GetChildKey = getChildKey;
	}

	public async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.Source.AssertNotNull();

		var loaderKey = Invariant($"{TypeOf<PARENT>.Member.GraphQLName()}.{this._Method.GraphQLName()}");
		var dataLoader = this._DataLoader.Context!.GetOrAddBatchLoader<KEY, CHILD>(loaderKey, async keys =>
		{
			var arguments = context.GetArguments<PARENT>(this._Method, keys).ToArray();
			var result = this._Method.Invoke(this._Controller, arguments);
			return result switch
			{
				ValueTask<IEnumerable<CHILD>> valueTask => await valueTask,
				Task<IEnumerable<CHILD>> task => await task,
				IAsyncEnumerable<CHILD> items => items.ToBlockingEnumerable().ToArray(),
				IEnumerable<CHILD> items => items,
				_ => Array<CHILD>.Empty
			};
		}, this._GetChildKey);

		var key = this._GetParentKey((PARENT)context.Source!);
		return await dataLoader.LoadAsync(key).GetResultAsync(context.CancellationToken);
	}
}
