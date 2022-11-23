// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class ParallelExtensions
{
	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);</c>
	/// </summary>
	public static ParallelLoopResult ForEach<T>(this OrderablePartitioner<T> @this, Action<T, ParallelLoopState, long> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>);</c>
	/// </summary>
	public static ParallelLoopResult ForEach<T, TLocal>(this OrderablePartitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, long, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, localInit, action, localFinally) : Parallel.ForEach(@this, localInit, action, localFinally);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);</c>
	/// </summary>
	public static ParallelLoopResult ForEach<T>(this Partitioner<T> @this, Action<T, ParallelLoopState> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);</c>
	/// </summary>
	public static ParallelLoopResult ForEach<T>(this Partitioner<T> @this, Action<T> action, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, action) : Parallel.ForEach(@this, action);

	/// <summary>
	/// <c>=&gt; <paramref name="options"/> <see langword="is not null"/> ? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>) : <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="localInit"/>, <paramref name="action"/>, <paramref name="localFinally"/>);</c>
	/// </summary>
	public static ParallelLoopResult ForEach<T, TLocal>(this Partitioner<T> @this, Func<TLocal> localInit, Func<T, ParallelLoopState, TLocal, TLocal> action, Action<TLocal> localFinally, ParallelOptions? options = null)
		=> options is not null ? Parallel.ForEach(@this, options, localInit, action, localFinally) : Parallel.ForEach(@this, localInit, action, localFinally);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5, @<paramref name="this"/>.Item6);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5, @this.Item6);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5, @<paramref name="this"/>.Item6, @<paramref name="this"/>.Item7);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5, @this.Item6, @this.Item7);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2, @<paramref name="this"/>.Item3, @<paramref name="this"/>.Item4,
	/// @<paramref name="this"/>.Item5, @<paramref name="this"/>.Item6, @<paramref name="this"/>.Item7, @<paramref name="this"/>.Item8);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InvokeInParallel(this (Action, Action, Action, Action, Action, Action, Action, Action) @this)
		=> Parallel.Invoke(@this.Item1, @this.Item2, @this.Item3, @this.Item4, @this.Item5, @this.Item6, @this.Item7, @this.Item8);
}
