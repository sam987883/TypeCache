// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
		public record Token(string Name, T Value, IImmutableList<Attribute> Attributes) : IEquatable<Token>
		{
			public bool Equals(Token token1, Token token2)
				=> token1.Name.Is(token2.Name, StringComparison.Ordinal) && Enum<T>.Comparer.Equals(token1.Value, token2.Value);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public override int GetHashCode()
				=> Enum<T>.Comparer.GetHashCode(this.Value);
		}

		private static Comparison<T> CreateCompare(Type underlyingType)
		{
			ParameterExpression value1 = nameof(value1).Parameter<T>();
			ParameterExpression value2 = nameof(value2).Parameter<T>();
			return value1.Cast(underlyingType)
				.Call(nameof(IComparable<T>.CompareTo), value2.Cast(underlyingType))
				.Lambda<Comparison<T>>(value1, value2)
				.Compile();
		}

		private static Func<T, T, bool> CreateEquals(Type underlyingType)
		{
			ParameterExpression value1 = nameof(value1).Parameter<T>();
			ParameterExpression value2 = nameof(value2).Parameter<T>();
			return value1.Cast(underlyingType)
				.Operation(EqualityOp.EqualTo, value2.Cast(underlyingType))
				.Lambda<Func<T, T, bool>>(value1, value2)
				.Compile();
		}

		private static Func<T, int> CreateGetHashCode(Type underlyingType)
		{
			ParameterExpression value = nameof(value).Parameter<T>();
			return value.Cast(underlyingType)
				.Call(nameof(object.GetHashCode))
				.Lambda<Func<T, int>>(value)
				.Compile();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Token CreateToken(StaticFieldMember field)
			=> new Token(field.Name, (T)field.GetValue!()!, field.Attributes);

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

			Tokens = TypeOf<T>.StaticFields.Values.To(CreateToken).ToImmutableDictionary(_ => _.Value, Comparer);
		}

		public static IImmutableList<Attribute> Attributes => Type.Attributes;

		public static CustomComparer<T> Comparer { get; }

		public static RuntimeTypeHandle Handle => Type.Handle;

		public static bool HasFlags { get; }

		public static bool IsInternal => Type.IsInternal;

		public static bool IsPublic => Type.IsPublic;

		public static string Name => Type.Name;

		public static IImmutableDictionary<T, Enum<T>.Token> Tokens { get; }

		public static SystemType UnderlyingType => Type.SystemType;

		public static RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}
}
