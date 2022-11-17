// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.GraphQL.Types;
using TypeCache.GraphQL.Web;

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
	/// <item><term>IAccessor&lt;<see cref="GraphQLSchema"/>&gt;</term> An instance of: Accessor&lt;<see cref="GraphQLSchema"/>&gt;.</item>
	/// <item><term><see cref="GraphQLEnumType{T}"/></term> The GraphQL EnumerationGraphType.</item>
	/// <item><term><see cref="GraphQLHashIdType"/></term> A <see cref="ScalarGraphType"/> that hashes and unhashes integer identifier types to prevent a sequential attack.</item>
	/// <item><term><see cref="GraphQLInputType{T}"/></term> The GraphQL InputObjectGraphType.</item>
	/// <item><term><see cref="GraphQLObjectType{T}"/></term> The GraphQL ObjectGraphType.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterGraphQL(this IServiceCollection @this)
		=> @this.AddSingleton<IDocumentExecuter, DocumentExecuter>()
			.AddSingleton<IGraphQLSerializer, GraphQLSerializer>()
			.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
			.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>()
			.AddSingleton<IAccessor<GraphQLSchema>, Accessor<GraphQLSchema>>()
			.AddSingleton(typeof(GraphQLEnumType<>))
			.AddSingleton<GraphQLHashIdType>()
			.AddSingleton(typeof(GraphQLInputType<>))
			.AddSingleton(typeof(GraphQLObjectType<>));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;<see cref="GraphQLSchema"/>&gt;(provider =&gt; <see langword="new"/> <see cref="GraphQLSchema"/>(provider, <paramref name="name"/>, <paramref name="initializeSchema"/>));</c>
	/// </summary>
	/// <param name="name">
	/// Register an instance of <c><see cref="GraphQLSchema"/></c> to be consumed by the call to:<br/>
	/// <c><see cref="UseGraphQLSchema{T}(IApplicationBuilder, string, PathString)"/></c>.
	/// </param>
	/// <param name="initializeSchema"></param>
	/// <returns></returns>
	public static IServiceCollection RegisterGraphQLSchema(this IServiceCollection @this, string name, Action<GraphQLSchema> initializeSchema)
		=> @this.AddSingleton<GraphQLSchema>(provider => new GraphQLSchema(provider, name, initializeSchema));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.UseMiddleware&lt;<see cref="GraphQLMiddleware"/>&gt;(<paramref name="name"/>, <paramref name="route"/>);</c>
	/// </summary>
	/// <param name="name">
	/// The name of the <c><see cref="GraphQLSchema"/></c> registered by a call to:<br/>
	/// <c><see cref="RegisterGraphQLSchema(IServiceCollection, string, Action{GraphQLSchema})"/></c>.
	/// </param>
	/// <param name="route">The route to use for this <c><see cref="GraphQLSchema"/></c>.</param>
	public static IApplicationBuilder UseGraphQLSchema(this IApplicationBuilder @this, string name, PathString route)
		=> @this.UseMiddleware<GraphQLMiddleware>(name, route);
}
