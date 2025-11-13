// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Utilities;

public readonly struct EnumComparer<T> : IComparer<T>, IEqualityComparer<T>
	where T : struct, Enum
{
	private static readonly Lazy<Comparison<T>> _Compare = new(CreateCompare, true);
	private static readonly Lazy<Func<T, T, bool>> _Equals = new(CreateEquals, true);
	private static readonly Lazy<Func<T, int>> _GetHashCode = new(CreateGetHashCode, true);

	private static Comparison<T> CreateCompare()
	{
		var underlyingType = typeof(T).GetEnumUnderlyingType();
		return LambdaFactory.CreateComparison<T>((value1, value2) =>
			value1.Cast(underlyingType).Call(nameof(IComparable<>.CompareTo), [value2.Cast(underlyingType)])).Compile();
	}

	private static Func<T, T, bool> CreateEquals()
	{
		var underlyingType = typeof(T).GetEnumUnderlyingType();
		return LambdaFactory.CreateFunc<T, T, bool>((value1, value2) =>
			value1.Cast(underlyingType).Operator(BinaryOperator.Equal, value2.Cast(underlyingType))).Compile();
	}

	private static Func<T, int> CreateGetHashCode()
	{
		var underlyingType = typeof(T).GetEnumUnderlyingType();
		return LambdaFactory.CreateFunc<T, int>(value =>
			value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int Compare([AllowNull] T x, [AllowNull] T y)
		=> _Compare.Value(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals([AllowNull] T x, [AllowNull] T y)
		=> _Equals.Value(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public int GetHashCode([DisallowNull] T value)
		=> _GetHashCode.Value(value);
}
