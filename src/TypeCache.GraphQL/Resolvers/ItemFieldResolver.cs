// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class ItemFieldResolver<ITEM> : FieldResolver
	where ITEM : notnull
{
	private readonly object _Lock = new();
	private readonly RuntimeMethodHandle _MethodHandle;
	private bool _HasResult = false;
	private Task<ITEM> _Result = default!;

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public ItemFieldResolver(MethodInfo methodInfo)
	{
		methodInfo.ThrowIfNull();
		methodInfo.IsStatic.ThrowIfFalse();
		methodInfo.ReturnType.IsAny<ITEM, Task<ITEM>, ValueTask<ITEM>>().ThrowIfFalse();

		this._MethodHandle = methodInfo.MethodHandle;
	}

	/// <exception cref="ArgumentNullException"/>
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.Source.ThrowIfNull();

		if (!this._HasResult)
		{
			lock (this._Lock)
			{
				if (!this._HasResult)
				{
					this._Result = this.GetData(context);
					this._HasResult = true;
				}
			}
		}

		return await this._Result;
	}

	private Task<ITEM> GetData(IResolveFieldContext context)
	{
		var methodInfo = (MethodInfo)this._MethodHandle.ToMethodBase();
		var result = methodInfo.InvokeStaticFunc(context.GetArguments(methodInfo).ToArray());
		return result switch
		{
			ITEM item => Task.FromResult(item),
			Task<ITEM> task => task,
			ValueTask<ITEM> valueTask => valueTask.AsTask(),
			_ => Task.FromResult<ITEM>(default(ITEM)!)
		};
	}
}
