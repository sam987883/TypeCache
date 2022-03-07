// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.GraphQL.Types;
using TypeCache.GraphQL.Web;

namespace TypeCache.GraphQL.Extensions;

public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><see cref="IDocumentExecuter"/></term> An instance of: <see cref="DocumentExecuter"/>.</item>
	/// <item><term><see cref="IDocumentWriter"/></term> An instance of: <see cref="DocumentWriter"/>.</item>
	/// <item><term><see cref="IGraphQLExecuter{TSchema}"/></term> An instance of: <see cref="BasicGraphQLExecuter{TSchema}"/>.</item>
	/// <item><term><see cref="IDataLoaderContextAccessor"/></term> An instance of: <see cref="DataLoaderContextAccessor"/>.</item>
	/// <item><term><see cref="IDocumentExecutionListener"/></term> An instance of: <see cref="DataLoaderDocumentListener"/>.</item>
	/// <item><term><see cref="GraphQLEnumType{T}"/></term> The GraphQL EnumerationGraphType.</item>
	/// <item><term><see cref="GraphQLHashIdType"/></term> A <see cref="ScalarGraphType"/> that hashes and unhashes integer identifier types to prevent a sequential attack.</item>
	/// <item><term><see cref="GraphQLInputType{T}"/></term> The GraphQL InputObjectGraphType.</item>
	/// <item><term><see cref="GraphQLObjectEnumType{T}"/></term> Treats the property names of a type as a GraphEnumType.</item>
	/// <item><term><see cref="GraphQLObjectType{T}"/></term> The GraphQL ObjectGraphType.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterGraphQL(this IServiceCollection @this)
		=> @this.AddSingleton<IDocumentExecuter, DocumentExecuter>()
			.AddSingleton<IDocumentWriter, DocumentWriter>()
			.AddSingleton(typeof(IGraphQLExecuter<>), typeof(BasicGraphQLExecuter<>))
			.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
			.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>()
			.AddSingleton(typeof(GraphQLEnumType<>))
			.AddSingleton<GraphQLHashIdType>()
			.AddSingleton(typeof(GraphQLInputType<>))
			.AddSingleton(typeof(GraphQLObjectEnumType<>))
			.AddSingleton(typeof(GraphQLObjectType<>))
			.AddSingleton(typeof(GraphQLOrderByType<>));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.UseMiddleware&lt;<see cref="GraphQLMiddleware{T}"/>&gt;(<see langword="new"/> <see cref="PathString"/>(<paramref name="route"/>));</c>
	/// </summary>
	/// <param name="route">The route to use for this <see cref="ISchema"/>.</param>
	public static IApplicationBuilder UseGraphQLSchema<T>(this IApplicationBuilder @this, string route)
		where T : ISchema
		=> @this.UseMiddleware<GraphQLMiddleware<T>>(new PathString(route));
}
