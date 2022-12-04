// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Attributes;

/// <summary>
/// Rename any cached <see cref="Member"/> within the TypeCache.Reflection system.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public sealed class NameAttribute : Attribute
{
	public NameAttribute(string name)
	{
		name.AssertNotBlank();
		this.Name = name;
	}

	/// <summary>
	/// The new name of the <see cref="Member"/>.
	/// </summary>
	public string Name { get; }
}
