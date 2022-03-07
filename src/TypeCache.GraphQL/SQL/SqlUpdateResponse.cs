// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.GraphQL.SQL;

public class SqlUpdateResponse<T>
{
	public T[] Deleted { get; set; } = Array<T>.Empty;

	public T[] Inserted { get; set; } = Array<T>.Empty;

	public string DataSource { get; set; } = string.Empty;

	public string SQL { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
