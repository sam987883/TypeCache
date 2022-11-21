// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type}.{Name,nq}", Name = "{Name}")]
public sealed class TokenMember<T> : IMember, IEquatable<TokenMember<T>>
	where T : struct, Enum
{
	internal TokenMember(FieldInfo fieldInfo, EnumMember<T> type)
	{
		this.Attributes = fieldInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Internal = fieldInfo.IsAssembly;
		this.Name = fieldInfo.Name();
		this.Public = fieldInfo.IsPublic;
		this.Type = type;
		this.Value = (T)Enum.Parse<T>(fieldInfo.Name);
		this.Hex = this.Value.ToString("X");
		this.Number = this.Value.ToString("D");
	}

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	/// <inheritdoc cref="FieldInfo.IsAssembly"/>
	public bool Internal { get; }

	/// <inheritdoc/>
	public string Name { get; }

	/// <inheritdoc cref="FieldInfo.IsPublic"/>
	public bool Public { get; }

	public string Hex { get; }

	public string Number { get; }

	public EnumMember<T> Type { get; }

	public T Value { get; }

	public bool Equals([NotNullWhen(true)] TokenMember<T>? other)
		=> other is not null && this.Type.Comparer.Equals(this.Value, other.Value) && other.Name.Is(this.Name, StringComparison.Ordinal);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as TokenMember<T>);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Value.GetHashCode();
}
