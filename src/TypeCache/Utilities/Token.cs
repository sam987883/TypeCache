// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Utilities;

[DebuggerDisplay("{Token<T>}.{Name,nq}", Name = "{Name}")]
public sealed class Token<T> : IEquatable<Token<T>>
	where T : struct, Enum
{
	internal Token(T value)
	{
		var fieldInfo = typeof(T).GetField(value.ToString(), Public | Static)!;
		this.Attributes = fieldInfo.GetCustomAttributes<Attribute>().ToImmutableArray();
		this.Name = fieldInfo.Name();
		this.Value = value;
		this.Hex = value.ToString("X");
		this.Number = value.ToString("D");
	}

	public IReadOnlyCollection<Attribute> Attributes { get; }

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
