// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class MapIgnoreAttribute : Attribute
{
	public MapIgnoreAttribute()
	{
		this.Type = null;
	}

	protected MapIgnoreAttribute(Type type)
	{
		this.Type = type;
	}

	public Type? Type { get; }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class MapIgnoreAttribute<T> : MapIgnoreAttribute
{
	public MapIgnoreAttribute(string property)
		: base(typeof(T))
	{
	}
}
