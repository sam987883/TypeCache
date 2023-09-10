// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQLParser.AST;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLUriType : GraphQLScalarType<GraphQLStringValue>
{
	public GraphQLUriType() : base(
		nameof(Uri),
		value => Uri.TryCreate(value.Value.Span.ToString(), value.Value.Span[0] is '/' ? UriKind.Relative : UriKind.Absolute, out _),
		value => new Uri(value.Value.Span.ToString(), value.Value.Span[0] is '/' ? UriKind.Relative : UriKind.Absolute),
		value => value is not null ? new Uri(value.ToString()!, value.ToString()![0] is '/' ? UriKind.Relative : UriKind.Absolute) : null)
	{
	}
}
