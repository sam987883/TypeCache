// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br/>
/// Sets the name of the object type, object property, enum type, enum field, endpoint or endpoint parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Struct)]
public class GraphNameAttribute : Attribute
{
	/// <exception cref="ArgumentNullException"/>
	public GraphNameAttribute(string name)
	{
		name.AssertNotBlank();

		this.Name = name;
	}

	public string Name { get; }
}
