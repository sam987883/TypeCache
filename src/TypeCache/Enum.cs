// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Expressions;
using TypeCache.Reflection.Extensions;

namespace TypeCache
{
	public static class Enum<T>
		where T : struct, Enum
	{
		public readonly struct Token
		{
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
				.Operation(Equality.EqualTo, value2.ConvertTo(underlyingType))
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

		private static Token CreateToken(StaticFieldMember field)
			=> new Token
			{
				Attributes = field.Attributes,
				Name = field.Name,
				Value = (T)field.GetValue!()!
			};

		private static readonly TypeMember Type;

		static Enum()
		{
			Type = MemberCache.Types[typeof(T).TypeHandle];

			var underlyingType = typeof(T).GetEnumUnderlyingType();
			var compare = CreateCompare(underlyingType);
			var equals = CreateEquals(underlyingType);
			var getHashCode = CreateGetHashCode(underlyingType);

			Comparer = new CustomComparer<T>(compare, equals, getHashCode);
			HasFlags = Type.Attributes.Any<FlagsAttribute>();
			UnderlyingTypeHandle = underlyingType.TypeHandle;

			Tokens = TypeOf<T>.StaticFields.Values.To(CreateToken).ToImmutable();
			Map = Tokens.ToImmutableDictionary(_ => _.Value, Comparer);
		}

		public static IImmutableList<Attribute> Attributes => Type.Attributes;

		public static CustomComparer<T> Comparer { get; }

		public static RuntimeTypeHandle Handle => Type.Handle;

		public static bool HasFlags { get; }

		public static bool IsInternal => Type.IsInternal;

		public static bool IsPublic => Type.IsPublic;

		public static IImmutableDictionary<T, Enum<T>.Token> Map { get; }

		public static string Name => Type.Name;

		public static IImmutableList<Token> Tokens { get; }

		public static NativeType UnderlyingType => Type.NativeType;

		public static RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}
}
