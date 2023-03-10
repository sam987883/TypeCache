// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapToAttribute : Attribute
{
	public MapToAttribute(Type type, string property)
	{
		this.Property = property;
		this.Type = type;
	}

	public string Property { get; }

	public Type Type { get; }
}
