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
	/// <see langword="var"/> count = 0;<br/>
	/// <see langword="while"/> (@<paramref name="this"/>.MoveNext())<br/>
	///	<see langword="    "/>++count;<br/>
	///	<see langword="return"/> count;
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
	/// <see langword="if"/> (@<paramref name="this"/>.Move(<paramref name="index"/> + 1)<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = @<paramref name="this"/>.Current;<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = <see langword="null"/>;<br/>
	/// <see langword="    return false"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool TryGet(this IEnumerator @this, int index, [NotNullWhen(true)] out object? item)
	{
		if (@this.Move(index + 1))
		{
			item = @this.Current!;
			return true;
		}
		else
		{
			item = null;
			return false;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/>.Move(<paramref name="index"/> + 1)<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = @<paramref name="this"/>.Current;<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = <see langword="default"/>;<br/>
	/// <see langword="    return false"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool TryGet<T>(this IEnumerator<T> @this, int index, [NotNullWhen(true)] out T? item)
	{
		if (@this.Move(index + 1))
		{
			item = @this.Current!;
			return true;
		}
		else
		{
			item = default;
			return false;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/>.MoveNext())<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = @<paramref name="this"/>.Current;<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = <see langword="null"/>;<br/>
	/// <see langword="    return false"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool TryNext(this IEnumerator @this, [NotNullWhen(true)] out object? item)
	{
		if (@this.MoveNext())
		{
			item = @this.Current!;
			return true;
		}
		else
		{
			item = null;
			return false;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/>.MoveNext())<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = @<paramref name="this"/>.Current;<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="item"/> = <see langword="default"/>;<br/>
	/// <see langword="    return false"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool TryNext<T>(this IEnumerator<T> @this, [NotNullWhen(true)] out T? item)
	{
		if (@this.MoveNext())
		{
			item = @this.Current!;
			return true;
		}
		else
		{
			item = default;
			return false;
		}
	}
}
