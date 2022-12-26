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

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Move(<paramref name="index"/> + 1) ? @<paramref name="this"/>.Current : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? Get<T>(this IEnumerator<T> @this, int index)
		=> @this.Move(index + 1) ? @this.Current : default;

	public static bool IfGet(this IEnumerator @this, int index, [NotNullWhen(true)] out object? item)
	{
		if (@this.Move(index + 1))
		{
			item = @this.Current!;
			return true;
		}

		item = null;
		return false;
	}

	public static bool IfGet<T>(this IEnumerator<T> @this, int index, [NotNullWhen(true)] out T? item)
	{
		if (@this.Move(index + 1))
		{
			item = @this.Current!;
			return true;
		}

		item = default;
		return false;
	}

	public static bool IfNext(this IEnumerator @this, [NotNullWhen(true)] out object? item)
	{
		if (@this.MoveNext())
		{
			item = @this.Current!;
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

	public static bool Move(this IEnumerator @this, int count)
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

	/// <param name="count">Read this many items.</param>
	public static IEnumerable<T> Read<T>(this IEnumerator<T> @this, int count)
	{
		while (--count > -1 && @this.MoveNext())
			yield return @this.Current;
	}

	public static IEnumerable<T> Rest<T>(this IEnumerator<T> @this)
	{
		while (@this.MoveNext())
			yield return @this.Current;
	}
}
