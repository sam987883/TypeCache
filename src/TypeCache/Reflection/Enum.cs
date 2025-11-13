// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public static class Enum<T>
	where T : struct, Enum
{
	[DebuggerHidden]
	public static IReadOnlySet<Attribute> Attributes => Type<T>.Attributes;

	[DebuggerHidden]
	public static bool Flags { get; } = Type<T>.Attributes.Any<FlagsAttribute>();

	[DebuggerHidden]
	public static string Name { get; } = Type<T>.Name;

	[DebuggerHidden]
	public static IReadOnlySet<T> Values { get; } = Enum.GetValues<T>().ToFrozenSet();

	[DebuggerHidden]
	public static ScalarType UnderlyingType { get; } = TypeStore.ScalarTypes[typeof(T).GetEnumUnderlyingType().TypeHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsDefined(T value)
		=> Enum.IsDefined(value);
}
