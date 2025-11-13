// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.DI;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TypeCache.GraphQL.Types;
using TypeCache.GraphQL.Web;

namespace TypeCache.GraphQL.Extensions;

public static class ApplicationBuilderExtensions
{
	extension(IApplicationBuilder @this)
	{
		/// <summary>
		/// <c>=&gt; @this.UseMiddleware&lt;<see cref="GraphQLMiddleware"/>&gt;(<paramref name="route"/>, <see langword="new"/> <see cref="ConfigureSchema"/>(<paramref name="configureSchema"/>));</c>
		/// </summary>
		/// <param name="route">The route to use for this <c><see cref="ISchema"/></c> instance.</param>
		public IApplicationBuilder UseGraphQLSchema(PathString route, IConfigureSchema configureSchema)
			=> @this.UseMiddleware<GraphQLMiddleware>(route, configureSchema);

		/// <summary>
		/// <c>=&gt; @this.UseMiddleware&lt;<see cref="GraphQLMiddleware"/>&gt;(<paramref name="route"/>, <see langword="new"/> <see cref="ConfigureSchema"/>(<paramref name="configureSchema"/>));</c>
		/// </summary>
		/// <param name="route">The route to use for this <c><see cref="ISchema"/></c> instance.</param>
		public IApplicationBuilder UseGraphQLSchema(PathString route, Action<ISchema, IServiceProvider> configureSchema)
			=> @this.UseMiddleware<GraphQLMiddleware>(route, new ConfigureSchema(configureSchema));
	}
}
