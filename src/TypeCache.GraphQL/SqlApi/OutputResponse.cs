// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.GraphQL.Attributes;
using static System.FormattableString;

namespace TypeCache.GraphQL.SqlApi;

[GraphQLDescription("Response for a DELETE/INSERT/UPDATE SQL API mutation action.")]
public class OutputResponse<T>
{
	public string? DataSource { get; set; }

	public IList<T>? Output { get; set; }

	public string? Sql { get; set; }

	public string? Table { get; set; }

	public long? TotalCount { get; set; }

	public static ObjectGraphType CreateGraphType(string name, string description, IGraphType dataGraphType)
	{
		var graphType = new ObjectGraphType
		{
			Name = Invariant($"{name}{nameof(OutputResponse<T>)}"),
			Description = description
		};

		graphType.AddField(new()
		{
			Name = nameof(OutputResponse<T>.DataSource),
			Type = typeof(StringGraphType),
			Resolver = new FuncFieldResolver<OutputResponse<T>, string?>(context => context.Source.DataSource)
		});
		graphType.AddField(new()
		{
			Name = nameof(OutputResponse<T>.Output),
			ResolvedType = new ListGraphType(new NonNullGraphType(dataGraphType)),
			Resolver = new FuncFieldResolver<OutputResponse<T>, IList<T>?>(context => context.Source.Output)
		});
		graphType.AddField(new()
		{
			Name = nameof(OutputResponse<T>.Sql),
			Type = typeof(StringGraphType),
			Resolver = new FuncFieldResolver<OutputResponse<T>, string?>(context => context.Source.Sql)
		});
		graphType.AddField(new()
		{
			Name = nameof(OutputResponse<T>.Table),
			Type = typeof(StringGraphType),
			Resolver = new FuncFieldResolver<OutputResponse<T>, string?>(context => context.Source.Table)
		});
		graphType.AddField(new()
		{
			Name = nameof(OutputResponse<T>.TotalCount),
			Type = typeof(LongGraphType),
			Resolver = new FuncFieldResolver<OutputResponse<T>, long?>(context => context.Source.TotalCount)
		});

		return graphType;
	}
}
