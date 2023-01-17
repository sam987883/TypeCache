﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("{EnumOf<T>.Name}.{Name,nq}", Name = "{Name}")]
public sealed class Token<T> : IEquatable<Token<T>>
	where T : struct, Enum
{
	internal Token(FieldInfo fieldInfo)
	{
		this.Attributes = fieldInfo.GetCustomAttributes<Attribute>().ToImmutableArray();
		this.Name = fieldInfo.Name();
		this.Value = (T)fieldInfo.GetFieldValue(null)!;
		this.Hex = this.Value.ToString("X");
		this.Number = this.Value.ToString("D");
	}

	/// <inheritdoc/>
	public IReadOnlyCollection<Attribute> Attributes { get; }

	/// <inheritdoc/>
	public string Name { get; }

	public string Hex { get; }

	public string Number { get; }

	public T Value { get; }

	public bool Equals([NotNullWhen(true)] Token<T>? other)
		=> other is not null && EnumOf<T>.Comparer.Equals(this.Value, other.Value);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as Token<T>);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> this.Value.GetHashCode();
}
