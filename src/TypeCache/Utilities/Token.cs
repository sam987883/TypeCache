// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Utilities;

[DebuggerDisplay("{Token<T>}.{Name,nq}", Name = "{Name}")]
public sealed class Token<T>
	where T : struct, Enum
{
	internal Token(T value)
	{
		var fieldInfo = typeof(T).GetField(Enum.GetName(value)!, Public | Static)!;
		var attributes = fieldInfo.GetCustomAttributes(false);
		this.Attributes = attributes.Length > 0 ? attributes.Cast<Attribute>().ToImmutableArray() : ImmutableArray<Attribute>.Empty;
		this.Name = fieldInfo.Name;
		this.Value = value;
	}

	public IReadOnlyList<Attribute> Attributes { get; }

	public string Name { get; }

	public T Value { get; }
}
