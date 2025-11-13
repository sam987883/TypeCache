// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using global::GraphQL;
using global::GraphQL.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.Converters;
using TypeCache.GraphQL.Converters;
using TypeCache.GraphQL.Listeners;
using TypeCache.GraphQL.Types;
using TypeCache.GraphQL.Web;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection @this)
	{
		/// <summary>
		/// <list type="table">
		/// <listheader>Registers the following:</listheader>
		/// <item><term>Singleton</term> <description><c><see cref="IDocumentExecuter"/></c></description></item>
		/// <item><term>Singleton</term> <description><c><see cref="IDocumentExecutionListener"/></c> <b>(if one is not already registered)</b></description></item>
		/// <item><term>Singleton</term> <description><c><see cref="IGraphQLSerializer"/></c> <b>(if one is not already registered)</b></description></item>
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
		public IServiceCollection AddGraphQL()
		{
			@this.TryAddSingleton<IGraphQLSerializer>(provider =>
			{
				var jsonOptions = new JsonSerializerOptions()
				{
					MaxDepth = 40,
					PropertyNameCaseInsensitive = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};
				jsonOptions.Converters.Add(new JsonStringEnumConverter());
				jsonOptions.Converters.Add(new BigIntegerJsonConverter());
				jsonOptions.Converters.Add(new DictionaryJsonConverter());
				jsonOptions.Converters.Add(new GraphQLExecutionResultJsonConverter());
				jsonOptions.Converters.Add(new GraphQLExecutionErrorJsonConverter());

				return new GraphQLJsonSerializer(jsonOptions);
			});
			@this.TryAddSingleton<IDocumentExecutionListener, DefaultDocumentExecutionListener>();
			return @this.AddSingleton<IDocumentExecuter, DocumentExecuter>()
				.AddScoped(typeof(EnumGraphType<>))
				.AddSingleton<HashIdGraphType>()
				.AddScoped(typeof(InterfaceGraphType<>))
				.AddScoped(typeof(InputGraphType<>))
				.AddScoped(typeof(OutputGraphType<>));
		}

		/// <summary>
		/// Add custom fields and subqueries that are not natively part of <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The model.</typeparam>
		/// <param name="options">
		/// Place to make calls to:<br/>
		/// <c>
		/// <see cref="OutputGraphType{T}.AddField(PropertyEntity)"/><br/>
		/// <see cref="OutputGraphType{T}.AddField(StaticMethodEntity)"/><br/>
		/// <see cref="OutputGraphType{T}.AddField{ITEM}(StaticMethodEntity, Func{T, ITEM[], ITEM})"/>
		/// <see cref="OutputGraphType{T}.AddField{ITEM}(StaticMethodEntity, Func{T, ITEM[], ITEM[]})"/>
		/// </c>
		/// </param>
		public IServiceCollection AddGraphQLTypeExtensions<T>(Action<OutputGraphType<T>> options)
			where T : notnull
			=> @this.AddScoped<OutputGraphType<T>>(provider =>
			{
				var graphType = new OutputGraphType<T>();
				options(graphType);
				return graphType;
			});
	}
}
