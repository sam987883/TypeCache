﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class ValueExtensions
	{
		public static void AssertNotNull<T>([AllowNull] this T? @this, string name, [CallerMemberName] string caller = null)
			where T : struct
		{
			if (@this == null)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Index Normalize(this Index @this, int count)
			=> @this.IsFromEnd ? new Index(@this.GetOffset(count)) : @this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range Normalize(this Range @this, int count)
			=> new Range(@this.Start.Normalize(count), @this.End.Normalize(count));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> Range(this int @this, int count, int increment = 1)
			=> @this.To(@this + (count - 1) * increment, increment);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>(this ref T @this, ref T value) where T : struct
			=> (@this, value) = (value, @this);

		public static IEnumerable<int> To(this int @this, int end, int increment = 1)
		{
			if (increment <= 0)
				yield break;

			while (@this <= end)
			{
				yield return @this;
				@this += increment;
			}
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this decimal @this)
			=> decimal.GetBits(@this).ToMany(BitConverter.GetBytes).ToArray(sizeof(decimal));
	}
}
