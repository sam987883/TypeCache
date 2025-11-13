// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Collections;

public class Collection<T>(ICollection<T> collection) : ICollection<T>
{
	public int Count { get; } = collection.Count;

	public bool IsReadOnly => false;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void Add(T item)
		=> collection.Add(item);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void Clear()
		=> collection.Clear();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Contains(T item)
		=> collection.Contains(item);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void CopyTo(T[] array, int arrayIndex)
		=> collection.CopyTo(array, arrayIndex);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<T> GetEnumerator()
		=> collection.GetEnumerator();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Remove(T item)
		=> collection.Remove(item);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> collection.GetEnumerator();
}
