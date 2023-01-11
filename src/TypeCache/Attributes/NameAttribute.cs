// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Attributes;

/// <summary>
/// Rename any <see cref="MemberInfo"/> objects used within the TypeCache system.<br/>
/// To access the new name use <c><see cref="MemberInfo"/>.Name()</c> instead of <c><see cref="MemberInfo.Name"/></c>.
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
	/// The new name of the <see cref="MemberInfo"/>.
	/// To access the new name use <c><see cref="MemberInfo"/>.Name()</c> instead of <c><see cref="MemberInfo.Name"/></c>.
	/// </summary>
	public string Name { get; }
}
