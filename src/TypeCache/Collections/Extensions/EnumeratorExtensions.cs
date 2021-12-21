// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class EnumeratorExtensions
{
	/// <summary>
	/// <code>
	/// <see langword="var"/> <paramref name="count"/> = 0;<br/>
	/// <see langword="while"/> (@<paramref name="this"/>.MoveNext())<br/>
	///	<see langword="    "/>++<paramref name="count"/>;<br/>
	///	<see langword="return"/> <paramref name="count"/>;
	/// </code>
	/// </summary>
	public static int Count(this IEnumerator @this)
	{
		var count = 0;
		while (@this.MoveNext())
			++count;
		return count;
	}

	/// <summary>
	/// <code>
	/// <paramref name="first"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="rest"/> = @<paramref name="this"/>.Rest();
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out IEnumerable<T> rest)
		where T : struct
	{
		first = @this.MoveNext() ? @this.Current : null;
		rest = @this.Rest();
	}

	/// <summary>
	/// <code>
	/// <paramref name="first"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="second"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="rest"/> = @<paramref name="this"/>.Rest();
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
		where T : struct
	{
		first = @this.MoveNext() ? @this.Current : null;
		second = @this.MoveNext() ? @this.Current : null;
		rest = @this.Rest();
	}

	/// <summary>
	/// <code>
	/// <paramref name="first"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="second"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="third"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="rest"/> = @<paramref name="this"/>.Rest();
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		where T : struct
	{
		first = @this.MoveNext() ? @this.Current : null;
		second = @this.MoveNext() ? @this.Current : null;
		third = @this.MoveNext() ? @this.Current : null;
		rest = @this.Rest();
	}

	/// <summary>
	/// <code>
	/// <paramref name="first"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="rest"/> = @<paramref name="this"/>.Rest();
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out IEnumerable<T> rest)
		where T : class
	{
		first = @this.MoveNext() ? @this.Current : null;
		rest = @this.Rest();
	}

	/// <summary>
	/// <code>
	/// <paramref name="first"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="second"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="rest"/> = @<paramref name="this"/>.Rest();
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
		where T : class
	{
		first = @this.MoveNext() ? @this.Current : null;
		second = @this.MoveNext() ? @this.Current : null;
		rest = @this.Rest();
	}

	/// <summary>
	/// <code>
	/// <paramref name="first"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="second"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="third"/> = @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="null"/>;<br/>
	/// <paramref name="rest"/> = @<paramref name="this"/>.Rest();
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerator<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		where T : class
	{
		first = @this.MoveNext() ? @this.Current : null;
		second = @this.MoveNext() ? @this.Current : null;
		third = @this.MoveNext() ? @this.Current : null;
		rest = @this.Rest();
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TryGet(<paramref name="index"/>, <see langword="out var"/> item) ? item : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Get<T>(this IEnumerator<T> @this, int index)
		=> @this.TryGet(index, out var item) ? item : default;

	/// <summary>
	/// <code>
	/// <see langword="while"/> (<paramref name="count"/> &gt; 0 &amp;&amp; @<paramref name="this"/>.MoveNext())<br/>
	///	<see langword="    "/>--<paramref name="count"/>;<br/>
	///	<see langword="return"/> <paramref name="count"/> == 0;
	/// </code>
	/// </summary>
	public static bool Move(this IEnumerator @this, int count)
	{
		while (count > 0 && @this.MoveNext())
			--count;
		return count == 0;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TryNext(<see langword="out var"/> item) ? item : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Next<T>(this IEnumerator<T> @this)
		=> @this.TryNext(out var item) ? item : default;

	/// <summary>
	/// <code>
	/// <see langword="while"/> (--<paramref name="count"/> &gt; -1 &amp;&amp; @<paramref name="this"/>.MoveNext())<br/>
	/// <see langword="    yield return"/> @<paramref name="this"/>.Current;
	/// </code>
	/// </summary>
	/// <param name="count">Read this many items.</param>
	public static IEnumerable<T> Read<T>(this IEnumerator<T> @this, int count)
	{
		while (--count > -1 && @this.MoveNext())
			yield return @this.Current;
	}

	/// <summary>
	/// <code>
	/// <see langword="while"/> (@<paramref name="this"/>.MoveNext())<br/>
	/// <see langword="    yield return"/> @<paramref name="this"/>.Current;
	/// </code>
	/// </summary>
	public static IEnumerable<T> Rest<T>(this IEnumerator<T> @this)
	{
		while (@this.MoveNext())
			yield return @this.Current;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> success = @<paramref name="this"/>.Move(<paramref name="index"/>.Next().Value);<br/>
	/// <paramref name="item"/> = success ? @<paramref name="this"/>.Current : <see langword="default"/>;<br/>
	/// <see langword="return"/> success;
	/// </code>
	/// </summary>
	public static bool TryGet<T>(this IEnumerator<T> @this, int index, [NotNullWhen(true)] out T? item)
	{
		var success = @this.Move(index + 1);
		item = success ? @this.Current : default;
		return success;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> success = @<paramref name="this"/>.MoveNext();<br/>
	/// <paramref name="item"/> = success ? @<paramref name="this"/>.Current : <see langword="default"/>;<br/>
	/// <see langword="return"/> success;
	/// </code>
	/// </summary>
	public static bool TryNext<T>(this IEnumerator<T> @this, [NotNullWhen(true)] out T? item)
	{
		var success = @this.MoveNext();
		item = success ? @this.Current : default;
		return success;
	}
}
