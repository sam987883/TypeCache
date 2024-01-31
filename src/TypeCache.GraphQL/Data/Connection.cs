// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;
using static System.FormattableString;

namespace TypeCache.GraphQL.Data;

public class Connection<T>
	where T : notnull
{
	public Connection()
	{
	}

	public Connection(uint offset, T[] items)
	{
		offset += 1;
		this.Items = items;
		this.Edges = items.Select((row, i) => new Edge<T>((int)offset + i, row)).ToArray();
	}

	[GraphQLDescription("The total number of records available. Returns `null` if the total number is unknown.")]
	public int? TotalCount { get; init; }

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
			Resolver = new FuncFieldResolver<Connection<T>, Edge<T>[]?>(context => context.Source.Edges)
		});
		graphType.AddField(new()
		{
			Name = nameof(Connection<T>.Items),
			ResolvedType = new ListGraphType(new NonNullGraphType(dataGraphType)),
			Resolver = new FuncFieldResolver<Connection<T>, T[]?>(context => context.Source.Items)
		});
		graphType.AddField(new()
		{
			Name = nameof(Connection<T>.PageInfo),
			ResolvedType = new GraphQLObjectType<PageInfo>(Invariant($"{name}{nameof(PageInfo)}")),
			//Type = typeof(GraphQLObjectType<PageInfo>),
			Resolver = new FuncFieldResolver<Connection<T>, PageInfo?>(context => context.Source.PageInfo)
		});
		graphType.AddField(new()
		{
			Name = nameof(Connection<T>.TotalCount),
			Type = ScalarType.Int32.ToGraphType(),
			Resolver = new FuncFieldResolver<Connection<T>, int?>(context => context.Source.TotalCount)
		});

		return graphType;
	}
}
