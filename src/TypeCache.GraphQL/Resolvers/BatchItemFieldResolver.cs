// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class BatchItemFieldResolver<SOURCE, ITEM> : FieldResolver
	where SOURCE : notnull
	where ITEM : notnull
{
	private readonly object _Lock = new object();
	private readonly RuntimeMethodHandle _MethodHandle;
	private readonly Func<SOURCE, ITEM[], ITEM> _GetResult;
	private bool _HasResults = false;
	private Task<ITEM[]>? _Results = null;

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public BatchItemFieldResolver(MethodInfo methodInfo, Func<SOURCE, ITEM[], ITEM> getResult)
	{
		methodInfo.ThrowIfNull();
		getResult.ThrowIfNull();
		methodInfo.IsStatic.ThrowIfFalse();
		methodInfo.ReturnType.IsAny<ITEM[], Task<ITEM[]>, ValueTask<ITEM[]>>().ThrowIfFalse();

		this._MethodHandle = methodInfo.MethodHandle;
		this._GetResult = getResult;
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

		return this._GetResult((SOURCE)context.Source, await this._Results!);
	}

	private Task<ITEM[]> GetData(IResolveFieldContext context)
	{
		var methodInfo = (MethodInfo)this._MethodHandle.ToMethodBase();
		var result = methodInfo.InvokeStaticFunc(context.GetArguments(methodInfo).ToArray());
		return result switch
		{
			ITEM[] batch => Task.FromResult(batch),
			Task<ITEM[]> task => task,
			ValueTask<ITEM[]> valueTask => valueTask.AsTask(),
			_ => Task.FromResult<ITEM[]>([])
		};
	}
}
