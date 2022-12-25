// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes;

public abstract class GraphQLSubqueryAttribute : Attribute
{
}

/// <summary>
/// <b>GraphQL</b><br/>
/// Marks a method to be used as a Subquery endpoint on the specified parent type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class GraphQLSubqueryAttribute<PARENT> : GraphQLSubqueryAttribute
{
}
