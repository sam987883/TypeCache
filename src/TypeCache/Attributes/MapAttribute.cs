// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Attributes;

public abstract class MapAttribute(Type type, string property) : Attribute
{
	public string Property { get; } = property;

	public Type Type { get; } = type;
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapAttribute<T>(string property) : MapAttribute(typeof(T), property)
{
}
