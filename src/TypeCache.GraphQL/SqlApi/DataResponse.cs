// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using System.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;
using static System.FormattableString;

namespace TypeCache.GraphQL.SqlApi;

[GraphQLDescription("Response for a SELECT SQL action.")]
public class DataResponse
{
	public DataResponse()
	{
	}

	public DataResponse(DataRow[] items, int totalCount, uint offset)
	{
		var start = offset + 1;
		var end = start + items.Length;
		this.Edges = items.Select((item, i) => new Edge<DataRow>
		{
			Cursor = (start + i).ToString(),
			Node = item
		}).ToArray();
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

	public Edge<DataRow>[]? Edges { get; set; }

	public IList<DataRow>? Items { get; set; }

	public PageInfo? PageInfo { get; set; }

	public string? Sql { get; set; }

	public string? Table { get; set; }

	public long? TotalCount { get; set; }

	public static ObjectGraphType CreateGraphType(string name, string description, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}{nameof(DataResponse)}"),
			Description = description
		};
		var edgeType = CreateEdgeGraphType(name, description, dataGraphType);

		graphType.AddField(new()
		{
			Name = nameof(DataResponse.DataSource),
			Type = typeof(GraphQLScalarType<string>),
			Resolver = new FuncFieldResolver<DataResponse, string>(context => context.Source.DataSource)
		});
		graphType.AddField(new()
		{
			Name = nameof(DataResponse.Edges),
			ResolvedType = new ListGraphType(new NonNullGraphType(edgeType)),
			Resolver = new FuncFieldResolver<DataResponse, Edge<DataRow>[]?>(context => context.Source.Edges)
		});
		graphType.AddField(new()
		{
			Name = nameof(DataResponse.Items),
			ResolvedType = new ListGraphType(new NonNullGraphType(dataGraphType)),
			Resolver = new FuncFieldResolver<DataResponse, IList<DataRow>?>(context => context.Source.Items!)
		});
		graphType.AddField(new()
		{
			Name = nameof(DataResponse.PageInfo),
			Type = typeof(GraphQLObjectType<PageInfo>),
			Resolver = new FuncFieldResolver<DataResponse, PageInfo>(context => context.Source.PageInfo)
		});
		graphType.AddField(new()
		{
			Name = nameof(DataResponse.Sql),
			Type = typeof(GraphQLScalarType<string>),
			Resolver = new FuncFieldResolver<DataResponse, string>(context => context.Source.Sql)
		});
		graphType.AddField(new()
		{
			Name = nameof(DataResponse.Table),
			Type = typeof(GraphQLScalarType<string>),
			Resolver = new FuncFieldResolver<DataResponse, string>(context => context.Source.Table)
		});
		graphType.AddField(new()
		{
			Name = nameof(DataResponse.TotalCount),
			Type = ScalarType.Int32.ToGraphType(),
			Resolver = new FuncFieldResolver<DataResponse, long?>(context => context.Source.TotalCount)
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
			Name = nameof(Edge<DataRow>.Cursor),
			Type = typeof(GraphQLScalarType<string>),
			Resolver = new FuncFieldResolver<Edge<DataRow>, string>(context => context.Source.Cursor)
		});
		graphType.AddField(new()
		{
			Name = nameof(Edge<DataRow>.Node),
			ResolvedType = new NonNullGraphType(dataGraphType),
			Resolver = new FuncFieldResolver<Edge<DataRow>, DataRow>(context => context.Source.Node)
		});
		return graphType;
	}
}
