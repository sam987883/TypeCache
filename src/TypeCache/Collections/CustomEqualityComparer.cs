// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Collections;

public readonly struct CustomEqualityComparer<T>(Func<T?, T?, bool> equals, Func<T?, int>? getHashCode)
	: IEqualityComparer<T>
{
	private readonly Func<T?, T?, bool> _Equals = equals ?? equals.ThrowArgumentNullException();
	private readonly Func<T?, int> _GetHashCode = getHashCode ?? (_ => _?.GetHashCode() ?? 0);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals([AllowNull] T? x, [AllowNull] T? y)
		=> this._Equals(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int GetHashCode([DisallowNull] T value)
		=> this._GetHashCode(value);
}
