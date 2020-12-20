// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IGraphHandler"/></term> <description>The GraphQL handler that contains query and/or mutation method endpoints.</description></item>
		/// </list>
		/// </summary>
		/// <typeparam name="T">The Graph handler implementation.</typeparam>
		public static IServiceCollection RegisterGraphHandler<T>(this IServiceCollection @this)
			where T : class, IGraphHandler
			=> @this.AddSingleton<IGraphHandler, T>();

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="GraphSchema"/></term> <description>The GraphQL schema that will be built based on all registered IGraphHandler classes.</description></item>
		/// <item><term><see cref="GraphObjectType&lt;&gt;"/></term> <description>The GraphQL ObjectGraphType.</description></item>
		/// <item><term><see cref="GraphInputType&lt;&gt;"/></term> <description>The GraphQL InputObjectGraphType.</description></item>
		/// <item><term><see cref="GraphEnumType&lt;&gt;"/>, <see cref="IStaticMethodCache&lt;&gt;"/></term> <description>The GraphQL EnumerationGraphType.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterGraphQL(this IServiceCollection @this)
            => @this
                .AddSingleton(typeof(GraphObjectType<>))
                .AddSingleton(typeof(GraphInputType<>))
                .AddSingleton(typeof(GraphEnumType<>))
                .AddSingleton<GraphSchema>();
    }
}
