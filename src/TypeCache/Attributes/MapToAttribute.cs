// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Attributes;

public abstract class MapToAttribute : Attribute
{
	public MapToAttribute(string member, Type type)
	{
		this.Member = member;
		this.Type = type;
	}

	public string Member { get; }

	public Type Type { get; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapToAttribute<T> : MapToAttribute
{
	public MapToAttribute(string member)
		: base(member, typeof(T))
	{
	}
}
