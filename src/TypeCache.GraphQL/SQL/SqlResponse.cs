// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.GraphQL.SQL;

public class SqlResponse<T>
{
	public T[] Data { get; set; } = Array<T>.Empty;

	public string DataSource { get; set; } = string.Empty;

	public string Sql { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
