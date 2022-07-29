// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions;

public static class ImmutableArrayExtensions
{
	/// <remarks>
	/// <code>
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IsEmpty)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/><paramref name="each"/>(@<paramref name="this"/>[i]);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this ImmutableArray<T> @this, Func<T, T> edit)
	{
		edit.AssertNotNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			yield return edit(@this[i]);
	}

	/// <remarks>
	/// <code>
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IsEmpty)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/><paramref name="each"/>(@<paramref name="this"/>[i], i);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this ImmutableArray<T> @this, Func<T, int, T> edit)
	{
		edit.AssertNotNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			yield return edit(@this[i], i);
	}

	/// <exception cref="IndexOutOfRangeException" />
	public static IEnumerable<T> Get<T>(this ImmutableArray<T> @this, Range range)
		=> @this.AsSpan().ToArray().Get(range);

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> If<T>(this ImmutableArray<T> @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
		{
			var item = @this[i];
			if (filter(item))
				yield return item;
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this ImmutableArray<T> @this, Func<T, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
		{
			if (token.IsCancellationRequested)
				yield break;

			var item = @this[i];
			if (await filter(item))
				yield return item;
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this ImmutableArray<T> @this, Func<T, CancellationToken, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
		{
			if (token.IsCancellationRequested)
				yield break;

			var item = @this[i];
			if (await filter(item, token))
				yield return item;
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this ImmutableArray<T> @this, Func<T, V> map)
	{
		map.AssertNotNull();

		var count = @this.Length;
		var array = new V[count];
		for (var i = 0; i < count; ++i)
			array[i] = map(@this[i]);

		return array;
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this ImmutableArray<T> @this, Func<T, int, V> map)
	{
		map.AssertNotNull();

		var count = @this.Length;
		var array = new V[count];
		for (var i = 0; i < count; ++i)
			array[i] = map(@this[i], i);

		return array;
	}
}
