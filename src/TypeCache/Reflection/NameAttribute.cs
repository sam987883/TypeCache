﻿// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

/// <summary>
/// Rename any cached <see cref="IMember"/> within the TypeCache system.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public class NameAttribute : Attribute
{
	public NameAttribute(string name)
	{
		name.AssertNotBlank();
		this.Name = name;
	}

	public string Name { get; }
}
