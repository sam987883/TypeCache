// Copyright (c) 2021 Samuel Abraham

using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.SQL;

public class Parameter
{
	[GraphQLType(ScalarType.NotNullString)]
	public string Name { get; set; } = string.Empty;

	[GraphQLType(ScalarType.NotNullString)]
	public string Value { get; set; } = string.Empty;
}
