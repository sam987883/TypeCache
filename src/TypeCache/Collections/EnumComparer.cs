// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.Collections;

public readonly struct EnumComparer<T> : IComparer<T>, IEqualityComparer<T>
	where T : struct, Enum
{
	private static readonly Comparison<T> EnumCompare;
	private static readonly Func<T, T, bool> EnumEquals;
	private static readonly Func<T, int> EnumGetHashCode;

	static EnumComparer()
	{
		var underlyingType = typeof(T).GetEnumUnderlyingType();
		EnumCompare = CreateCompare(underlyingType);
		EnumEquals = CreateEquals(underlyingType);
		EnumGetHashCode = CreateGetHashCode(underlyingType);
	}

	private static Comparison<T> CreateCompare(Type underlyingType)
		=> LambdaFactory.CreateComparison<T>((value1, value2) =>
			value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), new Expression[] { value2.Cast(underlyingType) })).Compile();

	private static Func<T, T, bool> CreateEquals(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, T, bool>((value1, value2) =>
			value1.Cast(underlyingType).Operation(BinaryOperator.EqualTo, value2.Cast(underlyingType))).Compile();

	private static Func<T, int> CreateGetHashCode(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, int>(value =>
			value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int Compare([AllowNull] T x, [AllowNull] T y)
		=> EnumCompare(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals([AllowNull] T x, [AllowNull] T y)
		=> EnumEquals(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int GetHashCode([DisallowNull] T value)
		=> EnumGetHashCode(value);
}
