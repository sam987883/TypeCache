// Copyright (c) 2021 Samuel Abraham

using System.Numerics;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static ulong Factorial(this int @this)
	{
		(@this < 0).ThrowIfTrue();

		if (@this is 0)
			return 1UL;

		var result = (ulong)@this;
		while (@this > 1)
			result *= (ulong)(--@this);

		return result;
	}

	/// <inheritdoc cref="TimeSpan.FromDays(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromDays(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Days(this int @this)
		=> TimeSpan.FromDays(@this);

	/// <inheritdoc cref="TimeSpan.FromHours(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromHours(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Hours(this int @this)
		=> TimeSpan.FromHours(@this);

	/// <inheritdoc cref="TimeSpan.FromMicroseconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMicroseconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Microseconds(this int @this)
		=> TimeSpan.FromMicroseconds(@this);

	/// <inheritdoc cref="TimeSpan.FromMilliseconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMilliseconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Milliseconds(this int @this)
		=> TimeSpan.FromMilliseconds(@this);

	/// <inheritdoc cref="TimeSpan.FromMinutes(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMinutes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Minutes(this int @this)
		=> TimeSpan.FromMinutes(@this);

	/// <inheritdoc cref="TimeSpan.FromSeconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromSeconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Seconds(this int @this)
		=> TimeSpan.FromSeconds(@this);
}
