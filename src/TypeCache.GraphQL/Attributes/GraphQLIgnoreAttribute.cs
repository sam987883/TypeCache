// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br/>
/// Ignore a parameter, enum field or property.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class GraphQLIgnoreAttribute : Attribute
{
}
