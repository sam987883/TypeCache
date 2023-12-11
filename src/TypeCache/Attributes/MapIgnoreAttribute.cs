// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class MapIgnoreAttribute() : Attribute
{
	protected MapIgnoreAttribute(Type type)
		: this()
	{
		type.AssertNotNull();

		this.Type = type;
	}

	public Type? Type { get; }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapIgnoreAttribute<T>() : MapIgnoreAttribute(typeof(T))
{
}
