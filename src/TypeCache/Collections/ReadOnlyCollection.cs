// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Collections;

public class ReadOnlyCollection<T> : IReadOnlyCollection<T>
{
	private readonly Func<IEnumerator<T>> _GetEnumerator;

	public ReadOnlyCollection(IEnumerable<T> enumerable)
	{
		this._GetEnumerator = enumerable.GetEnumerator;
		this.Count = enumerable.TryGetNonEnumeratedCount(out var count) ? count : enumerable.Count();
	}

	public ReadOnlyCollection(IReadOnlyCollection<T> collection)
	{
		this._GetEnumerator = collection.GetEnumerator;
		this.Count = collection.Count;
	}

	public int Count { get; }

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<T> GetEnumerator()
		=> this._GetEnumerator();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> this._GetEnumerator();
}
