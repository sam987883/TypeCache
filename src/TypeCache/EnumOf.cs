// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache;

public static class EnumOf<T>
	where T : struct, Enum
{
	private static Comparison<T> CreateCompare(Type underlyingType)
		=> LambdaFactory.CreateComparison<T>((value1, value2) => value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), value2.Convert(underlyingType))).Compile();

	private static Func<T, T, bool> CreateEquals(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, T, bool>((value1, value2) => value1.Cast(underlyingType).Operation(BinaryOperator.EqualTo, value2.Convert(underlyingType))).Compile();

	private static Func<T, int> CreateGetHashCode(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, int>(value => value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();

	static EnumOf()
	{
		const BindingFlags STATIC_BINDINGS = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static;

		var underlyingType = typeof(T).GetEnumUnderlyingType();
		var compare = CreateCompare(underlyingType);
		var equals = CreateEquals(underlyingType);
		var getHashCode = CreateGetHashCode(underlyingType);

		Attributes = typeof(T).GetCustomAttributes<Attribute>().ToImmutableArray();
		Comparer = new CustomComparer<T>(compare, equals, getHashCode);
		Tokens = typeof(T).GetFields(STATIC_BINDINGS)
			.Where(fieldInfo => fieldInfo.FieldType.IsAssignableTo<T>())
			.Select(fieldInfo => new Token<T>(fieldInfo))
			.ToImmutableArray();
	}

	public static IReadOnlyCollection<Attribute> Attributes { get; }

	public static CustomComparer<T> Comparer { get; }

	public static bool Flags { get; } = typeof(T).GetCustomAttribute<FlagsAttribute>() is not null;

	public static RuntimeTypeHandle Handle { get; } = typeof(T).TypeHandle;

	public static bool Internal { get; } = !typeof(T).IsVisible;

	public static string Name { get; } = typeof(T).Name();

	public static bool Public { get; } = typeof(T).IsPublic;

	public static IReadOnlyCollection<Token<T>> Tokens { get; }

	public static Type UnderlyingType => typeof(T).GetEnumUnderlyingType();

	public static Token<T>? GetToken(T value)
		=> Tokens.FirstOrDefault(token => Comparer.Equals(token.Value, value));

	public static Token<T>? GetToken(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> Tokens.FirstOrDefault(token => name.Is(token.Name, comparison));

	[DebuggerHidden]
	public static bool IsDefined(T value)
		=> Tokens.Any(token => Comparer.Equals(token.Value, value));

	[DebuggerHidden]
	public static string Parse(T value)
		=> Tokens.FirstOrDefault(token => Comparer.Equals(token.Value, value))?.Name ?? value.ToString("G");
}
