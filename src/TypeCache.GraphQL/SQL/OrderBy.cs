// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.SQL;

public class OrderBy<T>
	where T : class
{
	[GraphQLType(ScalarType.NotNullString)]
	public string Expression { get; set; } = string.Empty;

	public Sort Sort { get; set; }
}
