// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Attributes;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class EnumOf<T>
	where T : struct, Enum
{
	static EnumOf()
	{
		var type = typeof(T);

		Attributes = type.GetCustomAttributes<Attribute>().ToImmutableArray();
		Comparer = new EnumComparer<T>();
		Flags = Attributes.Any<FlagsAttribute>();
		Name = Attributes.FirstOrDefault<NameAttribute>()?.Name ?? type.Name;
		Tokens = Enum.GetValues<T>().ToImmutableDictionary(value => value, value => new Token<T>(value), Comparer);
	}

	public static IReadOnlyCollection<Attribute> Attributes { get; }

	public static EnumComparer<T> Comparer { get; }

	public static bool Flags { get; }

	public static string Name { get; }

	public static IReadOnlyDictionary<T, Token<T>> Tokens { get; }

	public static Type UnderlyingType => typeof(T).GetEnumUnderlyingType();
}
