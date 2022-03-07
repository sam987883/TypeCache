// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL.SQL;

public class SqlCountResponse
{
	public long Count { get; set; }

	public string DataSource { get; set; } = string.Empty;

	public string SQL { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
