// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Data;

public class Connection<T>
	where T : notnull
{
	public Connection()
	{
	}

	public Connection(long offset, T[] items)
	{
		++offset;
		this.Items = items;
		this.Edges = items.Select((row, i) => new Edge<T>(offset + i, row)).ToArray();
	}

	[GraphQLDescription("The total number of records available. Returns `null` if the total number is unknown.")]
	public long? TotalCount { get; init; }

	[GraphQLDescription("Pagination information for this result data set.")]
	public PageInfo? PageInfo { get; init; }

	[GraphQLDescription("The result data set, stored as a list of edges containing a node (the data) and a cursor (a unique identifier for the data).")]
	public Edge<T>[]? Edges { get; init; }

	[GraphQLDescription("The result data set.")]
	public T[]? Items { get; init; }

	public static ObjectGraphType CreateGraphType(string name, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}{nameof(Connection<T>)}"),
			Description = typeof(Connection<T>).GraphQLDescription()
		};
		var edgeGraphType = Edge<T>.CreateGraphType(name, dataGraphType);

		graphType.AddField(new()
		{
			Name = nameof(Connection<T>.Edges),
			ResolvedType = new ListGraphType(new NonNullGraphType(edgeGraphType)),
			Resolver = new CustomFieldResolver(context => ((Connection<T>)context.Source!).Edges)
		});
		graphType.AddField(new()
		{
			Name = nameof(Connection<T>.Items),
			ResolvedType = new ListGraphType(new NonNullGraphType(dataGraphType)),
			Resolver = new CustomFieldResolver(context => ((Connection<T>)context.Source!).Items)
		});
		graphType.AddField(new()
		{
			Name = nameof(Connection<T>.PageInfo),
			ResolvedType = new GraphQLObjectType<PageInfo>(Invariant($"{name}{nameof(PageInfo)}")),
			//Type = typeof(GraphQLObjectType<PageInfo>),
			Resolver = new CustomFieldResolver(context => ((Connection<T>)context.Source!).PageInfo)
		});
		graphType.AddField(new()
		{
			Name = nameof(Connection<T>.TotalCount),
			Type = typeof(GraphQLNumberType<long>),
			Resolver = new CustomFieldResolver(context => ((Connection<T>)context.Source!).TotalCount)
		});

		return graphType;
	}
}
