// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sam987883.Common.Extensions
{
	public static class ValueExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime As(this DateTime @this, DateTimeKind kind) =>
			DateTime.SpecifyKind(@this, kind);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params string[] values) =>
			string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<string> values) =>
			string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params object[] values) =>
			string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<object> values) =>
			string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsReverse(this Range @this) =>
			@this.End.Value > @this.Start.Value;

		public static Range Normalize(this Range @this, int count) =>
			new Range(@this.Start.IsFromEnd ? count - @this.Start.Value : @this.Start.Value, @this.End.IsFromEnd ? count - @this.End.Value : @this.End.Value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> Range(this int @this, int count, int increment = 1) =>
			@this.To(@this + (count - 1) * increment, increment);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>(this ref T @this, ref T value) where T : struct =>
			(@this, value) = (value, @this);

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
	}
}
