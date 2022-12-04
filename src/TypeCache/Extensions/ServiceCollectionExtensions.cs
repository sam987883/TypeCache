// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Attributes;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Data.Business;
using TypeCache.GraphQL.Extensions;
using TypeCache.Security;

namespace TypeCache.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers singleton <c><see cref="IDataSource"/></c> to be available via: <c>IAccessor&lt;<see cref="IDataSource"/>&gt;</c><br/>
	/// </summary>
	public static IServiceCollection RegisterDataSource(this IServiceCollection @this, string name, DbProviderFactory dbProviderFactory, string connectionString, DataSourceType type)
		=> @this.AddSingleton<IDataSource>(new DataSource(name, dbProviderFactory, connectionString, type));

	/// <summary>
	/// Provides data source related information: <c>IAccessor&lt;<see cref="IDataSource"/>&gt;</c><br/>
	/// <i><b>Requires call to:</b></i>
	/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
	/// </summary>
	public static IServiceCollection RegisterDataSourceAccessor(this IServiceCollection @this)
		=> @this.AddSingleton<IAccessor<IDataSource>, Accessor<IDataSource>>();

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random 16 bytes</param>
	/// <param name="rgbIV">Any random 16 bytes</param>
	public static IServiceCollection RegisterHashMaker(this IServiceCollection @this, byte[] rgbKey, byte[] rgbIV)
		=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey, rgbIV));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random decimal value (gets converted to a 16 byte array)</param>
	/// <param name="rgbIV">Any random decimal value (gets converted to a 16 byte array)</param>
	public static IServiceCollection RegisterHashMaker(this IServiceCollection @this, decimal rgbKey, decimal rgbIV)
		=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey, rgbIV));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><c><see cref="IMediator"/></c></item>
	/// <item><term><c><see cref="DefaultProcessIntermediary{REQUEST}"/></c></term> Default implementation of <c><see cref="IProcessIntermediary{REQUEST}"/></c>.</item>
	/// <item><term><c><see cref="DefaultRuleIntermediary{REQUEST, RESPONSE}"/></c></term> Default implementation of <c><see cref="IRuleIntermediary{REQUEST, RESPONSE}"/></c>.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterMediator(this IServiceCollection @this)
		=> @this.AddSingleton<IMediator, Mediator>()
			.AddSingleton(typeof(DefaultProcessIntermediary<>), typeof(DefaultProcessIntermediary<>))
			.AddSingleton(typeof(DefaultRuleIntermediary<,>), typeof(DefaultRuleIntermediary<,>));

	/// <summary>
	/// Registers Singleton Rules for the generic SQL API Commands.<br/>
	/// <code>
	/// {<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> type <see langword="in"/> typeAssembly.DefinedTypes.Where(type =&gt; type.IsDefined(typeof(SqlApiAttribute), <see langword="false"/>)))<br/>
	/// <see langword="        "/>@this.AddSingleton(<br/>
	/// <see langword="            "/><see langword="typeof"/>(IRule&lt;,&gt;).MakeGenericType(<see langword="typeof"/>(<see cref="SqlCommand"/>), typeof(IList&lt;&gt;).MakeGenericType(type)),<br/>
	/// <see langword="            "/><see langword="typeof"/>(SqlCommandModelsRule&lt;&gt;).MakeGenericType(type));<br/>
	/// <br/>
	/// <see langword="    return"/> @<paramref name="this"/>;<br/>
	/// }
	/// </code>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="RegisterMediator(IServiceCollection)"/><br/>
	/// <see cref="RegisterSqlCommandRules(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiRules(this IServiceCollection @this, Assembly typeAssembly)
	{
		foreach (var type in typeAssembly.DefinedTypes.Where(type => type.IsDefined(typeof(SqlApiAttribute), false)))
			@this.AddSingleton(
				typeof(IRule<,>).MakeGenericType(typeof(SqlCommand), typeof(IList<>).MakeGenericType(type)),
				typeof(SqlCommandModelsRule<>).MakeGenericType(type));

		return @this;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;IRule&lt;<see cref="SqlCommand"/>, <see cref="IList{T}"/>&gt;, <see cref="SqlCommandModelsRule{T}"/>&gt;();</c>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="RegisterMediator(IServiceCollection)"/><br/>
	/// <see cref="RegisterSqlCommandRules(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static void RegisterSqlCommandRule<T>(this IServiceCollection @this)
		where T : new()
		=> @this.AddSingleton<IRule<SqlCommand, IList<T>>, SqlCommandModelsRule<T>>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;IValidationRule&lt;<see cref="SqlCommand"/>, <see cref="SqlCommandValidationRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlCommand"/>, <see cref="DataSet"/>&gt;, <see cref="SqlCommandDataSetRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlCommand"/>, <see cref="DataTable"/>&gt;, <see cref="SqlCommandDataTableRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlCommand"/>, <see cref="JsonArray"/>&gt;, <see cref="SqlCommandJsonArrayRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IProcess&lt;<see cref="SqlCommand"/>, <see cref="SqlCommandProcess"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlCommand"/>, <see cref="object"/>&gt;, <see cref="SqlCommandScalarRule"/>&gt;()<br/>
	/// </c>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="RegisterMediator(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlCommandRules(this IServiceCollection @this)
		=> @this.AddSingleton<IValidationRule<SqlCommand>, SqlCommandValidationRule>()
			.AddSingleton<IRule<SqlCommand, DataSet>, SqlCommandDataSetRule>()
			.AddSingleton<IRule<SqlCommand, DataTable>, SqlCommandDataTableRule>()
			.AddSingleton<IRule<SqlCommand, JsonArray>, SqlCommandJsonArrayRule>()
			.AddSingleton<IProcess<SqlCommand>, SqlCommandProcess>()
			.AddSingleton<IRule<SqlCommand, object?>, SqlCommandScalarRule>();
}
