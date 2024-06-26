﻿// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using GraphQL.DataLoader;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class ItemLoaderFieldResolver<T> : FieldResolver
{
	private readonly RuntimeMethodHandle _MethodHandle;
	private readonly string _Name;

	/// <exception cref="ArgumentException"/>
	public ItemLoaderFieldResolver(MethodInfo methodInfo)
	{
		methodInfo.IsStatic.ThrowIfFalse();
		methodInfo.ReturnType.IsAny<T, Task<T>, ValueTask<T>>().ThrowIfFalse();

		this._MethodHandle = methodInfo.MethodHandle;
		this._Name = methodInfo.GraphQLName();
	}

	/// <exception cref="ArgumentNullException"/>
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.ThrowIfNull();
		context.Source.ThrowIfNull();

		var dataLoaderAccessor = context.RequestServices.GetRequiredService<IDataLoaderContextAccessor>();
		var dataLoaderContext = dataLoaderAccessor.Context;
		dataLoaderContext.ThrowIfNull();

		var loaderKey = Invariant($"{context.Source.GetType().GraphQLName()}.{this._Name}");
		var dataLoader = dataLoaderContext.GetOrAddLoader<T>(loaderKey, () => this.LoadData(context));

		return await dataLoader.LoadAsync().GetResultAsync(context.CancellationToken);
	}

	private Task<T> LoadData(IResolveFieldContext context)
	{
		var methodInfo = (MethodInfo)this._MethodHandle.ToMethodBase();
		var result = methodInfo.InvokeStaticFunc(context.GetArguments(methodInfo).ToArray());
		return result switch
		{
			ValueTask<T> valueTask => valueTask.AsTask(),
			Task<T> task => task,
			T item => Task.FromResult(item),
			_ => Task.FromResult(default(T))!
		};
	}
}
