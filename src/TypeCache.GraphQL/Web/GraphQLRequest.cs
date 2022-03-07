// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL.Web;

public class GraphQLRequest
{
	public string OperationName { get; set; } = string.Empty;

	public string Query { get; set; } = string.Empty;

	public object? Variables { get; set; }
}
