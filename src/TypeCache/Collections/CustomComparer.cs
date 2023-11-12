// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public readonly struct CustomComparer<T> : IComparer<T>, IEqualityComparer<T>
{
	private readonly Comparison<T?> _Compare;
	private readonly Func<T?, T?, bool> _Equals;
	private readonly Func<T, int> _GetHashCode;

	public CustomComparer(Comparison<T?> compare)
	{
		this._Compare = compare;
		this._Equals = (x, y) => compare(x, y) == 0;
		this._GetHashCode = _ => _?.GetHashCode() ?? 0;
	}

	public CustomComparer(Comparison<T?> compare, Func<T?, T?, bool> equals)
	{
		this._Compare = compare;
		this._Equals = equals;
		this._GetHashCode = _ => _?.GetHashCode() ?? 0;
	}

	public CustomComparer(Comparison<T?> compare, Func<T?, T?, bool> equals, Func<T, int> getHashCode)
	{
		this._Compare = compare;
		this._Equals = equals;
		this._GetHashCode = getHashCode;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int Compare([AllowNull] T x, [AllowNull] T y)
		=> this._Compare(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals([AllowNull] T x, [AllowNull] T y)
		=> this._Equals(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int GetHashCode([DisallowNull] T value)
		=> this._GetHashCode(value);
}
