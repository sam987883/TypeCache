// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type}.{Name,nq}", Name = "{Name}")]
public class TokenMember<T> : Member, IEquatable<TokenMember<T>>
	where T : struct, Enum
{
	internal TokenMember(FieldInfo fieldInfo, EnumMember<T> type) : base(fieldInfo)
	{
		this.Type = type;
		this.Value = (T)Enum.Parse<T>(fieldInfo.Name);
		this.Hex = this.Value.ToString("X");
		this.Number = this.Value.ToString("D");
	}

	public string Hex { get; }

	public string Number { get; }

	public new EnumMember<T> Type { get; }

	public T Value { get; }

	public bool Equals([NotNullWhen(true)] TokenMember<T>? other)
		=> other is not null && this.Type.Comparer.Equals(this.Value, other.Value) && other.Name.Is(this.Name, NAME_STRING_COMPARISON);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as TokenMember<T>);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Value.GetHashCode();
}
