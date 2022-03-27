// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.GraphQL.SQL;

public class SqlUpdateResponse<T>
{
	public T[] Deleted { get; set; } = Array<T>.Empty;

	public T[] Inserted { get; set; } = Array<T>.Empty;

	public long Count { get; set; }

	public string DataSource { get; set; } = string.Empty;

	public string Sql { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
