// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Attributes;

/// <summary>
/// Use this to override the property of a type where the current property gets mapped to.
/// </summary>
public abstract class MapAttribute : Attribute
{
	public MapAttribute(Type type, string member)
	{
		type.ThrowIfNull();
		member.ThrowIfBlank();

		if (type.Properties.TryGetValue(member, out var property))
			property.CanRead.ThrowIfFalse();
		else
			type.Fields.TryGetValue(member, out var field).ThrowIfFalse();

		this.Member = member;
		this.Type = type;
	}

	public string Member { get; }

	public Type Type { get; }
}

/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class MapAttribute<T>(string member) : MapAttribute(typeof(T), member)
{
}
