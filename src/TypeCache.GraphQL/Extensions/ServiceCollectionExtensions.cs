// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Converters;
using TypeCache.GraphQL.Resolvers;
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
	/// To limit the information exposed in <see cref="ExecutionError"/>, register a <c>JsonConverter&lt;<see cref="ExecutionError"/>&gt;</c> that writes out only what is desired.<br/><br/>
	/// You can override <b><see cref="IGraphQLSerializer"/></b> by registering a different implemntation before this call.<br/>
	/// Other implementations for <b><see cref="IGraphQLSerializer"/></b> can be found at:
	/// <list type="bullet">
	/// <item><see href="https://github.com/graphql-dotnet/graphql-dotnet/pkgs/nuget/GraphQL.SystemTextJson"/></item>
	/// <item><see href="https://github.com/graphql-dotnet/graphql-dotnet/pkgs/nuget/GraphQL.NewtonsoftJson"/></item>
	/// </list>
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

	/// <summary>
	/// Add custom fields and subqueries that are not natively part of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The model.</typeparam>
	/// <param name="setup">
	/// Place to make calls to:<br/>
	/// <c>
	/// <see cref="GraphQLObjectType{T}.AddField(MethodInfo)"/><br/>
	/// <see cref="GraphQLObjectType{T}.AddQueryItem{CHILD, MATCH}(MethodInfo, Func{T, MATCH}, Func{CHILD, MATCH})"/><br/>
	/// <see cref="GraphQLObjectType{T}.AddQueryCollection{CHILD, MATCH}(MethodInfo, Func{T, MATCH}, Func{CHILD, MATCH})"/>
	/// </c>
	/// </param>
	public static IServiceCollection AddGraphQLTypeExtensions<T>(this IServiceCollection @this, Action<GraphQLObjectType<T>> setup)
		where T : notnull
	{
		var graphType = new GraphQLObjectType<T>();
		setup(graphType);
		return @this.AddTransient<GraphQLObjectType<T>>(provider => graphType);
	}
}
