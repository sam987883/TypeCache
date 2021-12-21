// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br />
/// Marks a property with a key to be used in a Subquery endpoint in a parent-child relationship.
/// </summary>
/// <remarks>
/// The GraphKey name in the parent and child models must match to form the relationship.<br />
/// The GraphKey name must be unique within a model though a model may contain multiple GraphKeys.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class GraphKeyAttribute : Attribute
{
	public GraphKeyAttribute(string name)
	{
		this.Name = name;
	}

	public string Name { get; }
}
