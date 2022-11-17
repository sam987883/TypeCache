// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Types;
using static System.FormattableString;

namespace TypeCache.GraphQL.SqlApi;

[GraphQLDescription("Response for a SELECT SQL action.")]
public class SelectResponse<T>
{
	public SelectResponse()
	{
	}

	public SelectResponse(T[] items, int totalCount, uint offset)
	{
		var start = offset + 1;
		var end = start + items.Length;
		this.Edges = items.Map((item, i) => new Edge<T>
		{
			Cursor = (start + i).ToString(),
			Node = item
		});
		this.PageInfo = new()
		{
			StartCursor = start.ToString(),
			EndCursor = end.ToString(),
			HasNextPage = end < totalCount,
			HasPreviousPage = offset > 0
		};
		this.TotalCount = totalCount;
	}

	public string? DataSource { get; set; }

	public Edge<T>[]? Edges { get; set; }

	public IList<T>? Items { get; set; }

	public PageInfo? PageInfo { get; set; }

	public string? Sql { get; set; }

	public string? Table { get; set; }

	public long? TotalCount { get; set; }

	public static ObjectGraphType CreateGraphType(string name, string description, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}{nameof(SelectResponse<T>)}"),
			Description = description
		};
		var edgeType = CreateEdgeGraphType(name, description, dataGraphType);

		graphType.AddField(new()
		{
			Name = nameof(SelectResponse<T>.DataSource),
			Type = typeof(StringGraphType),
			Resolver = new FuncFieldResolver<SelectResponse<T>, string>(context => context.Source.DataSource)
		});
		graphType.AddField(new()
		{
			Name = nameof(SelectResponse<T>.Edges),
			ResolvedType = new ListGraphType(new NonNullGraphType(edgeType)),
			Resolver = new FuncFieldResolver<SelectResponse<T>, Edge<T>[]?>(context => context.Source.Edges)
		});
		graphType.AddField(new()
		{
			Name = nameof(SelectResponse<T>.Items),
			ResolvedType = new ListGraphType(new NonNullGraphType(dataGraphType)),
			Resolver = new FuncFieldResolver<SelectResponse<T>, IList<T>?>(context => context.Source.Items!)
		});
		graphType.AddField(new()
		{
			Name = nameof(SelectResponse<T>.PageInfo),
			Type = typeof(GraphQLObjectType<PageInfo>),
			Resolver = new FuncFieldResolver<SelectResponse<T>, PageInfo>(context => context.Source.PageInfo)
		});
		graphType.AddField(new()
		{
			Name = nameof(SelectResponse<T>.Sql),
			Type = typeof(StringGraphType),
			Resolver = new FuncFieldResolver<SelectResponse<T>, string>(context => context.Source.Sql)
		});
		graphType.AddField(new()
		{
			Name = nameof(SelectResponse<T>.Table),
			Type = typeof(StringGraphType),
			Resolver = new FuncFieldResolver<SelectResponse<T>, string>(context => context.Source.Table)
		});
		graphType.AddField(new()
		{
			Name = nameof(SelectResponse<T>.TotalCount),
			Type = typeof(IntGraphType),
			Resolver = new FuncFieldResolver<SelectResponse<T>, long?>(context => context.Source.TotalCount)
		});

		return graphType;
	}

	public static ObjectGraphType CreateEdgeGraphType(string name, string description, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}Edge"),
			Description = description
		};
		graphType.AddField(new()
		{
			Name = nameof(Edge<T>.Cursor),
			Type = typeof(StringGraphType),
			Resolver = new FuncFieldResolver<Edge<T>, string>(context => context.Source.Cursor)
		});
		graphType.AddField(new()
		{
			Name = nameof(Edge<T>.Node),
			ResolvedType = new NonNullGraphType(dataGraphType),
			Resolver = new FuncFieldResolver<Edge<T>, T>(context => context.Source.Node)
		});
		return graphType;
	}
}
