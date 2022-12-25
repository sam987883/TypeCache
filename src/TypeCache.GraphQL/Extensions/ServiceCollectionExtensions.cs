// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><see cref="IDocumentExecuter"/></term> An instance of: <see cref="DocumentExecuter"/>.</item>
	/// <item><term><see cref="IGraphQLSerializer"/></term> An instance of: <see cref="GraphQLSerializer"/>.</item>
	/// <item><term><see cref="IDataLoaderContextAccessor"/></term> An instance of: <see cref="DataLoaderContextAccessor"/>.</item>
	/// <item><term><see cref="IDocumentExecutionListener"/></term> An instance of: <see cref="DataLoaderDocumentListener"/>.</item>
	/// <item><term><see cref="GraphQLEnumType{T}"/></term> The GraphQL EnumerationGraphType.</item>
	/// <item><term><see cref="GraphQLHashIdType"/></term> A <see cref="ScalarGraphType"/> that hashes and unhashes integer identifier types to prevent a sequential attack.</item>
	/// <item><term><see cref="GraphQLInputType{T}"/></term> The GraphQL InputObjectGraphType.</item>
	/// <item><term><see cref="GraphQLObjectType{T}"/></term> The GraphQL ObjectGraphType.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection AddGraphQL(this IServiceCollection @this)
		=> @this.AddSingleton<IDocumentExecuter, DocumentExecuter>()
			.AddSingleton<IGraphQLSerializer, GraphQLSerializer>()
			.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
			.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>()
			.AddSingleton(typeof(GraphQLEnumType<>))
			.AddSingleton<GraphQLHashIdType>()
			.AddSingleton(typeof(GraphQLInputType<>))
			.AddSingleton(typeof(GraphQLObjectType<>));
}
