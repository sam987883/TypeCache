// Copyright (c) 2021 Samuel Abraham

using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Data;

[GraphQLDescription("An edge in a connection that associates a cursor ID with data.")]
public readonly record struct Edge<T>(
	[GraphQLDescription("An identification number for use in pagination.")] int Cursor
	, [GraphQLDescription("The data item associated with the cursor value.")] T Node)
	where T : notnull
{
	public static ObjectGraphType CreateGraphType(string name, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}{nameof(Edge<>)}"),
			Description = Type<Edge<T>>.Attributes.GraphQLDescription
		};

		graphType.AddField(new()
		{
			Name = nameof(Edge<>.Cursor),
			Type = ScalarType.Int32.ToGraphType(),
			Resolver = new FuncFieldResolver<Edge<T>, int>(context => context.Source.Cursor)
		});
		graphType.AddField(new()
		{
			Name = nameof(Edge<>.Node),
			ResolvedType = new NonNullGraphType(dataGraphType),
			Resolver = new FuncFieldResolver<Edge<T>, T>(context => context.Source.Node)
		});

		return graphType;
	}
}
