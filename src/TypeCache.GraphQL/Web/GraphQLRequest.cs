// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;

namespace TypeCache.GraphQL.Web;

public sealed class GraphQLRequest
{
	public string? OperationName { get; set; }

	public string? Query { get; set; }

	public IDictionary<string, object>? Variables { get; set; }

	public IDictionary<string, object>? Extensions { get; set; }
}
