// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;

namespace TypeCache.Extensions
{
	public static class ValueExtensions
	{
		public static void AssertNotNull<T>([AllowNull] this T? @this, string name, [CallerMemberName] string caller = null)
			where T : struct
		{
			if (@this is null)
				throw new ArgumentNullException($"{caller} -> {nameof(AssertNotNull)}: [{name}] is null.");
		}

		public static IEnumerable<T> Repeat<T>(this T @this, int count)
			where T : struct
		{
			while (count > 0)
			{
				yield return @this;
				--count;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime As(this DateTime @this, DateTimeKind kind)
			=> DateTime.SpecifyKind(@this, kind);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTime(this DateTime @this, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, sourceTimeZone, targetTimeZone);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTime(this DateTime @this, string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset ConvertTime(this DateTimeOffset @this, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, targetTimeZone);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset ConvertTime(this DateTimeOffset @this, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTimeFromUTC(this DateTime @this, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTimeFromUtc(@this, targetTimeZone);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTimeToUTC(this DateTime @this, TimeZoneInfo sourceTimeZone)
			=> TimeZoneInfo.ConvertTimeToUtc(@this, sourceTimeZone);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params string[] values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<string> values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params object[] values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<object> values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsReverse(this Range @this)
			=> @this.End.Value > @this.Start.Value;

		public static Index Normalize(this Index @this, int count)
			=> @this.IsFromEnd ? new Index(@this.GetOffset(count)) : @this;

		public static Range Normalize(this Range @this, int count)
			=> new Range(@this.Start.Normalize(count), @this.End.Normalize(count));

		public static IEnumerable<int> Range(this int @this, int count, int increment = 0)
			=> count != 0 ? @this.To(@this + (count - 1) * (increment == 0 ? (count > 0 ? 1 : -1) : increment), increment) : CustomEnumerable<int>.Empty;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>(this ref T @this, ref T value) where T : struct
			=> (@this, value) = (value, @this);

		public static IEnumerable<int> To(this int @this, int end, int increment = 0)
		{
			increment = increment switch
			{
				0 when @this < end => 1,
				0 when @this > end => -1,
				_ when increment < 0 && @this < end => 0,
				_ when increment > 0 && @this > end => 0,
				_ => increment
			};

			if (increment > 0)
			{
				while (@this <= end)
				{
					yield return @this;
					@this += increment;
				}
			}
			else if (increment < 0)
			{
				while (@this >= end)
				{
					yield return @this;
					@this += increment;
				}
			}
			else
				yield return @this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this bool @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this char @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this short @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this int @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this long @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this ushort @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this uint @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this ulong @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this float @this)
			=> BitConverter.GetBytes(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this double @this)
			=> BitConverter.GetBytes(@this);

		public static byte[] ToBytes(this decimal @this)
			=> decimal.GetBits(@this).ToMany(BitConverter.GetBytes).ToArray(sizeof(decimal));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(this float @this)
			=> BitConverter.SingleToInt32Bits(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToInt64(this double @this)
			=> BitConverter.DoubleToInt64Bits(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(this int @this)
			=> BitConverter.Int32BitsToSingle(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this long @this)
			=> BitConverter.Int64BitsToDouble(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> Values(this Range @this)
			=> @this.Start.Value.To(@this.End.Value, @this.IsReverse() ? -1 : 1);
	}
}
