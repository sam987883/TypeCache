// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public class Set<T>(ISet<T> values) : Collection<T>(values), ISet<T>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	bool ISet<T>.Add(T item)
		=> values.Add(item);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void ExceptWith(IEnumerable<T> other)
		=> values.ExceptWith(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void IntersectWith(IEnumerable<T> other)
		=> values.IntersectWith(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool IsProperSubsetOf(IEnumerable<T> other)
		=> values.IsProperSubsetOf(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool IsProperSupersetOf(IEnumerable<T> other)
		=> values.IsProperSupersetOf(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool IsSubsetOf(IEnumerable<T> other)
		=> values.IsSubsetOf(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool IsSupersetOf(IEnumerable<T> other)
		=> values.IsSupersetOf(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Overlaps(IEnumerable<T> other)
		=> values.Overlaps(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool SetEquals(IEnumerable<T> other)
		=> values.SetEquals(other);

	public void SymmetricExceptWith(IEnumerable<T> other)
		=> values.SymmetricExceptWith(other);

	public void UnionWith(IEnumerable<T> other)
		=> values.UnionWith(other);
}
