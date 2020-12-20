// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;

namespace TypeCache.Extensions
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

		public static object? Move(this IEnumerator @this, ref int count)
		{
			while (count > 0 && @this.MoveNext())
				--count;

			return count == 0 ? @this.Current : null;
		}

		public static T? Move<T>(this IEnumerator<T> @this, ref int count)
		{
			while (count > 0 && @this.MoveNext())
				--count;

			return count == 0 ? @this.Current : default;
		}
	}
}
