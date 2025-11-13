// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class AsyncEnumeratorExtensions
{
	extension<T>(IAsyncEnumerator<T> @this)
	{
		public async ValueTask<int> CountAsync()
		{
			var count = 0;
			while (await @this.MoveNextAsync())
				++count;

			return count;
		}

		/// <summary>
		/// <c>=&gt; <see langword="await"/> @this.MoveAsync(<paramref name="index"/>) ? @this.Current : <see langword="default"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public async ValueTask<T?> GetAsync(int index)
			=> await @this.MoveAsync(index) ? @this.Current : default;

		public async ValueTask<bool> MoveAsync(int count)
		{
			while (count > 0 && await @this.MoveNextAsync())
				--count;

			return count is 0;
		}

		/// <summary>
		/// <c>=&gt; <see langword="await"/> @this.MoveNextAsync() ? @this.Current : <see langword="default"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public async ValueTask<T?> NextAsync()
			=> await @this.MoveNextAsync() ? @this.Current : default;

		/// <param name="count">Read this many items.</param>
		public async IAsyncEnumerator<T> ReadAsync(int count)
		{
			while (--count > -1 && await @this.MoveNextAsync())
				yield return @this.Current;
		}

		public async IAsyncEnumerable<T> RestAsync()
		{
			while (await @this.MoveNextAsync())
				yield return @this.Current;
		}

		/// <exception cref="IndexOutOfRangeException" />
		public async ValueTask<T> SkipAsync(int count)
		{
			while (count > 0 && await @this.MoveNextAsync())
				--count;

			return count is 0
				? @this.Current
				: throw new IndexOutOfRangeException($"{nameof(IAsyncEnumerator<T>)}.{nameof(SkipAsync)}: Remaining {nameof(count)} of {count}.");
		}
	}
}
