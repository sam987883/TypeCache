// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;

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

		public static T? First<T>(this IEnumerator<T?> @this)
			where T : class
			=> @this.MoveNext() ? @this.Current : null;

		public static T? FirstValue<T>(this IEnumerator<T> @this)
			where T : struct
			=> @this.MoveNext() ? @this.Current : null;

		public static T? Get<T>(this IEnumerator<T> @this, Index index)
			where T : class
			=> @this.Move(index.Value) ? @this.Current : null;

		public static T? GetValue<T>(this IEnumerator<T> @this, Index index)
			where T : struct
			=> @this.Move(index.Value) ? @this.Current : null;

		public static bool Move(this IEnumerator enumerator, int count)
		{
			while (count > 0 && enumerator.MoveNext())
				--count;
			return count == 0;
		}
	}
}
