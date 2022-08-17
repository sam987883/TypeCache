// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
		=> LambdaFactory.CreateComparison<T>((value1, value2) => value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), value2.Convert(underlyingType))).Compile();

	private static Func<T, T, bool> CreateEquals(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, T, bool>((value1, value2) => value1.Cast(underlyingType).Operation(BinaryOperator.EqualTo, value2.Convert(underlyingType))).Compile();

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
		this.Tokens = type.GetFields(STATIC_BINDING_FLAGS)
			.Map(fieldInfo => new TokenMember<T>(fieldInfo, this))
			.ToArray();
	}

	public TokenMember<T>? this[T value]
		=> this.Tokens.First(token => this.Comparer.EqualTo(token.Value, value));

	public TokenMember<T>? this[string text, StringComparison comparison = STRING_COMPARISON]
		=> this.Tokens.First(token => token.Name.Is(text, comparison));

	public CustomComparer<T> Comparer { get; }

	public bool Flags { get; }

	public RuntimeTypeHandle Handle { get; }

	public IReadOnlyCollection<TokenMember<T>> Tokens { get; }

	public TypeMember UnderlyingType => this._UnderlyingType.Value;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool IsDefined(T value)
		=> this.Tokens.Any(token => this.Comparer.EqualTo(token.Value, value));

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public string Parse(T value)
		=> this[value]?.Name ?? value.ToString("G");

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] EnumMember<T>? other)
		=> other?.Handle.Equals(this.Handle) is true;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as EnumMember<T>);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();
}
