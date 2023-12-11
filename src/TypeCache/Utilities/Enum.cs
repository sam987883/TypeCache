// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class Enum<T>
	where T : struct, Enum
{
	private static readonly RuntimeTypeHandle UnderlyingTypeHandle = typeof(T).GetEnumUnderlyingType().TypeHandle;

	[DebuggerHidden]
	public static IReadOnlySet<Attribute> Attributes { get; } = typeof(T).GetCustomAttributes(false).Cast<Attribute>().ToFrozenSet();

	[DebuggerHidden]
	public static bool Flags { get; } = typeof(T).HasCustomAttribute<FlagsAttribute>();

	[DebuggerHidden]
	public static string Name { get; } = typeof(T).Name;

	[DebuggerHidden]
	public static IReadOnlySet<T> Values { get; } = Enum.GetValues<T>().ToFrozenSet();

	[DebuggerHidden]
	public static Type UnderlyingType => UnderlyingTypeHandle.ToType();
}
