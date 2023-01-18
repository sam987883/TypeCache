﻿// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
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
	/// Registers:
	/// <list type="bullet">
	/// <item><term><see cref="IDocumentExecuter"/></term> A singleton instance of: <see cref="DocumentExecuter"/>.</item>
	/// <item><term><see cref="IGraphQLSerializer"/></term> A singleton instance of: <see cref="GraphQLSerializer"/>.</item>
	/// <item><term><see cref="IDataLoaderContextAccessor"/></term> A singleton instance of: <see cref="DataLoaderContextAccessor"/>.</item>
	/// <item><term><see cref="IDocumentExecutionListener"/></term> A singleton instance of: <see cref="DataLoaderDocumentListener"/>.</item>
	/// <item><term><see cref="GraphQLEnumType{T}"/></term> The <see cref="EnumerationGraphType{TEnum}"/>.</item>
	/// <item><term><see cref="GraphQLHashIdType"/></term> A <see cref="ScalarGraphType"/> that hashes and unhashes integer identifier types to prevent a sequential attack.</item>
	/// <item><term><see cref="GraphQLInputType{T}"/></term> The GraphQL InputObjectGraphType.</item>
	/// <item><term><see cref="GraphQLObjectType{T}"/></term> The GraphQL ObjectGraphType.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection AddGraphQL(this IServiceCollection @this, JsonSerializerOptions? jsonOptions = null)
		=> @this.AddSingleton<IDocumentExecuter, DocumentExecuter>()
			.AddSingleton<IGraphQLSerializer>(provider => jsonOptions is not null ? new GraphQLSerializer(jsonOptions) : new GraphQLSerializer())
			.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
			.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>()
			.AddTransient(typeof(GraphQLEnumType<>))
			.AddTransient<GraphQLHashIdType>()
			.AddTransient(typeof(GraphQLInputType<>))
			.AddTransient(typeof(GraphQLObjectType<>));
}
