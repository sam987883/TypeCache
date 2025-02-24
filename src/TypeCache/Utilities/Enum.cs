// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class Enum<T>
	where T : struct, Enum
{
	private static readonly Comparison<T> EnumCompare;
	private static readonly Func<T, T, bool> EnumEquals;
	private static readonly Func<T, int> EnumGetHashCode;

	static Enum()
	{
		var underlyingType = typeof(T).GetEnumUnderlyingType();
		EnumCompare = CreateCompare(underlyingType);
		EnumEquals = CreateEquals(underlyingType);
		EnumGetHashCode = CreateGetHashCode(underlyingType);
	}

	private static Comparison<T> CreateCompare(Type underlyingType)
		=> LambdaFactory.CreateComparison<T>((value1, value2) =>
			value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), [value2.Cast(underlyingType)])).Compile();

	private static Func<T, T, bool> CreateEquals(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, T, bool>((value1, value2) =>
			value1.Cast(underlyingType).Operation(BinaryOperator.Equal, value2.Cast(underlyingType))).Compile();

	private static Func<T, int> CreateGetHashCode(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, int>(value =>
			value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Compare([AllowNull] T x, [AllowNull] T y)
		=> EnumCompare(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Equals([AllowNull] T x, [AllowNull] T y)
		=> EnumEquals(x, y);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int GetHashCode([DisallowNull] T value)
		=> EnumGetHashCode(value);

	[DebuggerHidden]
	public static IReadOnlySet<Attribute> Attributes { get; } = typeof(T).GetCustomAttributes(false).Cast<Attribute>().ToFrozenSet();

	[DebuggerHidden]
	public static bool Flags { get; } = typeof(T).HasCustomAttribute<FlagsAttribute>();

	[DebuggerHidden]
	public static string Name { get; } = typeof(T).Name;

	[DebuggerHidden]
	public static IReadOnlySet<T> Values { get; } = Enum.GetValues<T>().ToFrozenSet();

	[DebuggerHidden]
	public static Type UnderlyingType => typeof(T).GetEnumUnderlyingType();

	[DebuggerHidden]
	public static bool IsDefined(T value)
		=> Enum.IsDefined(value) || (Flags && Values.Any(_ => value.HasFlag(_)));
}
