// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Attributes;

public abstract class MapToAttribute : Attribute
{
	public MapToAttribute(Type type, string property)
	{
		this.Property = property;
		this.Type = type;
	}

	public string Property { get; }

	public Type Type { get; }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapToAttribute<T> : MapToAttribute
{
	public MapToAttribute(string property)
		: base(typeof(T), property)
	{
	}
}
