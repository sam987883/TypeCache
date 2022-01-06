﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions;

public static class ListExtensions
{
	public static void Deconstruct<T>(this List<T>? @this, out T? first, out IEnumerable<T> rest)
		where T : struct
	{
		first = @this?.Count > 0 ? @this[0] : null;
		rest = @this?.Count > 1 ? @this.Skip(1) : Enumerable<T>.Empty;
	}

	public static void Deconstruct<T>(this List<T>? @this, out T? first, out T? second, out IEnumerable<T> rest)
		where T : struct
	{
		first = @this?.Count > 0 ? @this[0] : null;
		second = @this?.Count > 1 ? @this[1] : null;
		rest = @this?.Count > 2 ? @this.Skip(2) : Enumerable<T>.Empty;
	}

	public static void Deconstruct<T>(this List<T>? @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		where T : struct
	{
		first = @this?.Count > 0 ? @this[0] : null;
		second = @this?.Count > 1 ? @this[1] : null;
		third = @this?.Count > 2 ? @this[2] : null;
		rest = @this?.Count > 3 ? @this.Skip(3) : Enumerable<T>.Empty;
	}

	public static void Deconstruct<T>(this List<T>? @this, out T? first, out IEnumerable<T> rest)
		where T : class
	{
		first = @this?.Count > 0 ? @this[0] : null;
		rest = @this?.Count > 1 ? @this.Skip(1) : Enumerable<T>.Empty;
	}

	public static void Deconstruct<T>(this List<T>? @this, out T? first, out T? second, out IEnumerable<T> rest)
		where T : class
	{
		first = @this?.Count > 0 ? @this[0] : null;
		second = @this?.Count > 1 ? @this[1] : null;
		rest = @this?.Count > 2 ? @this.Skip(2) : Enumerable<T>.Empty;
	}

	public static void Deconstruct<T>(this List<T>? @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		where T : class
	{
		first = @this?.Count > 0 ? @this[0] : null;
		second = @this?.Count > 1 ? @this[1] : null;
		third = @this?.Count > 2 ? @this[2] : null;
		rest = @this?.Count > 3 ? @this.Skip(3) : Enumerable<T>.Empty;
	}

	public static void Do<T>(this List<T>? @this, Action<T> action)
	{
		action.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
			action(@this![i]);
	}

	public static void Do<T>(this List<T>? @this, Action<T, int> action)
	{
		action.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
			action(@this![i], i);
	}

	public static void Do<T>(this List<T>? @this, Action<T> action, Action between)
	{
		action.AssertNotNull();
		between.AssertNotNull();

		var count = @this?.Count ?? 0;
		if (count > 0)
		{
			action(@this![0]);
			for (var i = 1; i < count; ++i)
			{
				between();
				action(@this[i]);
			}
		}
	}

	public static void Do<T>(this List<T>? @this, Action<T, int> action, Action between)
	{
		action.AssertNotNull();
		between.AssertNotNull();

		var count = @this?.Count ?? 0;
		if (count > 0)
		{
			action(@this![0], 0);
			for (var i = 1; i < count; ++i)
			{
				between();
				action(@this[i], i);
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this List<T>? @this, Func<T, T> edit)
	{
		edit.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
			yield return edit(@this![i]);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this List<T>? @this, Func<T, int, T> edit)
	{
		edit.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
			yield return edit(@this![i], i);
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static IEnumerable<T> Get<T>(this List<T> @this, Range range)
		=> range.Normalize(@this.Count).Values().Map(i => @this[i]);

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> If<T>(this List<T>? @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
		{
			var item = @this![i];
			if (filter(item))
				yield return item;
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this List<T>? @this, Func<T, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
		{
			var item = @this![i];
			if (await filter(item))
				yield return item;

			if (token.IsCancellationRequested)
				break;
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this List<T>? @this, Func<T, CancellationToken, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
		{
			var item = @this![i];
			if (await filter(item, token))
				yield return item;

			if (token.IsCancellationRequested)
				break;
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this List<T>? @this, Func<T, V> map)
	{
		map.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
			yield return map(@this![i]);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this List<T>? @this, Func<T, int, V> map)
	{
		map.AssertNotNull();

		var count = @this?.Count ?? 0;
		for (var i = 0; i < count; ++i)
			yield return map(@this![i], i);
	}

	/// <exception cref="ArgumentNullException"/>
	internal static async IAsyncEnumerable<V> MapAsync<T, V>(T[]? @this, Func<T, Task<V>> map, [EnumeratorCancellation] CancellationToken _ = default)
	{
		map.AssertNotNull();

		if (@this is null)
			yield break;

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			yield return await map(@this[i]);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/>.Has(<paramref name="index"/>))<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = @<paramref name="this"/>[<paramref name="index"/>];<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <paramref name="item"/> = <see langword="default"/>;<br/>
	/// <see langword="return false"/>;
	/// </code>
	/// </summary>
	public static bool TryGet<T>(this IList<T>? @this, Index index, out T? item)
	{
		if (@this.Has(index))
		{
			item = @this[index];
			return true;
		}
		item = default;
		return false;
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/>.Has(<paramref name="index"/>))<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = @<paramref name="this"/>[<paramref name="index"/>];<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <paramref name="item"/> = <see langword="default"/>;<br/>
	/// <see langword="return false"/>;
	/// </code>
	/// </summary>
	public static bool TryGet<T>(this IReadOnlyList<T>? @this, Index index, out T? item)
	{
		if (@this.Has(index))
		{
			item = @this[index];
			return true;
		}
		item = default;
		return false;
	}

	/// <summary>
	/// <code>
	/// <paramref name="filter"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> @this <see langword="is not null"/> ? (0..(@<paramref name="this"/>.Count).Values().If(i =&gt; <paramref name="filter"/>(@<paramref name="this"/>[i])) : Enumerable&lt;<see cref="int"/>&gt;.Empty;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static IEnumerable<int> ToIndex<T>(this List<T>? @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		return @this is not null ? (0..@this.Count).Values().If(i => filter(@this[i])) : Enumerable<int>.Empty;
	}
}
