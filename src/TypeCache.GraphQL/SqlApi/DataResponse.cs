// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

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

		graphType.AddField<string>(nameof(DataResponse.DataSource), new FuncFieldResolver<DataResponse, string>(context => context.Source.DataSource));
		graphType.AddField(nameof(DataResponse.Edges), new ListGraphType(new NonNullGraphType(edgeType)), new FuncFieldResolver<DataResponse, Edge<DataRow>[]?>(context => context.Source.Edges));
		graphType.AddField(nameof(DataResponse.Items), new ListGraphType(new NonNullGraphType(dataGraphType)), new FuncFieldResolver<DataResponse, IList<DataRow>?>(context => context.Source.Items!));
		graphType.AddField<PageInfo>(nameof(DataResponse.PageInfo), new FuncFieldResolver<DataResponse, PageInfo>(context => context.Source.PageInfo));
		graphType.AddField<string>(nameof(DataResponse.Sql), new FuncFieldResolver<DataResponse, string>(context => context.Source.Sql));
		graphType.AddField<string>(nameof(DataResponse.Table), new FuncFieldResolver<DataResponse, string>(context => context.Source.Table));
		graphType.AddField<int>(nameof(DataResponse.TotalCount), new FuncFieldResolver<DataResponse, long?>(context => context.Source.TotalCount));

		return graphType;
	}

	public static ObjectGraphType CreateEdgeGraphType(string name, string description, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}Edge"),
			Description = description
		};
		graphType.AddField<string>(nameof(Edge<DataRow>.Cursor), new FuncFieldResolver<Edge<DataRow>, string>(context => context.Source.Cursor));
		graphType.AddField(nameof(Edge<DataRow>.Node), new NonNullGraphType(dataGraphType), new FuncFieldResolver<Edge<DataRow>, DataRow>(context => context.Source.Node));
		return graphType;
	}
}
