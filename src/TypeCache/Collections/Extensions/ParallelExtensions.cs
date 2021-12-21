// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TypeCache.Collections.Extensions;

public static class ParallelExtensions
{
	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);</c>
	/// </summary>
	public static ParallelLoopResult Do<T>(this OrderablePartitioner<T> @this, Action<T, ParallelLoopState, long> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>);</c>
	/// </summary>
	public static ParallelLoopResult Do<T, TLocal>(this OrderablePartitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, long, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, localInit, action, localFinally) : Parallel.ForEach(@this, localInit, action, localFinally);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);</c>
	/// </summary>
	public static ParallelLoopResult Do<T>(this Partitioner<T> @this, Action<T, ParallelLoopState> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);</c>
	/// </summary>
	public static ParallelLoopResult Do<T>(this Partitioner<T> @this, Action<T> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>);</c>
	/// </summary>
	public static ParallelLoopResult Do<T, TLocal>(this Partitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, localInit, action, localFinally) : Parallel.ForEach(@this, localInit, action, localFinally);
}
