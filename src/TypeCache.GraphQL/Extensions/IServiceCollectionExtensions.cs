// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Data;
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
		/// <item><term><see cref="ISchema"/></term> The main GraphQL schema.</item>
		/// <item><term><see cref="GraphObjectType&lt;T&gt;"/></term> The GraphQL ObjectGraphType.</item>
		/// <item><term><see cref="GraphObjectEnumType&lt;T&gt;"/></term> Treats the property names of a type as a GraphEnumType.</item>
		/// <item><term><see cref="GraphInputType&lt;T&gt;"/></term> The GraphQL InputObjectGraphType.</item>
		/// <item><term><see cref="GraphEnumType&lt;T&gt;"/></term> The GraphQL EnumerationGraphType.</item>
		/// <item><term><see cref="GraphHashIdType"/></term> A <see cref="ScalarGraphType"/> that hashes and unhashes integer identifier types to prevent a sequential attack.</item>
		/// </list>
		/// </summary>
		/// <param name="addEndpoints">Use this to register handler classes containing endpoint methods or to register SQL API generated endpoints.</param>
		public static IServiceCollection RegisterGraphQL(this IServiceCollection @this, Action<GraphSchema> addEndpoints)
			=> @this
				.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
				.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>()
				.AddSingleton<ISchema>(provider => new GraphSchema(provider,
					provider.GetRequiredService<IMediator>(),
					provider.GetService<ISqlApi>(),
					provider.GetRequiredService<IDataLoaderContextAccessor>(),
					addEndpoints))
				.AddSingleton(typeof(GraphObjectType<>))
				.AddSingleton(typeof(GraphObjectEnumType<>))
				.AddSingleton(typeof(GraphInputType<>))
				.AddSingleton(typeof(GraphEnumType<>))
				.AddSingleton(typeof(GraphHashIdType));
	}
}
