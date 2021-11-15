// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache
{
	public static class EnumOf<T>
		where T : struct, Enum
	{
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
			Flags = Attributes.Any<FlagsAttribute>();
			Internal = !type.IsVisible;
			Public = type.IsPublic;
			Name = type.GetName();
			UnderlyingType = type.GetSystemType();
			UnderlyingTypeHandle = underlyingType.TypeHandle;

			Tokens = type.GetFields(STATIC_BINDING_FLAGS).To(fieldInfo => new Token(fieldInfo)).ToImmutableDictionary(_ => _.Value, Comparer);
		}

		public static IImmutableList<Attribute> Attributes { get; }

		public static CustomComparer<T> Comparer { get; }

		public static bool Flags { get; }

		public static RuntimeTypeHandle Handle { get; }

		public static bool Internal { get; }

		public static string Name { get; }

		public static bool Public { get; }

		public static IImmutableDictionary<T, Token> Tokens { get; }

		public static SystemType UnderlyingType { get; }

		public static RuntimeTypeHandle UnderlyingTypeHandle { get; }

		private static Comparison<T> CreateCompare(Type underlyingType)
			=> DelegateExpressionFactory.CreateComparison<T>((value1, value2) => value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), value2.Cast(underlyingType))).Compile();

		private static Func<T, T, bool> CreateEquals(Type underlyingType)
			=> DelegateExpressionFactory.CreateFunc<T, T, bool>((value1, value2) => value1.Cast(underlyingType).Operation(BinaryOperator.EqualTo, value2.Cast(underlyingType))).Compile();

		private static Func<T, int> CreateGetHashCode(Type underlyingType)
			=> DelegateExpressionFactory.CreateFunc<T, int>(value => value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();

		public readonly struct Token
			: IMember, IEquatable<Token>
		{
			internal Token(FieldInfo fieldInfo)
			{
				this.Attributes = fieldInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
				this.Name = this.Attributes.First<NameAttribute>()?.Name ?? fieldInfo.Name;
				this.Value = (T)Enum.Parse<T>(fieldInfo.Name);
				this.Hex = this.Value.ToString("X");
				this.Number = this.Value.ToString("D");
				this.Internal = fieldInfo.IsAssembly;
				this.Public = fieldInfo.IsPublic;
			}

			public IImmutableList<Attribute> Attributes { get; }

			public string Hex { get; }

			public bool Internal { get; }

			public string Name { get; }

			public string Number { get; }

			public bool Public { get; }

			public T Value { get; }

			public bool Equals(Token other)
				=> Comparer.Equals(this.Value, other.Value) && other.Name.Is(this.Name, NAME_STRING_COMPARISON);

			[MethodImpl(METHOD_IMPL_OPTIONS)]
			public override int GetHashCode()
				=> Comparer.GetHashCode(this.Value);
		}
	}
}
