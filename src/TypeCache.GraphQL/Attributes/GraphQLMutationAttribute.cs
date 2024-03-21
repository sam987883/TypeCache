﻿// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br/>
/// Marks a method to be used as a root level Mutation endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class GraphQLMutationAttribute : Attribute
{
}
