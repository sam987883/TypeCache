﻿// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using GraphQL;
using GraphQL.DataLoader;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Utilities;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class BatchLoaderFieldResolver<PARENT, CHILD, MATCH> : FieldResolver
	where MATCH : notnull
{
	private readonly string _DataLoaderKey;
	private readonly MethodInfo _MethodInfo;
	private readonly Func<PARENT, MATCH> _GetParentKey;
	private readonly Func<CHILD, MATCH> _GetChildKey;
	private readonly bool _ReturnCollection;

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public BatchLoaderFieldResolver(MethodInfo methodInfo, Func<PARENT, MATCH> getParentKey, Func<CHILD, MATCH> getChildKey, bool returnCollection)
	{
		methodInfo.ThrowIfNull();
		getParentKey.ThrowIfNull();
		getChildKey.ThrowIfNull();
		methodInfo.ReturnType.IsAny<CHILD[], IEnumerable<CHILD>, Task<CHILD[]>, Task<IEnumerable<CHILD>>, ValueTask<CHILD[]>, ValueTask<IEnumerable<CHILD>>>().ThrowIfFalse();

		this._DataLoaderKey = Invariant($"{typeof(PARENT).GraphQLName()}.{methodInfo.GraphQLName()}");
		this._MethodInfo = methodInfo;
		this._GetParentKey = getParentKey;
		this._GetChildKey = getChildKey;
		this._ReturnCollection = returnCollection;
	}

	/// <exception cref="ArgumentNullException"/>
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.ThrowIfNull();
		context.Source.ThrowIfNull();

		var dataLoaderAccessor = context.RequestServices.GetRequiredService<IDataLoaderContextAccessor>();
		dataLoaderAccessor.Context.ThrowIfNull();

		var key = this._GetParentKey((PARENT)context.Source);
		if (this._ReturnCollection)
		{
			var dataLoader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<MATCH, CHILD>(
				this._DataLoaderKey, keys => this.LoadData(context, keys), this._GetChildKey);
			return await dataLoader.LoadAsync(key).GetResultAsync(context.CancellationToken);
		}
		else
		{
			var dataLoader = dataLoaderAccessor.Context.GetOrAddBatchLoader<MATCH, CHILD>(
				this._DataLoaderKey, keys => this.LoadData(context, keys), this._GetChildKey);
			return await dataLoader.LoadAsync(key).GetResultAsync(context.CancellationToken);
		}
	}

	private Task<IEnumerable<CHILD>> LoadData(IResolveFieldContext context, IEnumerable<MATCH> keys)
	{
		var arguments = context.GetArguments<PARENT, MATCH>(this._MethodInfo, keys).ToArray();
		object? result;
		if (!this._MethodInfo.IsStatic)
		{
			var controller = context.RequestServices!.GetRequiredService(this._MethodInfo.DeclaringType!);
			result = this._MethodInfo.InvokeFunc(controller, arguments);
		}
		else
			result = this._MethodInfo.InvokeStaticFunc(arguments);

		return result switch
		{
			ValueTask<IEnumerable<CHILD>> valueTask => valueTask.AsTask(),
			ValueTask<CHILD[]> valueTask => Task.Run<IEnumerable<CHILD>>(async () => (IEnumerable<CHILD>)await valueTask),
			Task<IEnumerable<CHILD>> task => task,
			Task<CHILD[]> task => Task.Run<IEnumerable<CHILD>>(async () => (IEnumerable<CHILD>)await task),
			IAsyncEnumerable<CHILD> items => Task.FromResult(items.ToBlockingEnumerable()),
			IEnumerable<CHILD> items => Task.FromResult(items),
			_ => Task.FromResult<IEnumerable<CHILD>>(Array<CHILD>.Empty)
		};
	}
}
