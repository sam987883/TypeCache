// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Business;
using TypeCache.Data.Requests;
using TypeCache.Data.Responses;
using TypeCache.Mappers;
using TypeCache.Security;

namespace TypeCache.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers Singleton:
	/// <list type="bullet">
	/// <item><term><c><see cref="IFieldMapper{FROM, TO}"/></c></term> Default field mapper implementation matching by field names only.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterDefaultFieldMapper(this IServiceCollection @this)
		=> @this.AddSingleton(typeof(IFieldMapper<,>), typeof(DefaultFieldMapper<,>));

	/// <summary>
	/// Registers Singleton:
	/// <list type="bullet">
	/// <item><term><c><see cref="IFieldToCsvMapper{T}"/></c></term> Default field to CSV mapper implementation matching by field names only.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterDefaultFieldToCsvMapper(this IServiceCollection @this)
		=> @this.AddSingleton(typeof(IFieldToCsvMapper<>), typeof(DefaultFieldToCsvMapper<>));

	/// <summary>
	/// Registers Singleton:
	/// <list type="bullet">
	/// <item><term><c><see cref="IPropertyMapper{FROM, TO}"/></c></term> Default property mapper implementation matching by property names only.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterDefaultPropertyMapper(this IServiceCollection @this)
		=> @this.AddSingleton(typeof(IPropertyMapper<,>), typeof(DefaultPropertyMapper<,>));

	/// <summary>
	/// Registers Singleton:
	/// <list type="bullet">
	/// <item><term><c><see cref="IPropertyToCsvMapper{T}"/></c></term> Default property to CSV mapper implementation matching by property names only.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterDefaultPropertyToCsvMapper(this IServiceCollection @this)
		=> @this.AddSingleton(typeof(IPropertyToCsvMapper<>), typeof(DefaultPropertyToCsvMapper<>));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random 16 bytes</param>
	/// <param name="rgbIV">Any random 16 bytes</param>
	public static IServiceCollection RegisterHashMaker(this IServiceCollection @this, byte[] rgbKey, byte[] rgbIV)
		=> @this.AddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random decimal value (gets converted to a 16 byte array)</param>
	/// <param name="rgbIV">Any random decimal value (gets converted to a 16 byte array)</param>
	public static IServiceCollection RegisterHashMaker(this IServiceCollection @this, decimal rgbKey, decimal rgbIV)
		=> @this.AddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><c><see cref="IMediator"/></c></item>
	/// <item><term><c><see cref="DefaultProcessIntermediary{I}"/></c></term> Default implementation of <c><see cref="IProcessIntermediary{I}"/></c>.</item>
	/// <item><term><c><see cref="DefaultRuleIntermediary{I, O}"/></c></term> Default implementation of <c><see cref="IRuleIntermediary{I, O}"/></c>.</item>
	/// </list>
	/// </summary>
	public static IServiceCollection RegisterMediator(this IServiceCollection @this)
		=> @this.AddSingleton<IMediator, Mediator>()
			.AddSingleton(typeof(DefaultProcessIntermediary<>), typeof(DefaultProcessIntermediary<>))
			.AddSingleton(typeof(DefaultRuleIntermediary<,>), typeof(DefaultRuleIntermediary<,>));

	/// <summary>
	/// Provides various database operations: <c><see cref="ISqlApi"/></c><br/><br/>
	/// <i><b>Requires call to:</b></i>
	/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
	/// Sample config section:
	/// <code>
	/// "dataSources": [<br/>
	///   "Default": {<br/>
	///     "connectionString": "Data Source=localhost;Initial Catalog=Customers; Integrated Security=SSPI",<br/>
	///     "databaseProvider": "Microsoft.Data.SqlClient"<br/>
	///   }, ...<br/>
	/// ]
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApi(this IServiceCollection @this, IConfigurationSection dataSourceSection)
		=> @this.AddSingleton<ISqlApi>(new SqlApi(dataSourceSection.GetChildren().To(section => new DataSource
		{
			Name = section.Key,
			ConnectionString = section.GetSection("connectionString").Value,
			DatabaseProvider = section.GetSection("databaseProvider")?.Value ?? "Microsoft.Data.SqlClient"
		}).ToArray()));

	/// <summary>
	/// Provides various database operations: <c><see cref="ISqlApi"/></c><br/>
	/// <i><b>Requires call to:</b></i>
	/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
	/// </summary>
	public static IServiceCollection RegisterSqlApi(this IServiceCollection @this, params DataSource[] dataSources)
		=> @this.AddSingleton<ISqlApi>(new SqlApi(dataSources));

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for calling procedures.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="StoredProcedureRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiCallRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<StoredProcedureRequest, StoredProcedureResponse>, StoredProcedureRule>()
			.AddSingleton<IValidationRule<StoredProcedureRequest>, StoredProcedureValidationRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for deleting a batch of records.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="DeleteDataRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiDeleteDataRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<DeleteDataRequest, RowSet>, DeleteDataRule>()
			.AddSingleton<IRule<DeleteDataRequest, string>, DeleteDataRule>()
			.AddSingleton<IValidationRule<DeleteDataRequest>, DeleteDataValidationRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for performing deletes.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="DeleteRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiDeleteRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<DeleteRequest, RowSet>, DeleteRule>()
			.AddSingleton<IRule<DeleteRequest, string>, DeleteRule>()
			.AddSingleton<IValidationRule<DeleteRequest>, DeleteValidationRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for executing raw SQL.<br/>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiExecuteSqlRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<SqlRequest, RowSet[]>, ExecuteSqlRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for inserting a batch of records.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="InsertDataRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiInsertDataRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<InsertDataRequest, RowSet>, InsertDataRule>()
			.AddSingleton<IRule<InsertDataRequest, string>, InsertDataRule>()
			.AddSingleton<IValidationRule<InsertDataRequest>, InsertDataValidationRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for performing inserts.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="InsertRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiInsertRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<InsertRequest, RowSet>, InsertRule>()
			.AddSingleton<IRule<InsertRequest, string>, InsertRule>()
			.AddSingleton<IValidationRule<InsertRequest>, InsertValidationRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API.<br/>
	/// You can implement the following validation rules to validate the request before the database call is made:
	/// <c>
	/// <list type="table">
	/// <item>IValidationRule&lt;<see cref="StoredProcedureRequest" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="DeleteDataRequest" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="DeleteRequest" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="InsertDataRequest" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="InsertRequest" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="SelectRequest" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="UpdateDataRequest" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="UpdateRequest" />&gt;</item>
	/// </list>
	/// </c>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiRules(this IServiceCollection @this)
		=> @this.RegisterSqlApiCallRules()
			.RegisterSqlApiDeleteDataRules()
			.RegisterSqlApiDeleteRules()
			.RegisterSqlApiInsertDataRules()
			.RegisterSqlApiInsertRules()
			.RegisterSqlApiSelectRules()
			.RegisterSqlApiUpdateDataRules()
			.RegisterSqlApiUpdateRules();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for performing selects.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="SelectRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiSelectRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<SelectRequest, RowSet>, SelectRule>()
			.AddSingleton<IRule<SelectRequest, string>, SelectRule>()
			.AddSingleton<IValidationRule<SelectRequest>, SelectValidationRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for updating a batch of records.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="UpdateDataRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiUpdateDataRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<UpdateDataRequest, RowSet>, UpdateDataRule>()
			.AddSingleton<IRule<UpdateDataRequest, string>, UpdateDataRule>()
			.AddSingleton<IValidationRule<UpdateDataRequest>, UpdateDataValidationRule>();

	/// <summary>
	/// Registers Singleton Rules and RuleHandlers consumed by SQL API for performing updates.<br/>
	/// You can register the following validation rules to validate the request before the database call is made:
	/// <code>IValidationRule&lt;<see cref="UpdateRequest" />&gt;</code>
	/// <i><b>Requires calls to:</b></i>
	/// <code><see cref="RegisterMediator(IServiceCollection)"/></code>
	/// <code>
	/// <see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/><br/>
	/// - or -<br/>
	/// <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiUpdateRules(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<UpdateRequest, RowSet>, UpdateRule>()
			.AddSingleton<IRule<UpdateRequest, string>, UpdateRule>()
			.AddSingleton<IValidationRule<UpdateRequest>, UpdateValidationRule>();
}
