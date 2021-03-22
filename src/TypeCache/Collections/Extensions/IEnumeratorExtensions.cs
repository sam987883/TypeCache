// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections.Extensions
{
	public static class IEnumeratorExtensions
	{
		public static int Count(this IEnumerator @this)
		{
			var count = 0;
			while (@this.MoveNext())
				++count;
			return count;
		}

		public static T? Get<T>(this IEnumerator<T> @this, Index index)
			where T : class
			=> @this.Skip(index.Value) ? @this.Current : null;

		public static T? GetValue<T>(this IEnumerator<T> @this, Index index)
			where T : struct
			=> @this.Skip(index.Value) ? @this.Current : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Next<T>(this IEnumerator<T?> @this)
			where T : class
			=> @this.MoveNext() ? @this.Current : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
