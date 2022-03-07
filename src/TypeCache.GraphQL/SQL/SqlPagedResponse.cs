// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.SQL;

public class SqlPagedResponse<T>
{
	public Connection<T>? Data { get; set; }

	public string DataSource { get; set; } = string.Empty;

	public string SQL { get; set; } = string.Empty;

	public string Table { get; set; } = string.Empty;
}
