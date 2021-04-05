// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions
{
	public static class ImmutableArrayExtensions
	{

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out ImmutableArray<T> rest)
			where T : struct
		{
			first = @this.Length > 0 ? @this[0] : null;
			rest = @this.Length > 1 ? @this.RemoveAt(0) : ImmutableArray<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out ImmutableArray<T> rest)
			where T : struct
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			rest = @this.Length > 2 ? @this.RemoveRange(0, 2) : ImmutableArray<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out T? third, out ImmutableArray<T> rest)
			where T : struct
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			third = @this.Length > 2 ? @this[2] : null;
			rest = @this.Length > 3 ? @this.RemoveRange(0, 3) : ImmutableArray<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out ImmutableArray<T> rest)
			where T : class
		{
			first = @this.Length > 0 ? @this[0] : null;
			rest = @this.Length > 1 ? @this.RemoveAt(0) : ImmutableArray<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out ImmutableArray<T> rest)
			where T : class
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			rest = @this.Length > 2 ? @this.RemoveRange(0, 2) : ImmutableArray<T>.Empty;
		}

		public static void Deconstruct<T>(this ImmutableArray<T> @this, out T? first, out T? second, out T? third, out ImmutableArray<T> rest)
			where T : class
		{
			first = @this.Length > 0 ? @this[0] : null;
			second = @this.Length > 1 ? @this[1] : null;
			third = @this.Length > 2 ? @this[2] : null;
			rest = @this.Length > 3 ? @this.RemoveRange(0, 3) : ImmutableArray<T>.Empty;
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				action(@this[i]);
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				action(@this[i], i);
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this.Length;
			if (count > 0)
			{
				action(@this[0]);
				for (var i = 1; i < count; ++i)
				{
					between();
					action(@this[i]);
				}
			}
		}

		public static void Do<T>(this ImmutableArray<T> @this, Action<T, int> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this.Length;
			if (count > 0)
			{
				var i = 0;
				action(@this[0], i);
				for (i = 1; i < count; ++i)
				{
					between();
					action(@this[i], i);
				}
			}
		}
	}
}
