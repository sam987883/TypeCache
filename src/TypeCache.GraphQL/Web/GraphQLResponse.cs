// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;

namespace TypeCache.GraphQL.Web;

public class GraphQLResponse
{
	public object? Data { set; get; }

	public GraphQLError[]? Errors { get; set; }

	public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
}
