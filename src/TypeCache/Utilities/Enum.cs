// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class Enum<T>
	where T : struct, Enum
{
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
