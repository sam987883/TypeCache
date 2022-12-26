﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Concurrent;

namespace TypeCache.Extensions;

public static class ParallelExtensions
{
	public static ParallelLoopResult ForEach<T>(this OrderablePartitioner<T> @this, Action<T, ParallelLoopState, long> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	public static ParallelLoopResult ForEach<T, TLocal>(this OrderablePartitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, long, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, localInit, action, localFinally) : Parallel.ForEach(@this, localInit, action, localFinally);

	public static ParallelLoopResult ForEach<T>(this Partitioner<T> @this, Action<T, ParallelLoopState> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	public static ParallelLoopResult ForEach<T>(this Partitioner<T> @this, Action<T> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	public static ParallelLoopResult ForEach<T, TLocal>(this Partitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, localInit, action, localFinally) : Parallel.ForEach(@this, localInit, action, localFinally);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5, @<paramref name="this"/>.Item6);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5, @this.Item6);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5, @<paramref name="this"/>.Item6, @<paramref name="this"/>.Item7);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5, @this.Item6, @this.Item7);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5, @<paramref name="this"/>.Item6, @<paramref name="this"/>.Item7, @<paramref name="this"/>.Item8);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5, @this.Item6, @this.Item7, @this.Item8);
}
