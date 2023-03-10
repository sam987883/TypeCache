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

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public BatchLoaderFieldResolver(MethodInfo methodInfo, Func<PARENT, MATCH> getParentKey, Func<CHILD, MATCH> getChildKey, bool returnCollection)
	{
		methodInfo.AssertNotNull();
		getParentKey.AssertNotNull();
		getChildKey.AssertNotNull();
		methodInfo.ReturnType.IsAny<CHILD[], IEnumerable<CHILD>, Task<CHILD[]>, Task<IEnumerable<CHILD>>, ValueTask<CHILD[]>, ValueTask<IEnumerable<CHILD>>>().AssertTrue();

		this._DataLoaderKey = Invariant($"{typeof(PARENT).GraphQLName()}.{methodInfo.GraphQLName()}");
		this._MethodInfo = methodInfo;
		this._GetParentKey = getParentKey;
		this._GetChildKey = getChildKey;
		this._ReturnCollection = returnCollection;
	}

	protected override ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.RequestServices.AssertNotNull();

		var dataLoaderAccessor = context.RequestServices.GetRequiredService<IDataLoaderContextAccessor>();
		dataLoaderAccessor.Context.AssertNotNull();

		var dataLoaderResult = this._ReturnCollection
			? this.GetCollectionBatchLoaderResult(dataLoaderAccessor.Context, context)
			: this.GetBatchLoaderResult(dataLoaderAccessor.Context, context);
		return new ValueTask<object?>(dataLoaderResult.GetResultAsync(context.CancellationToken));
	}

	private IDataLoaderResult GetBatchLoaderResult(DataLoaderContext dataLoaderContext, IResolveFieldContext resolveFieldContext)
	{
		resolveFieldContext.Source.AssertNotNull();
		var key = this._GetParentKey((PARENT)resolveFieldContext.Source);
		var dataLoader = dataLoaderContext.GetOrAddBatchLoader<MATCH, CHILD>(
			this._DataLoaderKey, keys => this.GetData(resolveFieldContext, keys), this._GetChildKey);
		return (IDataLoaderResult)dataLoader.LoadAsync(key);
	}

	private IDataLoaderResult GetCollectionBatchLoaderResult(DataLoaderContext dataLoaderContext, IResolveFieldContext resolveFieldContext)
	{
		resolveFieldContext.Source.AssertNotNull();
		var key = this._GetParentKey((PARENT)resolveFieldContext.Source);
		var dataLoader = dataLoaderContext.GetOrAddCollectionBatchLoader<MATCH, CHILD>(
			this._DataLoaderKey, keys => this.GetData(resolveFieldContext, keys), this._GetChildKey);
		return (IDataLoaderResult)dataLoader.LoadAsync(key);
	}

	private Task<IEnumerable<CHILD>> GetData(IResolveFieldContext context, IEnumerable<MATCH> keys)
	{
		var arguments = context.GetArguments<PARENT, MATCH>(this._MethodInfo, keys);
		if (!this._MethodInfo.IsStatic)
		{
			var controller = context.RequestServices!.GetRequiredService(this._MethodInfo.DeclaringType!);
			arguments = arguments.Prepend(controller);
		}
		var result = this._MethodInfo.InvokeMethod(arguments.ToArray());
		return result switch
		{
			ValueTask<IEnumerable<CHILD>> valueTask => valueTask.AsTask(),
			ValueTask<CHILD[]> valueTask => Task.Run<IEnumerable<CHILD>>(async () => (IEnumerable<CHILD>)await valueTask),
			Task<IEnumerable<CHILD>> task => task,
			Task<CHILD[]> valueTask => Task.Run<IEnumerable<CHILD>>(async () => (IEnumerable<CHILD>)await valueTask),
			IEnumerable<CHILD> items => Task.FromResult(items),
			_ => Task.FromResult((IEnumerable<CHILD>)Array<CHILD>.Empty)
		};
	}
}
