// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class ValueTupleExtensions
{
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

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Item1 &gt; @<paramref name="this"/>.Item2 ? @<paramref name="this"/>.Item1 : @<paramref name="this"/>.Item2;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Maximum(this (int, int) @this)
		=> @this.Item1 > @this.Item2 ? @this.Item1 : @this.Item2;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Item1.Value &gt; @<paramref name="this"/>.Item2.Value ? @<paramref name="this"/>.Item1 : @<paramref name="this"/>.Item2;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Index Maximum(this (Index, Index) @this)
		=> @this.Item1.Value > @this.Item2.Value ? @this.Item1 : @this.Item2;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Item1 &lt; @<paramref name="this"/>.Item2 ? @<paramref name="this"/>.Item1 : @<paramref name="this"/>.Item2;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Minimum(this (int, int) @this)
		=> @this.Item1 < @this.Item2 ? @this.Item1 : @this.Item2;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Item1.Value &lt; @<paramref name="this"/>.Item2.Value ? @<paramref name="this"/>.Item1 : @<paramref name="this"/>.Item2;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Index Minimum(this (Index, Index) @this)
		=> @this.Item1.Value < @this.Item2.Value ? @this.Item1 : @this.Item2;

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>.Item2, @<paramref name="this"/>.Item1);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static (T, T) Swap<T>(this (T, T) @this)
		=> (@this.Item2, @this.Item1);
}
