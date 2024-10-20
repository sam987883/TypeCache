// Copyright (c) 2021 Samuel Abraham

using static System.AttributeTargets;

namespace TypeCache.GraphQL.Attributes;

/// <summary>
/// <b>GraphQL</b><br/>
/// Ignore a parameter, field, property or type.
/// </summary>
[AttributeUsage(Class | Field | Parameter | Property | Struct, AllowMultiple = false, Inherited = false)]
public sealed class GraphQLIgnoreAttribute : Attribute
{
}
