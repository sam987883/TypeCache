// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("EnumOf<{Name,nq}>", Name = "{Name}")]
public sealed class EnumMember<T> : IMember, IEquatable<EnumMember<T>>
	where T : struct, Enum
{
	private static Comparison<T> CreateCompare(Type underlyingType)
		=> LambdaFactory.CreateComparison<T>((value1, value2) => value1.Cast(underlyingType).Call(nameof(IComparable<T>.CompareTo), value2.Convert(underlyingType))).Compile();

	private static Func<T, T, bool> CreateEquals(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, T, bool>((value1, value2) => value1.Cast(underlyingType).Operation(BinaryOperator.EqualTo, value2.Convert(underlyingType))).Compile();

	private static Func<T, int> CreateGetHashCode(Type underlyingType)
		=> LambdaFactory.CreateFunc<T, int>(value => value.Cast(underlyingType).Call(nameof(object.GetHashCode))).Compile();

	private readonly Lazy<TypeMember> _UnderlyingType;

	internal EnumMember()
	{
		const BindingFlags STATIC_BINDINGS = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static;

		var type = typeof(T);
		var underlyingType = type.GetEnumUnderlyingType();
		var compare = CreateCompare(underlyingType);
		var equals = CreateEquals(underlyingType);
		var getHashCode = CreateGetHashCode(underlyingType);

		this._UnderlyingType = Lazy.Create(() => this.TypeHandle.ToType().GetEnumUnderlyingType().GetTypeMember());
		this.Attributes = type.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Comparer = new CustomComparer<T>(compare, equals, getHashCode);
		this.Internal = !type.IsVisible;
		this.Name = type.Name();
		this.Public = type.IsPublic;
		this.TypeHandle = type.TypeHandle;
		this.Flags = this.Attributes.OfType<FlagsAttribute>().Any();
		this.Tokens = type.GetFields(STATIC_BINDINGS)
			.Where(fieldInfo => type.IsAssignableFrom(fieldInfo.FieldType))
			.Select(fieldInfo => new TokenMember<T>(fieldInfo, this))
			.ToArray();
	}

	public TokenMember<T>? this[T value]
		=> this.Tokens.FirstOrDefault(token => this.Comparer.EqualTo(token.Value, value));

	public TokenMember<T>? this[string text, StringComparison comparison = STRING_COMPARISON]
		=> this.Tokens.FirstOrDefault(token => token.Name.Is(text, comparison));

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	public CustomComparer<T> Comparer { get; }

	public bool Flags { get; }

	public RuntimeTypeHandle TypeHandle { get; }

	/// <inheritdoc cref="Type.IsVisible"/>
	public bool Internal { get; }

	/// <inheritdoc/>
	public string Name { get; }

	/// <inheritdoc cref="Type.IsPublic"/>
	public bool Public { get; }

	public IReadOnlyCollection<TokenMember<T>> Tokens { get; }

	/// <inheritdoc cref="Type.GetEnumUnderlyingType"/>
	public TypeMember UnderlyingType => this._UnderlyingType.Value;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool IsDefined(T value)
		=> this.Tokens.Any(token => this.Comparer.EqualTo(token.Value, value));

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public string Parse(T value)
		=> this[value]?.Name ?? value.ToString("G");

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] EnumMember<T>? other)
		=> this.TypeHandle == other?.TypeHandle;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as EnumMember<T>);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.TypeHandle.GetHashCode();
}
