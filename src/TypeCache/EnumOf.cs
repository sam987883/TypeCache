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
	public static class EnumOf<T>
		where T : struct, Enum
	{
		public sealed record Token(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic, string Number, string Hex, T Value)
			: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<Token>
		{
			public bool Equals(Token? other)
				=> other?.Name.Is(this.Name, StringComparison.Ordinal) is true && Comparer.Equals(this.Value, other.Value);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public override int GetHashCode()
				=> EnumOf<T>.Comparer.GetHashCode(this.Value);
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

		private static Token CreateToken(StaticFieldMember field)
		{
			var value = (T)field.GetValue!()!;
			return new Token(field.Name, field.Attributes, IsInternal, IsPublic, value.ToString("D"), value.ToString("X"), value);
		}

		static EnumOf()
		{
			var typeMember = typeof(T).ToMember();
			var underlyingType = typeof(T).GetEnumUnderlyingType();
			var compare = CreateCompare(underlyingType);
			var equals = CreateEquals(underlyingType);
			var getHashCode = CreateGetHashCode(underlyingType);

			Attributes = typeMember.Attributes;
			Comparer = new CustomComparer<T>(compare, equals, getHashCode);
			Handle = typeMember.Handle;
			IsFlags = typeMember.Attributes.Any<FlagsAttribute>();
			IsInternal = typeMember.IsInternal;
			IsPublic = typeMember.IsPublic;
			Name = typeMember.Name;
			UnderlyingType = typeMember.SystemType;
			UnderlyingTypeHandle = underlyingType.TypeHandle;

			Tokens = TypeOf<T>.StaticFields.Values.To(CreateToken).ToImmutableDictionary(_ => _.Value, Comparer);
		}

		public static IImmutableList<Attribute> Attributes { get; }

		public static CustomComparer<T> Comparer { get; }

		public static RuntimeTypeHandle Handle { get; }

		public static bool IsFlags { get; }

		public static bool IsInternal { get; }

		public static bool IsPublic { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(T token)
			=> Tokens.Keys.Has(token, Comparer);

		public static string Name { get; }

		public static IImmutableDictionary<T, Token> Tokens { get; }

		public static SystemType UnderlyingType { get; }

		public static RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}
}
