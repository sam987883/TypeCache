﻿// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Attributes;

/// <summary>
/// Rename any cached <see cref="Member"/> within the TypeCache system.
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