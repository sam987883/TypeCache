// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.SqlApi;

[GraphQLDescription("Paged SQL response for an object of type `{0}`.")]
public class PageResponse<T>
{
	public string DataSource { get; set; } = string.Empty;

	public Connection<T>? Select { get; set; }

	public string Sql { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
