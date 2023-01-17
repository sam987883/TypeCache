// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Attributes;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.Reflection.BindingFlags;

namespace TypeCache;

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
		Tokens = type.GetFields(Public | Static).Select(fieldInfo => new Token<T>(fieldInfo)).ToImmutableArray();
	}

	public static IReadOnlyCollection<Attribute> Attributes { get; }

	public static EnumComparer<T> Comparer { get; }

	public static bool Flags { get; }

	public static string Name { get; }

	public static IReadOnlyCollection<Token<T>> Tokens { get; }

	public static Type UnderlyingType => typeof(T).GetEnumUnderlyingType();
}
