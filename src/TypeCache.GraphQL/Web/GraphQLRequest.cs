// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;

namespace TypeCache.GraphQL.Web;

public class GraphQLRequest
{
	public string OperationName { get; set; } = string.Empty;

	public string Query { get; set; } = string.Empty;

	public IDictionary<string, object?>? Variables { get; set; }
}
