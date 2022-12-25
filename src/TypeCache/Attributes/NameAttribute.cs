// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Attributes;

/// <summary>
/// Rename any cached <see cref="IMember"/> within the TypeCache.Reflection system.
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
	/// The new name of the <see cref="IMember"/>.
	/// </summary>
	public string Name { get; }
}
