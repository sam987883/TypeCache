// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.Collections;

public readonly struct EnumComparer<T> : IComparer<T>, IEqualityComparer<T>
	where T : struct, Enum
{
	private readonly Comparison<T> _Compare;
	private readonly Func<T, T, bool> _Equals;
	private readonly Func<T, int> _GetHashCode;

	private static Comparison<T> CreateCompare(Type underlyingType)
		=> LambdaFactory.CreateComparison<T>((value1, value2) =>
			value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), value2.Convert(underlyingType))).Compile();

	private static Func<T, T, bool> CreateEquals(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, T, bool>((value1, value2) =>
			value1.Cast(underlyingType).Operation(BinaryOperator.EqualTo, value2.Convert(underlyingType))).Compile();

	private static Func<T, int> CreateGetHashCode(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, int>(value =>
			value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();

	public EnumComparer()
	{
		var underlyingType = typeof(T).GetEnumUnderlyingType();
		this._Compare = CreateCompare(underlyingType);
		this._Equals = CreateEquals(underlyingType);
		this._GetHashCode = CreateGetHashCode(underlyingType);
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
