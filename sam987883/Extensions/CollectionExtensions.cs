// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace sam987883.Extensions
{
	public static class CollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>(this ICollection<T>? @this) =>
			@this?.Count > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>(this IReadOnlyCollection<T>? @this) =>
			@this?.Count > 0;

		public static void Do<T>(this IList<T>? @this, Action<T> action, Action? between = null)
		{
			if (@this?.Count > 0)
			{
				action(@this[0]);
				if (between != null)
				{
					for (var i = 1; i < @this.Count; ++i)
					{
						between();
						action(@this[i]);
					}
				}
				else
				{
					for (var i = 1; i < @this.Count; ++i)
						action(@this[i]);
				}
			}
		}

		public static void Do<T>(this IReadOnlyList<T>? @this, Action<T> action, Action? between = null)
		{
			if (@this?.Count > 0)
			{
				action(@this[0]);
				if (between != null)
				{
					for (var i = 1; i < @this.Count; ++i)
					{
						between();
						action(@this[i]);
					}
				}
				else
				{
					for (var i = 1; i < @this.Count; ++i)
						action(@this[i]);
				}
			}
		}

		public static void Do<T>(this IList<T>? @this, Action<T, int> action, Action? between = null)
		{
			if (@this?.Count > 0)
			{
				action(@this[0], 0);
				if (between != null)
				{
					for (var i = 1; i < @this.Count; ++i)
					{
						between();
						action(@this[i], i);
					}
				}
				else
				{
					for (var i = 1; i < @this.Count; ++i)
						action(@this[i], i);
				}
			}
		}

		public static void Do<T>(this IReadOnlyList<T>? @this, Action<T, int> action, Action? between = null)
		{
			if (@this?.Count > 0)
			{
				action(@this[0], 0);
				if (between != null)
				{
					for (var i = 1; i < @this.Count; ++i)
					{
						between();
						action(@this[i], i);
					}
				}
				else
				{
					for (var i = 1; i < @this.Count; ++i)
						action(@this[i], i);
				}
			}
		}

		public static Span<T> Get<T>(this T[]? @this, Range range)
		{
			if (!@this.Any())
				return Span<T>.Empty;

			range = range.Normalize(@this.Length);
			var span = @this.AsSpan(range);
			if (range.IsReverse())
				span.Reverse();
			return span;
		}
	}
}
