// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;

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
		types.ForEach(type => this.Type(graphObjectType.MakeGenericType(type)));
	}
}
