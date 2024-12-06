// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Attributes;

public abstract class MapAttribute : Attribute
{
	public MapAttribute(Type type, string property)
	{
		var propertyInfo = type.GetPropertyInfo(property, true);
		propertyInfo.ThrowIfNull();
		propertyInfo.CanWrite.ThrowIfFalse();
		propertyInfo.SetMethod?.IsStatic.ThrowIfTrue();

		this.Property = property;
		this.Type = type;
	}

	public string Property { get; }

	public Type Type { get; }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapAttribute<T>(string property) : MapAttribute(typeof(T), property)
{
}
