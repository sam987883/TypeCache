// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQLParser.AST;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLBooleanType : GraphQLScalarType<GraphQLBooleanValue>
{
	public GraphQLBooleanType() : base(
		nameof(Boolean),
		value => bool.TryParse(value.Value.Span, out _),
		value => bool.Parse(value.Value.Span),
		value => ValueConverter.ConvertToBoolean(value))
	{
	}
}
