// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class AsyncEnumeratorExtensions
{
	public async static ValueTask<int> CountAsync<T>(this IAsyncEnumerator<T> @this)
	{
		var count = 0;
		while (await @this.MoveNextAsync())
			++count;
		return count;
	}

	/// <summary>
	/// <c>=&gt; <see langword="await"/> @<paramref name="this"/>.MoveAsync(<paramref name="index"/>) ? @<paramref name="this"/>.Current : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static async ValueTask<T?> GetAsync<T>(this IAsyncEnumerator<T> @this, int index)
		=> await @this.MoveAsync(index) ? @this.Current : default;

	public static async ValueTask<bool> MoveAsync<T>(this IAsyncEnumerator<T> @this, int count)
	{
		while (count > 0 && await @this.MoveNextAsync())
			--count;
		return count == 0;
	}

	/// <summary>
	/// <c>=&gt; <see langword="await"/> @<paramref name="this"/>.MoveNextAsync() ? @<paramref name="this"/>.Current : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static async ValueTask<T?> NextAsync<T>(this IAsyncEnumerator<T> @this)
		=> await @this.MoveNextAsync() ? @this.Current : default;

	/// <param name="count">Read this many items.</param>
	public static async IAsyncEnumerator<T> ReadAsync<T>(this IAsyncEnumerator<T> @this, int count)
	{
		while (--count > -1 && await @this.MoveNextAsync())
			yield return @this.Current;
	}

	public static async IAsyncEnumerable<T> RestAsync<T>(this IAsyncEnumerator<T> @this)
	{
		while (await @this.MoveNextAsync())
			yield return @this.Current;
	}

	/// <exception cref="IndexOutOfRangeException" />
	public async static ValueTask<T> SkipAsync<T>(this IAsyncEnumerator<T> @this, int count)
	{
		while (count > 0 && await @this.MoveNextAsync())
			--count;

		return count == 0
			? @this.Current
			: throw new IndexOutOfRangeException($"{nameof(IAsyncEnumerator<T>)}.{nameof(SkipAsync)}: Remaining {nameof(count)} of {count}.");
	}
}
