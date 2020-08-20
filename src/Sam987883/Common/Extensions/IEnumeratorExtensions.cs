// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sam987883.Common.Extensions
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

	    public static int Count<T>(this IEnumerator<T> @this)
	    {
		    var count = 0;
		    while (@this.MoveNext())
			    ++count;
		    return count;
	    }

		public static (object? Value, bool Exists) Move(this IEnumerator @this, int count)
		{
			while (count > 0 && @this.MoveNext())
				--count;

			return count > 0 ? (@this.Current, true) : (null, false);
		}

		public static (T Value, bool Exists) Move<T>(this IEnumerator<T> @this, int count)
		{
			while (count > 0 && @this.MoveNext())
				--count;

			return count > 0 ? (@this.Current, true) : (default, false);
		}

		public static (V Value, bool Exists) MoveUntil<V>(this IEnumerator @this)
		{
			while (@this.MoveNext())
				if (@this.Current is V current)
					return (current, true);

			return (default, false);
		}

		public static (V Value, bool Exists) MoveUntil<T, V>(this IEnumerator<T> @this)
		{
			while (@this.MoveNext())
				if (@this.Current is V current)
					return (current, true);

			return (default, false);
		}

		public static (object? Value, bool Exists) MoveUntil(this IEnumerator @this, Func<object?, bool> condition)
		{
			while (@this.MoveNext())
				if (condition(@this.Current))
					return (@this.Current, true);

			return (default, false);
		}

		public static (T Value, bool Exists) MoveUntil<T>(this IEnumerator<T> @this, Func<T, bool> condition)
		{
			while (@this.MoveNext())
				if (condition(@this.Current))
					return (@this.Current, true);

			return (default, false);
		}
	}
}
