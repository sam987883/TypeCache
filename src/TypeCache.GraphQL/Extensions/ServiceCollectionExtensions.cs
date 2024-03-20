// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
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
	/// <list type="table">
	/// <listheader>Registers the following:</listheader>
	/// <item><term>Singleton</term> <description><c>JsonConverter&lt;<see cref="ExecutionError"/>&gt;</c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="GraphQLScalarType{T}"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="IDataLoaderContextAccessor"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="IDocumentExecuter"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="IDocumentExecutionListener"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="IGraphQLSerializer"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="GraphQLEnumType{T}"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="GraphQLHashIdType"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="GraphQLUriType"/></c></description></item>
	/// <item><term>Transient</term> <description><c><see cref="GraphQLInputType{T}"/></c></description></item>
	/// <item><term>Transient</term> <description><c><see cref="GraphQLObjectType{T}"/></c></description></item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// To limit the information exposed in <c><see cref="ExecutionError"/></c>, register a <c>JsonConverter&lt;<see cref="ExecutionError"/>&gt;</c> before this call that writes out only what is desired.<br/><br/>
	/// You can override <b><see cref="IGraphQLSerializer"/></b> by registering a different implementation before this call.<br/>
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
			.AddSingleton(typeof(GraphQLScalarType<>))
			.AddSingleton(typeof(GraphQLEnumType<>))
			.AddSingleton<GraphQLHashIdType>()
			.AddSingleton<GraphQLUriType>()
			.AddTransient(typeof(GraphQLInputType<>))
			.AddTransient(typeof(GraphQLObjectType<>));
	}

	/// <summary>
	/// Add custom fields and subqueries that are not natively part of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The model.</typeparam>
	/// <param name="options">
	/// Place to make calls to:<br/>
	/// <c>
	/// <see cref="GraphQLObjectType{T}.AddField(MethodInfo)"/><br/>
	/// <see cref="GraphQLObjectType{T}.AddQueryItem{CHILD, MATCH}(MethodInfo, Func{T, MATCH}, Func{CHILD, MATCH})"/><br/>
	/// <see cref="GraphQLObjectType{T}.AddQueryCollection{CHILD, MATCH}(MethodInfo, Func{T, MATCH}, Func{CHILD, MATCH})"/>
	/// </c>
	/// </param>
	public static IServiceCollection AddGraphQLTypeExtensions<T>(this IServiceCollection @this, Action<GraphQLObjectType<T>> options)
		where T : notnull
	{
		var graphType = new GraphQLObjectType<T>();
		options(graphType);
		return @this.AddTransient<GraphQLObjectType<T>>(provider => graphType);
	}
}
