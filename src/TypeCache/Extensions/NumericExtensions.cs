// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	extension(short @this)
	{
		/// <inheritdoc cref="BitConverter.Int16BitsToHalf(short)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="short"/>, <see cref="Half"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Half ToHalf()
			=> Unsafe.BitCast<short, Half>(@this);
	}

	extension(ushort @this)
	{
		/// <inheritdoc cref="BitConverter.UInt16BitsToHalf(ushort)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="ushort"/>, <see cref="Half"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Half ToHalf()
			=> Unsafe.BitCast<ushort, Half>(@this);
	}

	extension(uint @this)
	{
		/// <inheritdoc cref="BitConverter.UInt32BitsToSingle(uint)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="uint"/>, <see cref="float"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public float ToSingle()
			=> Unsafe.BitCast<uint, float>(@this);
	}

	extension(long @this)
	{
		/// <inheritdoc cref="BitConverter.Int64BitsToDouble(long)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="long"/>, <see cref="double"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public double ToDouble()
			=> Unsafe.BitCast<long, double>(@this);
	}

	extension(ulong @this)
	{
		/// <inheritdoc cref="BitConverter.UInt64BitsToDouble(ulong)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="ulong"/>, <see cref="double"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public double ToDouble()
			=> Unsafe.BitCast<ulong, double>(@this);
	}

	extension(Half @this)
	{
		/// <inheritdoc cref="BitConverter.HalfToInt16Bits(Half)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="Half"/>, <see cref="short"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public short ToInt16()
			=> Unsafe.BitCast<Half, short>(@this);

		/// <inheritdoc cref="BitConverter.HalfToUInt16Bits(Half)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="Half"/>, <see cref="ushort"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ushort ToUInt16()
			=> Unsafe.BitCast<Half, ushort>(@this);
	}

	extension(float @this)
	{
		/// <inheritdoc cref="BitConverter.SingleToInt32Bits(float)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="float"/>, <see cref="int"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public int ToInt32()
			=> Unsafe.BitCast<float, int>(@this);

		/// <inheritdoc cref="BitConverter.SingleToUInt32Bits(float)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="float"/>, <see cref="uint"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public uint ToUInt32()
			=> Unsafe.BitCast<float, uint>(@this);
	}

	extension(Guid @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.ToString("D", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText(IFormatProvider? provider = null)
			=> @this.ToString("D", provider ?? InvariantCulture);
	}
}
