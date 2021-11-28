// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions
{
	public static class EnumeratorExtensions
	{
		public static int Count(this IEnumerator @this)
		{
			var count = 0;
			while (@this.MoveNext())
				++count;
			return count;
		}

		public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this.MoveNext() ? @this.Current : null;
			rest = @this.Rest();
		}

		public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this.MoveNext() ? @this.Current : null;
			second = @this.MoveNext() ? @this.Current : null;
			rest = @this.Rest();
		}

		public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
			where T : struct
		{
			first = @this.MoveNext() ? @this.Current : null;
			second = @this.MoveNext() ? @this.Current : null;
			third = @this.MoveNext() ? @this.Current : null;
			rest = @this.Rest();
		}

		public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out IEnumerable<T> rest)
			where T : class
		{
			first = @this.MoveNext() ? @this.Current : null;
			rest = @this.Rest();
		}

		public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
			where T : class
		{
			first = @this.MoveNext() ? @this.Current : null;
			second = @this.MoveNext() ? @this.Current : null;
			rest = @this.Rest();
		}

		public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
			where T : class
		{
			first = @this.MoveNext() ? @this.Current : null;
			second = @this.MoveNext() ? @this.Current : null;
			third = @this.MoveNext() ? @this.Current : null;
			rest = @this.Rest();
		}

		public static T? Get<T>(this IEnumerator<T> @this, int index)
			where T : class
			=> @this.Skip(index + 1) ? @this.Current : null;

		public static T? GetValue<T>(this IEnumerator<T> @this, int index)
			where T : struct
			=> @this.Skip(index + 1) ? @this.Current : null;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T? Next<T>(this IEnumerator<T?> @this)
			where T : class
			=> @this.MoveNext() ? @this.Current : null;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T? NextValue<T>(this IEnumerator<T> @this)
			where T : struct
			=> @this.MoveNext() ? @this.Current : null;

		public static IEnumerable<T> Rest<T>(this IEnumerator<T> @this)
		{
			while (@this.MoveNext())
				yield return @this.Current;
		}

		public static bool Skip(this IEnumerator enumerator, int count)
		{
			while (count > 0 && enumerator.MoveNext())
				--count;
			return count == 0;
		}
	}
}
