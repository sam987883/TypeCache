// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Data;

[GraphQLDescription("An edge in a connection that associates a cursor ID with data.")]
public readonly record struct Edge<T>(
	[GraphQLDescription("An identification number for use in pagination.")] long Cursor
	, [GraphQLDescription("The data item associated with the cursor value.")] T Node)
	where T : notnull
{
	public static ObjectGraphType CreateGraphType(string name, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}{nameof(Edge<T>)}"),
			Description = typeof(Edge<T>).GraphQLDescription()
		};

		graphType.AddField(new()
		{
			Name = nameof(Edge<T>.Cursor),
			Type = typeof(GraphQLNumberType<long>),
			Resolver = new CustomFieldResolver(context => ((Edge<T>)context.Source!).Cursor)
		});
		graphType.AddField(new()
		{
			Name = nameof(Edge<T>.Node),
			ResolvedType = new NonNullGraphType(dataGraphType),
			Resolver = new CustomFieldResolver(context => ((Edge<T>)context.Source!).Node)
		});

		return graphType;
	}
}
