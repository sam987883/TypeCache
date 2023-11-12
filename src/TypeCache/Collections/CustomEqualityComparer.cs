// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public readonly struct CustomEqualityComparer<T> : IEqualityComparer<T>
{
	private readonly Func<T?, T?, bool> _Equals;
	private readonly Func<T?, int> _GetHashCode;

	public CustomEqualityComparer(Func<T?, T?, bool> equals)
	{
		this._Equals = equals;
		this._GetHashCode = _ => _?.GetHashCode() ?? 0;
	}

	public CustomEqualityComparer(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
	{
		this._Equals = equals;
		this._GetHashCode = getHashCode;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals([AllowNull] T? x, [AllowNull] T? y)
		=> this._Equals(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int GetHashCode([DisallowNull] T value)
		=> this._GetHashCode(value);
}
