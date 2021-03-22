// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypeCache.Collections.Extensions
{
	public static class IAsyncEnumeratorExtensions
	{
		public async static ValueTask<int> CountAsync<T>(this IAsyncEnumerator<T> @this)
		{
			var count = 0;
			while (await @this.MoveNextAsync())
				++count;
			return count;
		}

		public static async IAsyncEnumerable<T> RestAsync<T>(this IAsyncEnumerator<T> @this)
		{
			while (await @this.MoveNextAsync())
				yield return @this.Current;
		}

		public async static ValueTask<T> SkipAsync<T>(this IAsyncEnumerator<T> @this, int count)
		{
			while (count > 0 && await @this.MoveNextAsync())
				--count;

			return count == 0
				? @this.Current
				: throw new IndexOutOfRangeException($"{nameof(IAsyncEnumerator<T>)}.{nameof(SkipAsync)}: Remaining {nameof(count)} of {count}.");
		}
	}
}
