// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Attributes;

/// <summary>
/// Use this to override the property of a type where the current property gets mapped to.
/// </summary>
public abstract class MapAttribute : Attribute
{
	public MapAttribute(Type type, string property)
	{
		var propertyInfo = type.GetPropertyInfo(property);
		propertyInfo.ThrowIfNull();
		propertyInfo.CanWrite.ThrowIfFalse();

		this.Property = property;
		this.Type = type;
	}

	public string Property { get; }

	public Type Type { get; }
}

/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapAttribute<T>(string property) : MapAttribute(typeof(T), property)
{
}
