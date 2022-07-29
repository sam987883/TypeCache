// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions;

public static class ListExtensions
{
	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this IList<T>? @this, Func<T, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		if (@this?.Count > 0)
		{
			var count = @this.Count;
			for (var i = 0; i < count; ++i)
			{
				var item = @this[i];
				if (await filter(item))
					yield return item;

				if (token.IsCancellationRequested)
					break;
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static IAsyncEnumerable<T> IfAsync<T>(this List<T>? @this, Func<T, Task<bool>> filter, CancellationToken token = default)
		=> (@this as IList<T>).IfAsync(filter, token);

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this IList<T>? @this, Func<T, CancellationToken, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		if (@this?.Count > 0)
		{
			var count = @this.Count;
			for (var i = 0; i < count; ++i)
			{
				var item = @this[i];
				if (await filter(item, token))
					yield return item;

				if (token.IsCancellationRequested)
					yield break;
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static IAsyncEnumerable<T> IfAsync<T>(this List<T>? @this, Func<T, CancellationToken, Task<bool>> filter, CancellationToken token = default)
		=> (@this as IList<T>).IfAsync(filter, token);

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/>.Has(<paramref name="index"/>))<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = @<paramref name="this"/>[<paramref name="index"/>];<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <br/>
	/// <paramref name="item"/> = <see langword="default"/>;<br/>
	/// <see langword="return false"/>;
	/// </code>
	/// </summary>
	public static bool IfGet<T>(this IList<T>? @this, Index index, out T? item)
	{
		if (@this.Has(index))
		{
			item = @this[index];
			return true;
		}

		item = default;
		return false;
	}

	/// <exception cref="ArgumentNullException"/>
	public static bool IfGet<T>(this List<T>? @this, Index index, out T? item)
		=> (@this as IList<T>).IfGet(index, out item);

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this IList<T>? @this, Func<T, V> map)
	{
		map.AssertNotNull();

		if (@this?.Count > 0)
		{
			var count = @this.Count;
			var array = new V[count];
			for (var i = 0; i < count; ++i)
				array[i] = map(@this[i]);

			return array;
		}

		return Array.Empty<V>();
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this List<T>? @this, Func<T, V> map)
		=> (@this as IList<T>).Map(map);

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this IList<T>? @this, Func<T, int, V> map)
	{
		map.AssertNotNull();

		if (@this?.Count > 0)
		{
			var count = @this.Count;
			var array = new V[count];
			for (var i = 0; i < count; ++i)
				array[i] = map(@this[i], i);

			return array;
		}

		return Array.Empty<V>();
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this List<T>? @this, Func<T, int, V> map)
		=> (@this as IList<T>).Map(map);

	/// <exception cref="ArgumentNullException"/>
	internal static async IAsyncEnumerable<V> MapAsync<T, V>(this IList<T>? @this, Func<T, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
	{
		map.AssertNotNull();

		if (@this?.Count > 0)
		{
			var count = @this.Count;
			for (var i = 0; i < count; ++i)
			{
				if (token.IsCancellationRequested)
					yield break;

				yield return await map(@this[i]);
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static IAsyncEnumerable<V> MapAsync<T, V>(this List<T>? @this, Func<T, Task<V>> map, CancellationToken token = default)
		=> (@this as IList<T>).MapAsync(map, token);
}
