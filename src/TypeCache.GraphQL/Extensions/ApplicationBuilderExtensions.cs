// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TypeCache.GraphQL.Types;
using TypeCache.GraphQL.Web;

namespace TypeCache.GraphQL.Extensions;

public static class ApplicationBuilderExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.UseMiddleware&lt;<see cref="GraphQLMiddleware"/>&gt;(<paramref name="route"/>, <see langword="new"/> <see cref="ConfigureSchema"/>(<paramref name="configureSchema"/>));</c>
	/// </summary>
	/// <param name="route">The route to use for this <c><see cref="ISchema"/></c> instance.</param>
	public static IApplicationBuilder UseGraphQLSchema(this IApplicationBuilder @this, PathString route, Action<ISchema, IServiceProvider> configureSchema)
		=> @this.UseMiddleware<GraphQLMiddleware>(route, new ConfigureSchema(configureSchema));
}
