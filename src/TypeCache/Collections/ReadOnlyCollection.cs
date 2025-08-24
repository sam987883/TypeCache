// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Collections;

public sealed class ReadOnlyCollection<T>(IEnumerable<T> enumerable) : IReadOnlyCollection<T>
{
	public int Count { get; } = enumerable.TryGetNonEnumeratedCount(out var count) ? count : enumerable.Count();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<T> GetEnumerator()
		=> enumerable.GetEnumerator();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)enumerable).GetEnumerator();
}
