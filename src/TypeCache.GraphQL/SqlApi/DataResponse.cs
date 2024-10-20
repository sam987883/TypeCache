// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Resolvers;

namespace TypeCache.GraphQL.SqlApi;

[GraphQLDescription("Response for a SELECT SQL action.")]
public class DataResponse
{
	public DataResponse()
	{
	}

	public DataResponse(DataRow[] items, long totalCount, long offset)
	{
		var start = offset + 1;
		var end = start + items.Length;
		this.Edges = items.Select((item, i) => new Edge<DataRow>
		{
			Cursor = start + i,
			Node = item
		}).ToArray();
		this.PageInfo = new()
		{
			StartCursor = start,
			EndCursor = end,
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

		graphType.Fields.UnionWith(new FieldType[]
		{
			new()
			{
				Name = nameof(DataResponse.DataSource),
				Type = typeof(string),
				Resolver = new CustomFieldResolver(context => ((DataResponse)context.Source!).DataSource)
			},
			new()
			{
				Name = nameof(DataResponse.Edges),
				ResolvedType = new ListGraphType(new NonNullGraphType(edgeType)),
				Resolver = new CustomFieldResolver(context => ((DataResponse)context.Source!).Edges)
			},
			new()
			{
				Name = nameof(DataResponse.Items),
				ResolvedType = new ListGraphType(new NonNullGraphType(dataGraphType)),
				Resolver = new CustomFieldResolver(context => ((DataResponse)context.Source!).Items)
			},
			new()
			{
				Name = nameof(DataResponse.PageInfo),
				Type = typeof(PageInfo),
				Resolver = new CustomFieldResolver(context => ((DataResponse)context.Source!).PageInfo)
			},
			new()
			{
				Name = nameof(DataResponse.Sql),
				Type = typeof(string),
				Resolver = new CustomFieldResolver(context => ((DataResponse)context.Source!).Sql)
			},
			new()
			{
				Name = nameof(DataResponse.Table),
				Type = typeof(string),
				Resolver = new CustomFieldResolver(context => ((DataResponse)context.Source!).Table)
			},
			new()
			{
				Name = nameof(DataResponse.TotalCount),
				Type = typeof(int),
				Resolver = new CustomFieldResolver(context => ((DataResponse)context.Source!).TotalCount)
			}
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
		graphType.Fields.UnionWith(new FieldType[]
		{
			new()
			{
				Name = nameof(Edge<DataRow>.Cursor),
				Type = typeof(long),
				Resolver = new CustomFieldResolver(context => ((Edge<DataRow>)context.Source!).Cursor)
			},
			new()
			{
				Name = nameof(Edge<DataRow>.Node),
				ResolvedType = new NonNullGraphType(dataGraphType),
				Resolver = new CustomFieldResolver(context => ((Edge<DataRow>)context.Source!).Node)
			},
		});

		return graphType;
	}
}
