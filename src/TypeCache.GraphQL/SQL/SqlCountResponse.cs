// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL.SQL;

public class SqlCountResponse
{
	public long Count { get; set; }

	public string? SQL { get; set; }

	public string? Table { get; set; }
}
