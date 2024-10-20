// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Resolvers;

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
		graphType.Fields.UnionWith(new FieldType[]
		{
			new()
			{
				Name = nameof(OutputResponse<T>.DataSource),
				Type = typeof(string),
				Resolver = new CustomFieldResolver(context => ((OutputResponse<T>)context.Source!).DataSource)
			},
			new()
			{
				Name = nameof(OutputResponse<T>.Output),
				ResolvedType = new ListGraphType(new NonNullGraphType(dataGraphType)),
				Resolver = new CustomFieldResolver(context => ((OutputResponse<T>)context.Source!).Output)
			},
			new()
			{
				Name = nameof(OutputResponse<T>.Sql),
				Type = typeof(string),
				Resolver = new CustomFieldResolver(context => ((OutputResponse<T>)context.Source!).Sql)
			},
			new()
			{
				Name = nameof(OutputResponse<T>.Table),
				Type = typeof(string),
				Resolver = new CustomFieldResolver(context => ((OutputResponse<T>)context.Source!).Table)
			},
			new()
			{
				Name = nameof(OutputResponse<T>.TotalCount),
				Type = typeof(long),
				Resolver = new CustomFieldResolver(context => ((OutputResponse<T>)context.Source!).TotalCount)
			},
		});

		return graphType;
	}
}
