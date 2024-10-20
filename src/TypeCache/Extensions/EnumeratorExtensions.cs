// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Extensions;

public static class EnumeratorExtensions
{
	public static int Count(this IEnumerator @this)
	{
		var count = 0;
		while (@this.MoveNext())
			++count;
		return count;
	}

	public static int Count<E>(this ref E @this)
		where E : struct, IEnumerator
	{
		var count = 0;
		while (@this.MoveNext())
			++count;
		return count;
	}

	public static bool IfNext(this IEnumerator @this, [NotNullWhen(true)] out object? item)
	{
		if (@this.MoveNext())
		{
			item = @this.Current;
			return true;
		}

		item = null;
		return false;
	}

	public static bool IfNext<E>(this ref E @this, [NotNullWhen(true)] out object? item)
		where E : struct, IEnumerator
	{
		if (@this.MoveNext())
		{
			item = @this.Current;
			return true;
		}

		item = null;
		return false;
	}

	public static bool IfNext<T>(this IEnumerator<T> @this, [NotNullWhen(true)] out T? item)
	{
		if (@this.MoveNext())
		{
			item = @this.Current!;
			return true;
		}

		item = default;
		return false;
	}

	public static bool IfNext<E, T>(this ref E @this, [NotNullWhen(true)] out T? item)
		where E : struct, IEnumerator<T>
	{
		if (@this.MoveNext())
		{
			item = @this.Current!;
			return true;
		}

		item = default;
		return false;
	}

	public static bool Move(this IEnumerator @this, int count)
	{
		while (count > 0 && @this.MoveNext())
			--count;

		return count == 0;
	}

	public static bool Move<E>(this ref E @this, int count)
		where E : struct, IEnumerator
	{
		while (count > 0 && @this.MoveNext())
			--count;

		return count == 0;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? Next<T>(this IEnumerator<T> @this)
		=> @this.MoveNext() ? @this.Current : default;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.MoveNext() ? @<paramref name="this"/>.Current : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? Next<E, T>(this ref E @this)
		where E : struct, IEnumerator<T>
		=> @this.MoveNext() ? @this.Current : default;

	public static IEnumerable<T> Rest<T>(this IEnumerator<T> @this)
	{
		while (@this.MoveNext())
			yield return @this.Current;
	}
}
