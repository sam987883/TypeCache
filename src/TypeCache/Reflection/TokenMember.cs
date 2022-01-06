// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

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

	public EnumMember<T> Type { get; }

	public T Value { get; }

	public bool Equals(TokenMember<T>? other)
		=> other is not null && this.Type.Comparer.Equals(this.Value, other.Value) && other.Name.Is(this.Name, NAME_STRING_COMPARISON);
}
