// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.SqlApi;

[GraphQLDescription("SQL response for an object of type `{0}`.")]
public class DeleteResponse<T>
{
	public long Count { get; set; }

	public string DataSource { get; set; } = string.Empty;

	public T[] Deleted { get; set; } = Array<T>.Empty;

	public string Sql { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
