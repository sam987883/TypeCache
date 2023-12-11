// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Collections;

public readonly struct CustomComparer<T>(Comparison<T?> compare, Func<T?, T?, bool>? equals, Func<T, int>? getHashCode)
	: IComparer<T>, IEqualityComparer<T>
{
	private readonly Comparison<T?> _Compare = compare ?? compare.ThrowArgumentNullException();
	private readonly Func<T?, T?, bool> _Equals = equals ?? ((x, y) => compare(x, y) == 0);
	private readonly Func<T, int> _GetHashCode = getHashCode ?? (_ => _?.GetHashCode() ?? 0);

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
