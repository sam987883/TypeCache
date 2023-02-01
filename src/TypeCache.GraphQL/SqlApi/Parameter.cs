// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.SqlApi;

public class Parameter
{
	[GraphQLType(typeof(NonNullGraphType<StringGraphType>))]
	public string Name { get; set; } = string.Empty;

	[GraphQLType(typeof(NonNullGraphType<StringGraphType>))]
	public string Value { get; set; } = string.Empty;
}
