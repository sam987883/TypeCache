// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="TimeSpan.FromDays(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromDays(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Days(this double @this)
		=> TimeSpan.FromDays(@this);

	/// <inheritdoc cref="TimeSpan.FromHours(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromHours(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Hours(this double @this)
		=> TimeSpan.FromHours(@this);

	/// <inheritdoc cref="TimeSpan.FromMicroseconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMicroseconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Microseconds(this double @this)
		=> TimeSpan.FromMicroseconds(@this);

	/// <inheritdoc cref="TimeSpan.FromMilliseconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMilliseconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Milliseconds(this double @this)
		=> TimeSpan.FromMilliseconds(@this);

	/// <inheritdoc cref="TimeSpan.FromMinutes(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMinutes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Minutes(this double @this)
		=> TimeSpan.FromMinutes(@this);

	/// <inheritdoc cref="TimeSpan.FromSeconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromSeconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan Seconds(this double @this)
		=> TimeSpan.FromSeconds(@this);

	/// <inheritdoc cref="BitConverter.DoubleToInt64Bits(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="Half"/>, <see cref="long"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long ToInt64(this double @this)
		=> Unsafe.BitCast<double, long>(@this);

	/// <inheritdoc cref="BitConverter.DoubleToUInt64Bits(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="Half"/>, <see cref="ulong"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ulong ToUInt64(this double @this)
		=> Unsafe.BitCast<double, ulong>(@this);
}
