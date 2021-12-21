// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data;

namespace TypeCache.GraphQL.SQL;

public class OrderBy<T>
	where T : class
{
	public string Expression { get; set; } = string.Empty;

	public Sort Sort { get; set; }
}
