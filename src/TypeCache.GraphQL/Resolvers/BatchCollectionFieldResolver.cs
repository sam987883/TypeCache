// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class BatchCollectionFieldResolver<SOURCE, ITEM> : FieldResolver
	where SOURCE : notnull
	where ITEM : notnull
{
	private readonly object _Lock = new object();
	private readonly RuntimeMethodHandle _MethodHandle;
	private readonly Func<SOURCE, ITEM[], ITEM[]> _GetResults;
	private bool _HasResults = false;
	private Task<ITEM[]>? _Results = null;

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public BatchCollectionFieldResolver(MethodInfo methodInfo, Func<SOURCE, ITEM[], ITEM[]> getResults)
	{
		methodInfo.ThrowIfNull();
		getResults.ThrowIfNull();
		methodInfo.IsStatic.ThrowIfFalse();
		methodInfo.ReturnType.IsAny<IEnumerable<ITEM>, Task<IEnumerable<ITEM>>, ValueTask<IEnumerable<ITEM>>, ITEM[], Task<ITEM[]>, ValueTask<ITEM[]>>().ThrowIfFalse();

		this._MethodHandle = methodInfo.MethodHandle;
		this._GetResults = getResults;
	}

	/// <exception cref="ArgumentNullException"/>
	protected override async Task<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.Source.ThrowIfNull();

		if (!this._HasResults)
		{
			lock (this._Lock)
			{
				if (!this._HasResults)
				{
					this._Results = this.GetData(context);
					this._HasResults = true;
				}
			}
		}

		return this._GetResults((SOURCE)context.Source, await this._Results!);
	}

	private Task<ITEM[]> GetData(IResolveFieldContext context)
	{
		var methodInfo = (MethodInfo)this._MethodHandle.ToMethodBase();
		var result = methodInfo.InvokeStaticFunc(context.GetArguments(methodInfo).ToArray());
		return result switch
		{
			ITEM[] batch => Task.FromResult(batch),
			IEnumerable<ITEM> batch => Task.FromResult(batch.AsArray()),
			Task<ITEM[]> task => task,
			Task<IEnumerable<ITEM>> task => Task.FromResult(task.Result.AsArray()),
			ValueTask<ITEM[]> valueTask => valueTask.AsTask(),
			ValueTask<IEnumerable<ITEM>> valueTask => Task.FromResult(valueTask.Result.AsArray()),
			_ => Task.FromResult<ITEM[]>([])
		};
	}
}
