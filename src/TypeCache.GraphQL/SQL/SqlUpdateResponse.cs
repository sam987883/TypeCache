// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.GraphQL.SQL;

public class SqlUpdateResponse<T>
{
	public T[]? Deleted { get; set; }

	public T[]? Inserted { get; set; }

	public string? SQL { get; set; }

	public string? Table { get; set; }
}
