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
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Resolvers;

public class BatchLoaderFieldResolver<PARENT, CHILD, KEY> : IFieldResolver<IDataLoaderResult<CHILD>>
{
	private readonly MethodMember _Method;
	private readonly object? _Handler;
	private readonly IDataLoaderContextAccessor _DataLoader;
	private readonly Func<PARENT, KEY> _GetParentKey;
	private readonly Func<CHILD, KEY> _GetChildKey;

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public BatchLoaderFieldResolver(MethodMember method, object? handler, IDataLoaderContextAccessor dataLoader, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
	{
		dataLoader.AssertNotNull();

		if (!method.Static)
			handler.AssertNotNull();

		getParentKey.AssertNotNull();
		getChildKey.AssertNotNull();

		if (!method.Return.Type.Implements<IEnumerable<CHILD>>()
			&& ((method.Return.IsTask || method.Return.IsValueTask) && method.Return.Type.EnclosedType?.Implements<IEnumerable<CHILD>>() is false))
			throw new ArgumentException($"{nameof(BatchLoaderFieldResolver<PARENT, CHILD, KEY>)}: Expected method [{method.Name}] to have a return type of [{TypeOf<IEnumerable<CHILD>>.Name}] instead of [{method.Return.Type.Name}].");

		this._Method = method;
		this._Handler = handler;
		this._DataLoader = dataLoader;
		this._GetParentKey = getParentKey;
		this._GetChildKey = getChildKey;
	}

	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	object IFieldResolver.Resolve(IResolveFieldContext context)
		=> this.Resolve(context);

	/// <exception cref="ArgumentNullException"/>
	public IDataLoaderResult<CHILD> Resolve(IResolveFieldContext context)
	{
		context.Source.AssertNotNull();

		var parentType = context.Source!.GetTypeMember();
		var dataLoader = this._DataLoader!.Context.GetOrAddBatchLoader<KEY, CHILD>(
			$"{parentType.GraphName()}.{this._Method.GraphName()}",
			async keys =>
			{
				var arguments = context.GetArguments<PARENT>(this._Method, keys).ToArray();
				var result = this._Method.Invoke(this._Handler, arguments);
				return result switch
				{
					ValueTask<IEnumerable<CHILD>> valueTask => await valueTask,
					Task<IEnumerable<CHILD>> task => await task,
					_ => (IEnumerable<CHILD>)result!
				};
			},
			this._GetChildKey);

		return dataLoader.LoadAsync(this._GetParentKey((PARENT)context.Source!));
	}
}
