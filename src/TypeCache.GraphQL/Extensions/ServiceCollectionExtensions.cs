// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Text.Json.Serialization;
using global::GraphQL;
using global::GraphQL.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.GraphQL.Converters;
using TypeCache.GraphQL.Listeners;
using TypeCache.GraphQL.Types;
using TypeCache.GraphQL.Web;

namespace TypeCache.GraphQL.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// <list type="table">
	/// <listheader>Registers the following:</listheader>
	/// <item><term>Singleton</term> <description><c>JsonConverter&lt;<see cref="ExecutionError"/>&gt;</c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="IDocumentExecuter"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="IDocumentExecutionListener"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="IGraphQLSerializer"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="EnumGraphType{T}"/></c></description></item>
	/// <item><term>Singleton</term> <description><c><see cref="HashIdGraphType"/></c></description></item>
	/// <item><term>Transient</term> <description><c><see cref="InputGraphType{T}"/></c></description></item>
	/// <item><term>Transient</term> <description><c><see cref="OutputGraphType{T}"/></c></description></item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// To limit the information exposed in <c><see cref="ExecutionError"/></c>, register a <c>JsonConverter&lt;<see cref="ExecutionError"/>&gt;</c> before this call that writes out only what is desired.<br/><br/>
	/// You can override <b><see cref="IDocumentExecutionListener"/></b> by registering a different implementation before this call.<br/>
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
		@this.TryAddSingleton<IDocumentExecutionListener, DefaultDocumentExecutionListener>();
		return @this.AddSingleton<IDocumentExecuter, DocumentExecuter>()
			.AddSingleton(typeof(EnumGraphType<>))
			.AddSingleton<HashIdGraphType>()
			.AddTransient(typeof(InputGraphType<>))
			.AddTransient(typeof(OutputGraphType<>));
	}

	/// <summary>
	/// Add custom fields and subqueries that are not natively part of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The model.</typeparam>
	/// <param name="options">
	/// Place to make calls to:<br/>
	/// <c>
	/// <see cref="OutputGraphType{T}.AddField(MethodInfo)"/><br/>
	/// <see cref="OutputGraphType{T}.AddQueryItem{CHILD, MATCH}(MethodInfo, Func{T, MATCH}, Func{CHILD, MATCH})"/><br/>
	/// <see cref="OutputGraphType{T}.AddQueryCollection{CHILD, MATCH}(MethodInfo, Func{T, MATCH}, Func{CHILD, MATCH})"/>
	/// </c>
	/// </param>
	public static IServiceCollection AddGraphQLTypeExtensions<T>(this IServiceCollection @this, Action<OutputGraphType<T>> options)
		where T : notnull
	{
		var graphType = new OutputGraphType<T>();
		options(graphType);
		return @this.AddTransient<OutputGraphType<T>>(provider => graphType);
	}
}
