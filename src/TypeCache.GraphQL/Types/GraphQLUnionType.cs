// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Collections.Extensions;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <see cref="GraphQLUnionType"/> works with 2 or more types.
/// </summary>
public sealed class GraphQLUnionType : UnionGraphType
{
	public GraphQLUnionType(Type[] types)
	{
		if (types.Length < 2)
			throw new ArgumentOutOfRangeException(nameof(GraphQLUnionType), $"2 or more types are required.");

		var graphObjectType = typeof(GraphQLObjectType<>);
		types.Do(type => this.Type(graphObjectType.MakeGenericType(type)));
	}
}
