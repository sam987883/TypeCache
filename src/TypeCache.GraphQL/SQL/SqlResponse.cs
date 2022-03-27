// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.SQL;

[GraphQLDescription("SQL response for an object of type `{0}`.")]
public class SqlResponse<T>
{
	public long Count { get; set; }

	public T[] Data { get; set; } = Array<T>.Empty;

	public string DataSource { get; set; } = string.Empty;

	public string Sql { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
