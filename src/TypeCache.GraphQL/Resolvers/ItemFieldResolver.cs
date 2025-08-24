// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class ItemFieldResolver<ITEM>(StaticMethodEntity method) : FieldResolver
	where ITEM : notnull
{
	private readonly object _Lock = new();
	private bool _HasResult = false;
	private ValueTask<ITEM> _Result = default!;

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.Source.ThrowIfNull();
		method.ThrowIfNull();
		method.Return.ParameterType.IsAny<ITEM, Task<ITEM>, ValueTask<ITEM>>().ThrowIfFalse();

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

	private ValueTask<ITEM> GetData(IResolveFieldContext context)
	{
		var result = method.Invoke(context.GetArguments(method.Parameters).ToArray());
		return result switch
		{
			ITEM item => ValueTask.FromResult(item),
			Task<ITEM> task => new ValueTask<ITEM>(task),
			ValueTask<ITEM> valueTask => valueTask,
			_ => ValueTask.FromResult<ITEM>(default(ITEM)!)
		};
	}
}
