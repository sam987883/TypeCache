// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.Attributes;
using TypeCache.Data;
using TypeCache.Data.Mediation;
using TypeCache.Mediation;
using TypeCache.Net.Mediation;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers singleton <c><see cref="IDataSource"/></c> to be available via: <c>IAccessor&lt;<see cref="IDataSource"/>&gt;</c><br/>
	/// </summary>
	public static IServiceCollection AddDataSource(this IServiceCollection @this, string name, DbProviderFactory dbProviderFactory, string connectionString, DataSourceType type)
		=> @this.AddSingleton<IDataSource>(new DataSource(name, dbProviderFactory, connectionString, type));

	/// <summary>
	/// Provides data source related information: <c>IAccessor&lt;<see cref="IDataSource"/>&gt;</c><br/>
	/// <i><b>Requires call to:</b></i>
	/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
	/// </summary>
	public static IServiceCollection AddDataSourceAccessor(this IServiceCollection @this)
		=> @this.AddSingleton<IAccessor<IDataSource>, Accessor<IDataSource>>();

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random 16 bytes</param>
	/// <param name="rgbIV">Any random 16 bytes</param>
	public static IServiceCollection AddHashMaker(this IServiceCollection @this, byte[] rgbKey, byte[] rgbIV)
		=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey, rgbIV));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random decimal value (gets converted to a 16 byte array)</param>
	/// <param name="rgbIV">Any random decimal value (gets converted to a 16 byte array)</param>
	public static IServiceCollection AddHashMaker(this IServiceCollection @this, decimal rgbKey, decimal rgbIV)
		=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey, rgbIV));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;IRule&lt;<see cref="HttpClientRequest"/>, <see cref="HttpResponseMessage"/>&gt;, <see cref="HttpClientRule"/>&gt;()</c>
	/// </summary>
	/// <remarks>Requires call to: <c>IServiceCollection.AddHttpClient</c></remarks>
	public static IServiceCollection AddHttpClientRule(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<HttpClientRequest, HttpResponseMessage>, HttpClientRule>();

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><c><see cref="IMediator"/></c></item>
	/// <item><term><c><see cref="DefaultProcessIntermediary{REQUEST}"/></c></term> Default implementation of <c><see cref="IProcessIntermediary{REQUEST}"/></c>.</item>
	/// <item><term><c><see cref="DefaultRuleIntermediary{REQUEST, RESPONSE}"/></c></term> Default implementation of <c><see cref="IRuleIntermediary{REQUEST, RESPONSE}"/></c>.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection AddMediation(this IServiceCollection @this)
		=> @this.AddSingleton<IMediator, Mediator>()
			.AddSingleton(typeof(DefaultProcessIntermediary<>), typeof(DefaultProcessIntermediary<>))
			.AddSingleton(typeof(DefaultRuleIntermediary<>), typeof(DefaultRuleIntermediary<>))
			.AddSingleton(typeof(DefaultRuleIntermediary<,>), typeof(DefaultRuleIntermediary<,>));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;IRule&lt;<see cref="SqlDataSetRequest"/>, <see cref="DataSet"/>&gt;, <see cref="SqlDataSetRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlDataTableRequest"/>, <see cref="DataTable"/>&gt;, <see cref="SqlDataTableRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlExecuteRequest"/>, <see cref="SqlExecuteRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlJsonArrayRequest"/>, <see cref="JsonArray"/>&gt;, <see cref="SqlJsonArrayRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlModelsRequest"/>, IList&lt;<see cref="object"/>&gt;&gt;, <see cref="SqlModelsRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlScalarRequest"/>, <see cref="object"/>&gt;, <see cref="SqlScalarRule"/>&gt;()<br/>
	/// </c>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="AddMediation(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static IServiceCollection AddSqlCommandRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<SqlDataSetRequest, DataSet>, SqlDataSetRule>()
			.AddSingleton<IRule<SqlDataTableRequest, DataTable>, SqlDataTableRule>()
			.AddSingleton<IRule<SqlExecuteRequest>, SqlExecuteRule>()
			.AddSingleton<IRule<SqlJsonArrayRequest, JsonArray>, SqlJsonArrayRule>()
			.AddSingleton<IRule<SqlModelsRequest, IList<object>>, SqlModelsRule>()
			.AddSingleton<IRule<SqlScalarRequest, object?>, SqlScalarRule>();

	/// <summary>
	/// Registers all types in the specified assembly that have <see cref="ServiceLifetimeAttribute"/>.
	/// </summary>
	/// <param name="fromAssembly">The assembly to register the types from.</param>
	public static IServiceCollection AddTypes(this IServiceCollection @this, Assembly fromAssembly)
	{
		var serviceDescriptors = fromAssembly.GetTypes()
			.Where(type => !type.IsAbstract && !type.IsGenericType && !type.IsInterface && !type.IsPointer && !type.IsPrimitive
				&& type.HasCustomAttribute<ServiceLifetimeAttribute>())
			.Select(implementationType =>
			{
				var attribute = implementationType.GetCustomAttribute<ServiceLifetimeAttribute>()!;
				return new ServiceDescriptor(attribute.ServiceType ?? implementationType, implementationType, attribute.ServiceLifetime);
			})
			.ToArray();
		serviceDescriptors.ForEach(@this.Add);

		return @this;
	}
}
