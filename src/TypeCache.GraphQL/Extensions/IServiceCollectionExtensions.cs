// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IDataLoaderContextAccessor"/></term> An instance of: <see cref="DataLoaderContextAccessor"/>.</item>
		/// <item><term><see cref="IDocumentExecutionListener"/></term> An instance of: <see cref="DataLoaderDocumentListener"/>.</item>
		/// <item><term><see cref="GraphObjectType{T}"/></term> The GraphQL ObjectGraphType.</item>
		/// <item><term><see cref="GraphObjectEnumType{T}"/></term> Treats the property names of a type as a GraphEnumType.</item>
		/// <item><term><see cref="GraphInputType{T}"/></term> The GraphQL InputObjectGraphType.</item>
		/// <item><term><see cref="GraphEnumType{T}"/></term> The GraphQL EnumerationGraphType.</item>
		/// <item><term><see cref="GraphHashIdType"/></term> A <see cref="ScalarGraphType"/> that hashes and unhashes integer identifier types to prevent a sequential attack.</item>
		/// <item><term><see cref="ISchema"/></term> The main GraphQL schema.</item>
		/// </list>
		/// </summary>
		/// <param name="addEndpoints">Use this to register handler classes containing endpoint methods or to register SQL API generated endpoints.</param>
		/// <param name="version">The version number of the Graph Schema.</param>
		public static IServiceCollection RegisterGraphQL(this IServiceCollection @this, Action<GraphSchema> addEndpoints, string version = "0")
			=> @this
				.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
				.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>()
				.AddSingleton(typeof(GraphObjectType<>))
				.AddSingleton(typeof(GraphObjectEnumType<>))
				.AddSingleton(typeof(GraphInputType<>))
				.AddSingleton(typeof(GraphEnumType<>))
				.AddSingleton(typeof(GraphHashIdType))
				.AddSingleton<ISchema>(provider => new GraphSchema(provider, addEndpoints, version));
	}
}
