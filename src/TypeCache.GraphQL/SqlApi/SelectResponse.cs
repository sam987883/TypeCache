// Copyright (c) 2021 Samuel Abraham

using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Extensions;
using static System.FormattableString;

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

		graphType.AddField(nameof(SelectResponse<T>.Data), Connection<T>.CreateGraphType(name, dataGraphType), new FuncFieldResolver<SelectResponse<T>, Connection<T>>(context => context.Source.Data));
		graphType.AddField<string>(nameof(SelectResponse<T>.DataSource), new FuncFieldResolver<SelectResponse<T>, string>(context => context.Source.DataSource));
		graphType.AddField<string>(nameof(SelectResponse<T>.Sql), new FuncFieldResolver<SelectResponse<T>, string>(context => context.Source.Sql));
		graphType.AddField<string>(nameof(SelectResponse<T>.Table), new FuncFieldResolver<SelectResponse<T>, string>(context => context.Source.Table));

		return graphType;
	}
}
