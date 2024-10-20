// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.SqlApi;

[GraphQLDescription("Response for a SELECT SQL action.")]
public class SelectResponse<T>
	where T : notnull
{
	public SelectResponse()
	{
	}

	public SelectResponse(T[] items, int totalCount, uint offset)
	{
		var start = offset + 1;
		this.Data = new(offset, items)
		{
			PageInfo = new(offset, (uint)items.Length, totalCount),
			TotalCount = totalCount
		};
	}

	public Connection<T>? Data { get; set; }

	public string? DataSource { get; set; }

	public string? Sql { get; set; }

	public string? Table { get; set; }

	public static ObjectGraphType CreateGraphType(string name, string description, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}{nameof(SelectResponse<T>)}"),
			Description = description
		};
		graphType.Fields.UnionWith(new FieldType[]
		{
			new()
			{
				Name = nameof(SelectResponse<T>.Data),
				ResolvedType = Connection<T>.CreateGraphType(name, dataGraphType),
				Resolver = new CustomFieldResolver(context => ((SelectResponse<T>)context.Source!).Data)
			},
			new()
			{
				Name = nameof(SelectResponse<T>.DataSource),
				Type = typeof(GraphQLStringType),
				Resolver = new CustomFieldResolver(context => ((SelectResponse<T>)context.Source!).DataSource)
			},
			new()
			{
				Name = nameof(SelectResponse<T>.Sql),
				Type = typeof(GraphQLStringType),
				Resolver = new CustomFieldResolver(context => ((SelectResponse<T>)context.Source!).Sql)
			},
			new()
			{
				Name = nameof(SelectResponse<T>.Table),
				Type = typeof(GraphQLStringType),
				Resolver = new CustomFieldResolver(context => ((SelectResponse<T>)context.Source!).Table)
			},
		});

		return graphType;
	}
}
