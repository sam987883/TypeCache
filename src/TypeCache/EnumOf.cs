// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
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
		private const BindingFlags STATIC_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public sealed class Token
			: Member, IEquatable<Token>
		{
			internal Token(FieldInfo fieldInfo)
				: base(fieldInfo, fieldInfo.IsAssembly, fieldInfo.IsPublic)
			{
				this.Value = (T)Enum.Parse<T>(fieldInfo.Name);
				this.Hex = this.Value.ToString("X");
				this.Number = this.Value.ToString("D");
			}

			public string Hex { get; }

			public string Number { get; }

			public T Value { get; }

			public bool Equals(Token? other)
				=> other != null && Comparer.Equals(this.Value, other.Value) && other.Name.Is(this.Name, StringComparison.Ordinal);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public override int GetHashCode()
				=> Comparer.GetHashCode(this.Value);
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

		static EnumOf()
		{
			var type = typeof(T);
			var underlyingType = type.GetEnumUnderlyingType();
			var compare = CreateCompare(underlyingType);
			var equals = CreateEquals(underlyingType);
			var getHashCode = CreateGetHashCode(underlyingType);

			Attributes = type.GetCustomAttributes<Attribute>().ToImmutableArray();
			Comparer = new CustomComparer<T>(compare, equals, getHashCode);
			Handle = type.TypeHandle;
			IsFlags = Attributes.Any<FlagsAttribute>();
			IsInternal = !type.IsVisible;
			IsPublic = type.IsPublic;
			Name = type.GetName();
			UnderlyingType = type.GetSystemType();
			UnderlyingTypeHandle = underlyingType.TypeHandle;

			Tokens = type.GetFields(STATIC_BINDINGS).To(fieldInfo => new Token(fieldInfo)).ToImmutableDictionary(_ => _.Value, Comparer);
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
