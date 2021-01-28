// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache
{
	public static class Enum<T>
		where T : struct, Enum
	{
		public readonly struct Token
		{
			public Token(T value)
			{
				var field = value.GetType().GetField(value.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				var attributes = field?.GetCustomAttributes(true);

				this.Attributes = attributes?.As<Attribute>().ToImmutable() ?? ImmutableArray<Attribute>.Empty;
				this.Name = attributes.First<object, NameAttribute>()?.Name ?? field!.Name;
				this.Value = value;
			}

			public IImmutableList<Attribute> Attributes { get; init; }

			public string Name { get; init; }

			public T Value { get; init; }
		}

		private static Comparison<T> CreateCompare(Type underlyingType)
		{
			ParameterExpression value1 = nameof(value1).Parameter<T>();
			ParameterExpression value2 = nameof(value2).Parameter<T>();
			return value1.ConvertTo(underlyingType)
				.Call(nameof(IComparable<T>.CompareTo), value2.ConvertTo(underlyingType))
				.Lambda<Comparison<T>>(value1, value2)
				.Compile();
		}

		private static Func<T, T, bool> CreateEquals(Type underlyingType)
		{
			ParameterExpression value1 = nameof(value1).Parameter<T>();
			ParameterExpression value2 = nameof(value2).Parameter<T>();
			return value1.ConvertTo(underlyingType)
				.EqualTo(value2.ConvertTo(underlyingType))
				.Lambda<Func<T, T, bool>>(value1, value2)
				.Compile();
		}

		private static Func<T, int> CreateGetHashCode(Type underlyingType)
		{
			ParameterExpression value = nameof(value).Parameter<T>();
			return value.ConvertTo(underlyingType)
				.Call(nameof(object.GetHashCode))
				.Lambda<Func<T, int>>(value)
				.Compile();
		}

		static Enum()
		{
			var type = typeof(T);
			var underlyingType = type.GetEnumUnderlyingType();

			var compare = CreateCompare(underlyingType);
			var equals = CreateEquals(underlyingType);
			var getHashCode = CreateGetHashCode(underlyingType);

			Attributes = type.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();
			Comparer = new CustomComparer<T>(compare, equals, getHashCode);
			HasFlags = type.IsDefined(typeof(FlagsAttribute));
			IsInternal = type.IsVisible;
			Name = Attributes.First<Attribute, NameAttribute>()?.Name ?? type.Name;
			IsPublic = type.IsPublic;
			TypeHandle = type.TypeHandle;
			UnderlyingType = underlyingType.ToNativeType();
			UnderlyingTypeHandle = underlyingType.TypeHandle;

			var values = (T[])type.GetEnumValues();
			Tokens = values.To(value => new Token(value)).ToImmutable(values.Length);
			Map = Tokens.ToImmutableDictionary(_ => _.Value, Comparer);
		}

		public static IImmutableList<Attribute> Attributes { get; }

		public static CustomComparer<T> Comparer { get; }

		public static bool HasFlags { get; }

		public static bool IsInternal { get; }

		public static bool IsPublic { get; }

		public static IImmutableDictionary<T, Enum<T>.Token> Map { get; }

		public static string Name { get; }

		public static IImmutableList<Token> Tokens { get; }

		public static RuntimeTypeHandle TypeHandle { get; }

		public static NativeType UnderlyingType { get; }

		public static RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}
}
