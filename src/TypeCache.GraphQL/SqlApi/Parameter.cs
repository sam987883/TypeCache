// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.SqlApi;

public class Parameter
{
	[GraphQLType<NonNullGraphType<GraphQLStringType>>()]
	public string Name { get; set; } = string.Empty;

	[GraphQLType<NonNullGraphType<GraphQLStringType>>()]
	public string Value { get; set; } = string.Empty;
}
