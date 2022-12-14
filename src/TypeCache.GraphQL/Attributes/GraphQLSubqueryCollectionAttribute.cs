// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br/>
/// Marks a method to be used as a Subquery endpoint on the specified parent type returning a collection.<br />
/// Establishes a parent-child relationship based on key which is matched against the GraphKey of the parent and child models.
/// </summary>
/// <remarks>The tagged endpoint must return an IEnumerable&lt;&gt; type.</remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class GraphQLSubqueryCollectionAttribute : Attribute
{
	public GraphQLSubqueryCollectionAttribute(Type parentType, string key)
	{
		this.ParentType = parentType;
		this.Key = key;
	}

	public Type ParentType { get; }

	public string Key { get; }
}
