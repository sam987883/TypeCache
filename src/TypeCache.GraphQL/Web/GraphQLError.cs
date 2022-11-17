// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Generic;
using GraphQL.Execution;

namespace TypeCache.GraphQL.Web;

public sealed class GraphQLError
{
	public string? Message { set; get; }

	public IEnumerable<DocumentError>? Locations { get; set; }

	public IEnumerable<object>? Path { get; set; }

	public IDictionary<string, object?>? Extensions { get; set; }

	public IDictionary? Data { get; set; }
}
