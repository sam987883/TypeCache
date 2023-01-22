// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.GraphQL.Converters;
using TypeCache.GraphQL.Types;
using TypeCache.GraphQL.Web;

namespace TypeCache.GraphQL.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers singletons for: <see cref="IDocumentExecuter"/>, <see cref="IGraphQLSerializer"/>, <see cref="IDataLoaderContextAccessor"/> and <see cref="IDocumentExecutionListener"/>.<br/>
	/// Also registers transients: <see cref="GraphQLEnumType{T}"/>, <see cref="GraphQLHashIdType"/>, <see cref="GraphQLInputType{T}"/> and <see cref="GraphQLObjectType{T}"/>.
	/// </summary>
	/// <remarks>
	/// You can override <b><see cref="IGraphQLSerializer"/></b> by registering a different implemntation before this call.<br/>
	/// Other implementations for <b><see cref="IGraphQLSerializer"/></b> can be found at:
	/// <list type="bullet">
	/// <item><see href="https://github.com/graphql-dotnet/graphql-dotnet/pkgs/nuget/GraphQL.SystemTextJson"/></item>
	/// <item><see href="https://github.com/graphql-dotnet/graphql-dotnet/pkgs/nuget/GraphQL.NewtonsoftJson"/></item>
	/// </list>
	/// To limit the information exposed in <see cref="ExecutionError"/>, register a JsonConverter&lt;<see cref="ExecutionError"/>&gt; that writes out only what is desired.
	/// </remarks>
	public static IServiceCollection AddGraphQL(this IServiceCollection @this)
	{
		@this.TryAddSingleton<IGraphQLSerializer, GraphQLJsonSerializer>();
		@this.TryAddSingleton<JsonConverter<ExecutionError>, GraphQLExecutionErrorJsonConverter>();
		return @this.AddSingleton<IDocumentExecuter, DocumentExecuter>()
			.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
			.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>()
			.AddTransient(typeof(GraphQLEnumType<>))
			.AddTransient<GraphQLHashIdType>()
			.AddTransient(typeof(GraphQLInputType<>))
			.AddTransient(typeof(GraphQLObjectType<>));
	}
}
