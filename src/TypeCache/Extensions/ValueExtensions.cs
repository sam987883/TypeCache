// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Collections;

namespace TypeCache.Extensions
{
	public static class ValueExtensions
	{
		public static void AssertNotNull<T>([AllowNull] this T? @this, string name, [CallerMemberName] string? caller = null)
			where T : struct
		{
			if (!@this.HasValue)
				throw new ArgumentNullException($"{caller} -> {nameof(AssertNotNull)}: [{name}] is null.");
		}

		public static IEnumerable<T> Repeat<T>(this T @this, int count)
			where T : unmanaged
		{
			while (count > 0)
			{
				yield return @this;
				--count;
			}
		}

		/// <summary>
		/// <c><see cref="Range.End"/> &lt; <see cref="Range.Start"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsReverse(this Range @this)
			=> @this.End.Value < @this.Start.Value;

		public static Index Normalize(this Index @this, int count)
			=> @this.IsFromEnd ? new Index(@this.GetOffset(count)) : @this;

		public static Range Normalize(this Range @this, int count)
			=> new Range(@this.Start.Normalize(count), @this.End.Normalize(count));

		public static IEnumerable<int> Range(this int @this, int count, int increment = 0)
			=> count != 0 ? @this.To(@this + (count - 1) * (increment == 0 ? (count > 0 ? 1 : -1) : increment), increment) : CustomEnumerable<int>.Empty;

		/// <summary>
		/// <c>(@this, value) = (value, @this)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>(this ref T @this, ref T value) where T : struct
			=> (@this, value) = (value, @this);

		public static IEnumerable<int> To(this int @this, int end, int increment = 0)
		{
			increment = increment switch
			{
				0 when @this < end => 1,
				0 when @this > end => -1,
				< 0 when @this < end => 0,
				> 0 when @this > end => 0,
				_ => increment
			};

			if (increment == 0)
				yield break;

			while (@this != end)
			{
				yield return @this;
				@this += increment;
			}
		}

		public static IEnumerable<int> Values(this Range @this)
			=> @this.Start.Value.To(@this.End.Value, @this.IsReverse() ? -1 : 1);
	}
}
