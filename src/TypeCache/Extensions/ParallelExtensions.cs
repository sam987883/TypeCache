// Copyright (c) 2021 Samuel Abraham

using System.Collections.Concurrent;

namespace TypeCache.Extensions;

public static class ParallelExtensions
{
	extension<T>(OrderablePartitioner<T> @this)
	{
		public ParallelLoopResult ForEach(Action<T, ParallelLoopState, long> action, ParallelOptions? options = null)
			=> options is not null
				? Parallel.ForEach(@this, options, action)
				: Parallel.ForEach(@this, action);

		public ParallelLoopResult ForEach<TLocal>(Func<TLocal> localInit, Func<T, ParallelLoopState, long, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
			=> options is not null
				? Parallel.ForEach(@this, options, localInit, action, localFinally)
				: Parallel.ForEach(@this, localInit, action, localFinally);
	}

	extension<T>(Partitioner<T> @this)
	{
		public ParallelLoopResult ForEach(Action<T, ParallelLoopState> action, ParallelOptions? options = null)
			=> options is not null
				? Parallel.ForEach(@this, options, action)
				: Parallel.ForEach(@this, action);

		public ParallelLoopResult ForEach(Action<T> action, ParallelOptions? options = null)
			=> options is not null
				? Parallel.ForEach(@this, options, action)
				: Parallel.ForEach(@this, action);

		public ParallelLoopResult ForEach<TLocal>(Func<TLocal> localInit, Func<T, ParallelLoopState, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
			=> options is not null
				? Parallel.ForEach(@this, options, localInit, action, localFinally)
				: Parallel.ForEach(@this, localInit, action, localFinally);
	}
}
