// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br />
/// Marks a method to be used as a Subquery endpoint in the specified parent type.<br />
/// Establishes a parent-child relationship based on key which is matched against the GraphKey of the parent and child models.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class GraphQLSubqueryBatchAttribute : Attribute
{
	public GraphQLSubqueryBatchAttribute(Type parentType, string key)
	{
		this.ParentType = parentType;
		this.Key = key;
	}

	public Type ParentType { get; }

	public string Key { get; }
}
