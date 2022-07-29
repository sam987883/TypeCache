// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

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
	/// <c>=&gt; <see langword="await"/> @<paramref name="this"/>.MoveAsync(<paramref name="index"/>)
	/// ? @<paramref name="this"/>.Current
	/// : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async ValueTask<T?> GetAsync<T>(this IAsyncEnumerator<T> @this, int index)
		=> await @this.MoveAsync(index) ? @this.Current : default;

	/// <summary>
	/// <code>
	/// <see langword="while"/> (<paramref name="count"/> &gt; 0 &amp;&amp; <see langword="await"/> @<paramref name="this"/>.MoveNextAsync())<br/>
	///	<see langword="    "/>--<paramref name="count"/>;<br/>
	///	<see langword="return"/> <paramref name="count"/> == 0;
	/// </code>
	/// </summary>
	public static async ValueTask<bool> MoveAsync<T>(this IAsyncEnumerator<T> @this, int count)
	{
		while (count > 0 && await @this.MoveNextAsync())
			--count;
		return count == 0;
	}

	/// <summary>
	/// <c>=&gt; <see langword="await"/> @<paramref name="this"/>.MoveNextAsync()
	/// ? @<paramref name="this"/>.Current
	/// : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async ValueTask<T?> NextAsync<T>(this IAsyncEnumerator<T> @this)
		=> await @this.MoveNextAsync() ? @this.Current : default;

	/// <summary>
	/// <code>
	/// <see langword="while"/> (--<paramref name="count"/> &gt; -1 &amp;&amp; @<paramref name="this"/>.MoveNext())<br/>
	/// <see langword="    yield return"/> @<paramref name="this"/>.Current;
	/// </code>
	/// </summary>
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
