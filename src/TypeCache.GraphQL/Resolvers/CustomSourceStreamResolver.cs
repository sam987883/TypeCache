// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Utilities;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;
using ISourceStreamResolver = global::GraphQL.Resolvers.ISourceStreamResolver;
using ResolveFieldContext = global::GraphQL.ResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class CustomSourceStreamResolver(Func<IResolveFieldContext, ValueTask<IObservable<object?>>> resolve) : SourceStreamResolver
{
	public static ISourceStreamResolver Create(Func<IResolveFieldContext, ValueTask<IObservable<object?>>> resolve)
	{
		resolve.ThrowIfNull();

		return new CustomSourceStreamResolver(resolve);
	}

	public static ISourceStreamResolver Create<T>(Func<IResolveFieldContext, ValueTask<IObservable<T?>>> resolve)
		=> typeof(T).IsValueType
			? Create(async context => new CustomObservable<T?>(await resolve(context)))
			: Create(async context => (IObservable<object?>)await resolve(context));

	public static ISourceStreamResolver Create<T>(Func<IResolveFieldContext, IObservable<T?>> resolve)
		=> typeof(T).IsValueType
			? Create(context => ValueTask.FromResult<IObservable<object?>>(new CustomObservable<T?>(resolve(context))))
			: Create(context => ValueTask.FromResult<IObservable<object?>>((IObservable<object?>)resolve(context)));

	public static ISourceStreamResolver Create<T>(Func<IResolveFieldContext, IAsyncEnumerable<T?>> resolve)
		=> Create(context =>
		{
			var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
			((ResolveFieldContext)context).CancellationToken = tokenSource.Token;
			var enumerable = resolve(context);
			return new ObservableAdapter<T>(enumerable!, tokenSource);
		});

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	protected override ValueTask<IObservable<object?>> ResolveAsync(IResolveFieldContext context)
		=> resolve.Invoke(context);
}
