// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Attributes;

/// <summary>
/// Do not map this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class MapIgnoreAttribute() : Attribute
{
	protected MapIgnoreAttribute(Type type)
		: this()
	{
		type.ThrowIfNull();

		this.Type = type;
	}

	public Type? Type { get; }
}

/// <summary>
/// Do not map this property to type <typeparamref name="T"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapIgnoreAttribute<T>() : MapIgnoreAttribute(typeof(T))
{
}
