// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class ValueExtensions
	{
		/// <summary>
		/// <c>
		/// <list type="table">
		/// <item>@<paramref name="this"/>.Start.IsFromEnd &amp;&amp; @<paramref name="this"/>.End.IsFromEnd =&gt; @<paramref name="this"/>.Start &lt; @<paramref name="this"/>.End</item>
		/// <item>!@<paramref name="this"/>.Start.IsFromEnd &amp;&amp; !@<paramref name="this"/>.End.IsFromEnd =&gt; @<paramref name="this"/>.Start &gt; @<paramref name="this"/>.End</item>
		/// <item>_ =&gt; null</item>
		/// </list>
		/// </c>
		/// </summary>
		/// <remarks>Reversal can only be determined if both Range Indexes have the same <c>IsFromEnd</c> value.</remarks>
		public static bool? IsReverse(this Range @this)
			=> @this switch
			{
				_ when @this.Start.IsFromEnd && @this.End.IsFromEnd => @this.Start.Value < @this.End.Value,
				_ when !@this.Start.IsFromEnd && !@this.End.IsFromEnd => @this.Start.Value > @this.End.Value,
				_ => null
			};

		/// <summary>
		/// <c>@<paramref name="this"/>.IsFromEnd ? new <see cref="Index"/>(@<paramref name="this"/>.GetOffset(<paramref name="count"/>)) : @<paramref name="this"/></c>
		/// </summary>
		public static Index Normalize(this Index @this, int count)
			=> @this.IsFromEnd ? new Index(@this.GetOffset(count)) : @this;

		/// <summary>
		/// <c>new <see cref="System.Range"/>(@<paramref name="this"/>.Start.Normalize(<paramref name="count"/>), @<paramref name="this"/>.End.Normalize(<paramref name="count"/>))</c>
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public static Range Normalize(this Range @this, int count)
			=> new Range(@this.Start.Normalize(count), @this.End.Normalize(count));

		public static IEnumerable<int> Range(this int @this, int count, int increment = 1)
		{
			while (--count > -1)
			{
				yield return @this;
				@this += increment;
			}
		}

		public static IEnumerable<T> Repeat<T>(this T @this, int count)
			where T : unmanaged
		{
			while (--count > -1)
			{
				yield return @this;
			}
		}

		/// <summary>
		/// <c>(@<paramref name="this"/>, <paramref name="value"/>) = (<paramref name="value"/>, @<paramref name="this"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static void Swap<T>(this ref T @this, ref T value)
			where T : struct
			=> (@this, value) = (value, @this);

		public static IEnumerable<int> To(this int @this, int end, int increment = 0)
		{
			if (@this < end)
			{
				if (increment < 0)
					yield break;

				if (increment is 0)
					increment = 1;

				while (@this <= end)
				{
					yield return @this;
					@this += increment;
				}
			}
			else if (@this > end)
			{
				if (increment > 0)
					yield break;

				if (increment is 0)
					increment = -1;

				while (@this >= end)
				{
					yield return @this;
					@this += increment;
				}
			}
			else
				yield return @this;
		}

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(bool)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this bool @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="decimal"/>.GetBits(@<paramref name="this"/>).ToMany(<see cref="BitConverter"/>.GetBytes).ToArray()</c>
		/// </summary>
		public static byte[] ToBytes(this decimal @this)
			=> decimal.GetBits(@this).ToMany(BitConverter.GetBytes).ToArray();

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(double)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this double @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(float)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this float @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(int)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this int @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(long)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this long @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(short)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this short @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(uint)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this uint @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(ulong)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this ulong @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.GetBytes(ushort)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this ushort @this)
			=> BitConverter.GetBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.Int64BitsToDouble(long)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static double ToDouble(this long @this)
			=> BitConverter.Int64BitsToDouble(@this);

		/// <summary>
		/// <c><see cref="BitConverter.SingleToInt32Bits(float)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static int ToInt32(this float @this)
			=> BitConverter.SingleToInt32Bits(@this);

		/// <summary>
		/// <c><see cref="BitConverter.DoubleToInt64Bits(double)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static long ToInt64(this double @this)
			=> BitConverter.DoubleToInt64Bits(@this);

		/// <summary>
		/// <c><see cref="BitConverter.Int32BitsToSingle(int)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static float ToSingle(this int @this)
			=> BitConverter.Int32BitsToSingle(@this);

		/// <summary>
		/// <c>
		/// <list type="table">
		/// <item>@<paramref name="this"/>.IsReverse() <see langword="is true"/> =&gt; @<paramref name="this"/>.Start.Value.To(@<paramref name="this"/>.End.Value + 1, -1)</item>
		/// <item>@<paramref name="this"/>.IsReverse() <see langword="is false"/> =&gt; @<paramref name="this"/>.Start.Value.To(@<paramref name="this"/>.End.Value - 1, 1)</item>
		/// <item>_ =&gt; Array&lt;<see cref="int"/>&gt;.Empty</item>
		/// </list>
		/// </c>
		/// </summary>
		public static IEnumerable<int> Values(this Range @this)
			=> @this.IsReverse() switch
			{
				true => @this.Start.Value.To(@this.End.Value + 1, -1),
				false => @this.Start.Value.To(@this.End.Value - 1, 1),
				_ => Array<int>.Empty
			};
	}
}
