// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using TypeCache.Extensions;

namespace TypeCache.Collections;

public sealed class ReadOnlyList<T>(IEnumerable<T> enumerable) : IReadOnlyList<T>
{
	public T this[int index] => enumerable switch
	{
		T[] array => array[index],
		IList<T> list => list[index],
		IReadOnlyList<T> list => list[index],
		_ => enumerable.Skip(index).First()
	};

	public int Count { get; } = enumerable switch
	{
		T[] array => array.Length,
		ICollection<T> collection => collection.Count,
		ICollection collection => collection.Count,
		IReadOnlyCollection<T> collection => collection.Count,
		_ => enumerable.Count()
	};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<T> GetEnumerator()
		=> enumerable.GetEnumerator();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)enumerable).GetEnumerator();
}
