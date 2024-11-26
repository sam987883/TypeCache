// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class Sequence
{
	/// <summary>
	/// Creates an infinite sequence of exponentially increasing seconds based on powers of natural log base <c><see cref="Math.E"/></c>.<br/>
	/// For example: (<c>e^1, e^2, e^3, ...</c>)<br/>
	/// Use Linq's Skip method to change the start of the sequence.<br/>
	/// Use Linq's Select method to cap values at a maximum or minimum.
	/// <param name="count">The power to use in generating the sequence. (<c>i^<paramref name="exponent"/></c>).</param>
	/// </summary>
	/// <returns>An <c><see cref="Int32.MaxValue"/></c> sequence of <c><see cref="TimeSpan"/></c> values.</returns>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<TimeSpan> ExponentialSeconds(int count)
		=> Enumerable.Range(1, count).Select(i => Math.Exp(i).ToSeconds());

	/// <summary>
	/// Creates an infinite sequence of exponentially increasing seconds based on powers of <c><paramref name="exponent"/></c>.<br/>
	/// For example when <c><paramref name="exponent"/></c> = 2: (<c>1^2, 2^2, 3^2, ...</c>)<br/>
	/// Use Linq's Skip method to change the start of the sequence.<br/>
	/// Use Linq's Select method to cap values at a maximum or minimum.
	/// </summary>
	/// <param name="exponent">The power to use in generating the sequence. (<c>i^<paramref name="exponent"/></c>).</param>
	/// <returns>An <c><see cref="Int32.MaxValue"/></c> sequence of <c><see cref="TimeSpan"/></c> values.</returns>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<TimeSpan> ExponentialSeconds(uint exponent, int count)
		=> Enumerable.Range(1, count).Select(i => Math.Pow(i, exponent).ToSeconds());

	/// <summary>
	/// Creates an infinite sequence of linear increasing times based on a fixed value of <c><paramref name="increase"/></c>.<br/>
	/// For example when <c><paramref name="increase"/></c> = 6: <c>(6, 12, 18, ...)</c><br/>
	/// Use Linq's Skip method to change the start of the sequence.<br/>
	/// Use Linq's Select method to cap values at a maximum or minimum.
	/// </summary>
	/// <param name="exponent">The power to use in generating the sequence. (<c>i^<paramref name="exponent"/></c>).</param>
	/// <returns>An <c><see cref="Int32.MaxValue"/></c> sequence of <c><see cref="TimeSpan"/></c> values.</returns>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<TimeSpan> LinearTime(TimeSpan increase, int count)
		=> Enumerable.Range(1, count).Select(i => i * increase);
}
