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
	/// <c>=&gt; @<paramref name="this"/>.UseMiddleware&lt;<see cref="GraphQLMiddleware"/>&gt;(<paramref name="name"/>, <paramref name="route"/>);</c>
	/// </summary>
	/// <param name="name">
	/// The name of the <c><see cref="GraphQLSchema"/></c> registered by a call to:<br/>
	/// <c><see cref="AddGraphQLSchema(IServiceCollection, string, Action{GraphQLSchema})"/></c>.
	/// </param>
	/// <param name="route">The route to use for this <c><see cref="GraphQLSchema"/></c>.</param>
	public static IApplicationBuilder UseGraphQLSchema(this IApplicationBuilder @this, PathString route, Action<ISchema, IServiceProvider> configureSchema)
		=> @this.UseMiddleware<GraphQLMiddleware>(route, new ConfigureSchema(configureSchema));
}
