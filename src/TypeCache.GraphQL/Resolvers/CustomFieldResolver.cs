// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class CustomFieldResolver : FieldResolver
{
	private readonly Func<IResolveFieldContext, ValueTask<object?>> _Resolve;

	public CustomFieldResolver(Func<IResolveFieldContext, ValueTask<object?>> resolve)
	{
		resolve.ThrowIfNull();

		this._Resolve = resolve;
	}

	public CustomFieldResolver(Func<IResolveFieldContext, Task<object?>> resolve)
	{
		resolve.ThrowIfNull();

		this._Resolve = context => new ValueTask<object?>(resolve(context));
	}

	public CustomFieldResolver(Func<IResolveFieldContext, object?> resolve)
	{
		resolve.ThrowIfNull();

		this._Resolve = context => ValueTask.FromResult(resolve(context));
	}

	protected override ValueTask<object?> ResolveAsync(IResolveFieldContext context)
		=> this._Resolve(context);
}
