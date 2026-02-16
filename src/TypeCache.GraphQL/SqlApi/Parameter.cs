// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.SqlApi;

public class Parameter
{
	[GraphQLType<NonNullGraphType<StringGraphType>>()]
	public string Name { get; set; } = string.Empty;

	[GraphQLType<StringGraphType>()]
	public string? Value { get; set; }
}
