// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Collections;

public readonly struct CustomEnumerable<T>
{
	private readonly Func<IEnumerator<T>> _GetEnumerator;

	/// <exception cref="ArgumentNullException"/>
	public CustomEnumerable(Func<IEnumerator<T>> getEnumerator)
	{
		getEnumerator.AssertNotNull();

		this._GetEnumerator = getEnumerator;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<T> GetEnumerator()
		=> this._GetEnumerator();
}
