// Copyright(c) 2020 Samuel Abraham

using System;
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
		/// <item><term><see cref="GraphSchema"/></term> <description>The GraphQL schema that will be built based on all registered IGraphHandler classes.</description></item>
		/// <item><term><see cref="GraphObjectType&lt;T&gt;"/></term> <description>The GraphQL ObjectGraphType.</description></item>
		/// <item><term><see cref="GraphInputType&lt;T&gt;"/></term> <description>The GraphQL InputObjectGraphType.</description></item>
		/// <item><term><see cref="GraphEnumType&lt;T&gt;"/></term> <description>The GraphQL EnumerationGraphType.</description></item>
		/// <item><term><see cref="GraphHashIdType"/></term> <description>A ScalarGraphType that hashes and unhashes integer identifier types to prevent a sequential attack.</description></item>
		/// <item><term><see cref="GraphConnectionType&lt;T&gt;"/></term> <description>GraphQL Connection type for paging.</description></item>
		/// <item><term><see cref="GraphEdgeType&lt;T&gt;"/></term> <description>Edge component of the GraphQL Connection type.</description></item>
		/// </list>
		/// </summary>
		/// <param name="addEndpoints">Use this to register handler classes containing endpoint methods or to register SQL API generated endpoints.</param>
		public static IServiceCollection RegisterGraphQL(this IServiceCollection @this, Action<GraphSchema> addEndpoints)
			=> @this
				.AddSingleton(typeof(GraphObjectType<>))
				.AddSingleton(typeof(GraphObjectEnumType<>))
				.AddSingleton(typeof(GraphInputType<>))
				.AddSingleton(typeof(GraphEnumType<>))
				.AddSingleton(typeof(GraphHashIdType))
				.AddSingleton(typeof(GraphConnectionType<>))
				.AddSingleton(typeof(GraphEdgeType<>))
				.AddSingleton<ISchema>(provider => new GraphSchema(provider, provider.GetRequiredService<IMediator>(), provider.GetService<ISqlApi>(), addEndpoints));
	}
}
