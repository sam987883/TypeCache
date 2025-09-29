// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public class ReadOnlySet<T>(IReadOnlySet<T> values) : ReadOnlyCollection<T>(values), IReadOnlySet<T>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Contains(T item)
		=> values.Contains(item);

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
}
