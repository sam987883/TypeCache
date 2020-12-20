﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypeCache.Extensions
{
	public static class IAsyncEnumeratorExtensions
	{
	    public async static Task<int> CountAsync<T>(this IAsyncEnumerator<T> @this)
	    {
		    var count = 0;
		    while (await @this.MoveNextAsync())
			    ++count;
		    return count;
	    }

		public async static Task<T> MoveAsync<T>(this IAsyncEnumerator<T> @this, int count)
		{
			while (count > 0 && await @this.MoveNextAsync())
				--count;

			if (count > 0)
				new IndexOutOfRangeException($"{nameof(IAsyncEnumerator<T>)}.{nameof(MoveAsync)}: Remaining {nameof(count)} of {count}.");

			return @this.Current;
		}

		public async static Task<V> MoveUntilAsync<T, V>(this IAsyncEnumerator<T> @this)
		{
			while (await @this.MoveNextAsync())
				if (@this.Current is V current)
					return current;

			throw new ArgumentException("Value of this type was not found.", $"typeof({typeof(V).Name})");
		}

		public async static Task<T> MoveUntilAsync<T>(this IAsyncEnumerator<T> @this, Func<T, bool> condition)
		{
			while (await @this.MoveNextAsync())
				if (condition(@this.Current))
					return @this.Current;

			throw new ArgumentException("Value that meets condition was not found.", nameof(condition));
		}
	}
}