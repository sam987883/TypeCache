// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("Enum {Name,nq}", Name = "{Name}")]
public class EnumMember<T> : Member, IEquatable<EnumMember<T>>
	where T : struct, Enum
{
	private static Comparison<T> CreateCompare(Type underlyingType)
		=> LambdaFactory.CreateComparison<T>((value1, value2) => value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), value2.Cast(underlyingType))).Compile();

	private static Func<T, T, bool> CreateEquals(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, T, bool>((value1, value2) => value1.Cast(underlyingType).Operation(BinaryOperator.EqualTo, value2.Cast(underlyingType))).Compile();

	private static Func<T, int> CreateGetHashCode(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, int>(value => value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();

	private readonly Lazy<TypeMember> _UnderlyingType;

	internal EnumMember() : base(typeof(T))
	{
		var type = typeof(T);
		var underlyingType = type.GetEnumUnderlyingType();
		var compare = CreateCompare(underlyingType);
		var equals = CreateEquals(underlyingType);
		var getHashCode = CreateGetHashCode(underlyingType);

		this._UnderlyingType = Lazy.Create(() => this.Handle.ToType().GetEnumUnderlyingType().GetTypeMember());
		this.Comparer = new CustomComparer<T>(compare, equals, getHashCode);
		this.Handle = type.TypeHandle;
		this.Flags = this.Attributes.Any<FlagsAttribute>();
		this.Tokens = type.GetFields(STATIC_BINDING_FLAGS).Map(fieldInfo => new TokenMember<T>(fieldInfo, this)).ToImmutableDictionary(_ => _.Value, Comparer);
	}

	public CustomComparer<T> Comparer { get; }

	public bool Flags { get; }

	public RuntimeTypeHandle Handle { get; }

	public IReadOnlyDictionary<T, TokenMember<T>> Tokens { get; }

	public TypeMember UnderlyingType => this._UnderlyingType.Value;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool IsValid(T value)
		=> this.Tokens.ContainsKey(value);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public string Parse(T value)
		=> this.Tokens.TryGetValue(value, out var token) ? token.Name : value.ToString("D");

	public T? Parse(string text, StringComparison comparison = STRING_COMPARISON)
		=> this.Tokens.Values.If(token => token.Name.Is(text, comparison)).TryFirst(out var token) ? (T?)token.Value : null;

	public bool TryParse(T value, out string text)
	{
		if (this.Tokens.TryGetValue(value, out var token))
		{
			text = token.Name;
			return true;
		}
		else
		{
			text = value.ToString("D");
			return false;
		}
	}

	public bool TryParse(string text, out T value, StringComparison comparison = STRING_COMPARISON)
	{
		if (this.Tokens.Values.TryFirst(token => token.Name.Is(text, comparison), out var token))
		{
			value = token.Value;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(EnumMember<T>? other)
		=> other is not null && this.Handle.Equals(other.Handle);
}
