// Copyright (c) 2021 Samuel Abraham

using System.Collections.Concurrent;

namespace TypeCache.Extensions;

public static class ParallelExtensions
{
	public static ParallelLoopResult ForEach<T>(this OrderablePartitioner<T> @this, Action<T, ParallelLoopState, long> action, ParallelOptions? options = null)
		=> options is not null
			? Parallel.ForEach(@this, options, action)
			: Parallel.ForEach(@this, action);

	public static ParallelLoopResult ForEach<T, TLocal>(this OrderablePartitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, long, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null
			? Parallel.ForEach(@this, options, localInit, action, localFinally)
			: Parallel.ForEach(@this, localInit, action, localFinally);

	public static ParallelLoopResult ForEach<T>(this Partitioner<T> @this, Action<T, ParallelLoopState> action, ParallelOptions? options = null)
		=> options is not null
			? Parallel.ForEach(@this, options, action)
			: Parallel.ForEach(@this, action);

	public static ParallelLoopResult ForEach<T>(this Partitioner<T> @this, Action<T> action, ParallelOptions? options = null)
		=> options is not null
			? Parallel.ForEach(@this, options, action)
			: Parallel.ForEach(@this, action);

	public static ParallelLoopResult ForEach<T, TLocal>(this Partitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null
			? Parallel.ForEach(@this, options, localInit, action, localFinally)
			: Parallel.ForEach(@this, localInit, action, localFinally);
}
