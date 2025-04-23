// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="BitConverter.Int64BitsToDouble(long)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="long"/>, <see cref="double"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double ToDouble(this long @this)
		=> Unsafe.BitCast<long, double>(@this);

	/// <inheritdoc cref="BitConverter.UInt64BitsToDouble(ulong)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="ulong"/>, <see cref="double"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double ToDouble(this ulong @this)
		=> Unsafe.BitCast<ulong, double>(@this);

	/// <inheritdoc cref="BitConverter.Int16BitsToHalf(short)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="short"/>, <see cref="Half"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Half ToHalf(this short @this)
		=> Unsafe.BitCast<short, Half>(@this);

	/// <inheritdoc cref="BitConverter.UInt16BitsToHalf(ushort)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="ushort"/>, <see cref="Half"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Half ToHalf(this ushort @this)
		=> Unsafe.BitCast<ushort, Half>(@this);

	/// <inheritdoc cref="BitConverter.HalfToInt16Bits(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="Half"/>, <see cref="short"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static short ToInt16(this Half @this)
		=> Unsafe.BitCast<Half, short>(@this);

	/// <inheritdoc cref="BitConverter.SingleToInt32Bits(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="float"/>, <see cref="int"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int ToInt32(this float @this)
		=> Unsafe.BitCast<float, int>(@this);

	/// <inheritdoc cref="BitConverter.UInt32BitsToSingle(uint)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="uint"/>, <see cref="float"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float ToSingle(this uint @this)
		=> Unsafe.BitCast<uint, float>(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this Guid @this, IFormatProvider? provider = null)
		=> @this.ToString("D", provider ?? InvariantCulture);

	/// <inheritdoc cref="BitConverter.HalfToUInt16Bits(Half)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="Half"/>, <see cref="ushort"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ushort ToUInt16(this Half @this)
		=> Unsafe.BitCast<Half, ushort>(@this);

	/// <inheritdoc cref="BitConverter.SingleToUInt32Bits(float)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="float"/>, <see cref="uint"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static uint ToUInt32(this float @this)
		=> Unsafe.BitCast<float, uint>(@this);
}
