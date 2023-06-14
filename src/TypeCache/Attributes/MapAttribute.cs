// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Attributes;

public abstract class MapAttribute : Attribute
{
	public MapAttribute(Type type, string property)
	{
		this.Property = property;
		this.Type = type;
	}

	public string Property { get; }

	public Type Type { get; }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapAttribute<T> : MapAttribute
{
	public MapAttribute(string property)
		: base(typeof(T), property)
	{
	}
}
