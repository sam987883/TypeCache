// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br/>
/// Sets the name of the input type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class GraphQLInputNameAttribute : Attribute
{
	/// <exception cref="ArgumentNullException"/>
	public GraphQLInputNameAttribute(string name)
	{
		name.ThrowIfBlank();

		this.Name = name;
	}

	public string Name { get; }
}
