// Copyright(c) 2020 Samuel Abraham

using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Business;

namespace TypeCache.Data.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Provides various database operations: <see cref="ISqlApi"/><br /><br />
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
		/// Provides various database operations: <see cref="ISqlApi"/><br /><br />
		/// <i><b>Requires call to:</b></i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApi(this IServiceCollection @this, params DataSource[] dataSources)
			=> @this.AddSingleton<ISqlApi>(new SqlApi(dataSources));

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API.<br />
		/// You can implement the following validation rules to validate the request before the database call is made:
		/// <code>
		/// IValidationRule&lt;<see cref="StoredProcedureRequest" />&gt;<br/>
		/// IValidationRule&lt;<see cref="DeleteRequest" />&gt;<br/>
		/// IValidationRule&lt;<see cref="InsertRequest" />&gt;<br/>
		/// IValidationRule&lt;<see cref="BatchRequest" />&gt;<br/>
		/// IValidationRule&lt;<see cref="SelectRequest" />&gt;<br/>
		/// IValidationRule&lt;<see cref="UpdateRequest" />&gt;
		/// </code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiRules(this IServiceCollection @this)
			=> @this.RegisterSqlApiCallRules()
				.RegisterSqlApiDeleteRules()
				.RegisterSqlApiInsertRules()
				.RegisterSqlApiMergeRules()
				.RegisterSqlApiSelectRules()
				.RegisterSqlApiUpdateRules();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for calling procedures.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="StoredProcedureRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiCallRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<StoredProcedureRequest, StoredProcedureResponse>, StoredProcedureRule>()
				.AddSingleton<IValidationRule<StoredProcedureRequest>, StoredProcedureValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing deletes.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="DeleteRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiDeleteRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<DeleteRequest, RowSet>, DeleteRule>()
				.AddSingleton<IRule<DeleteRequest, string>, DeleteRule>()
				.AddSingleton<IValidationRule<DeleteRequest>, DeleteValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing inserts.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="InsertRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiInsertRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<InsertRequest, RowSet>, InsertRule>()
				.AddSingleton<IRule<InsertRequest, string>, InsertRule>()
				.AddSingleton<IValidationRule<InsertRequest>, InsertValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing merges.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="BatchRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiMergeRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<BatchRequest, RowSet>, MergeRule>()
				.AddSingleton<IRule<BatchRequest, string>, MergeRule>()
				.AddSingleton<IValidationRule<BatchRequest>, MergeValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing selects.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="SelectRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiSelectRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<SelectRequest, RowSet>, SelectRule>()
				.AddSingleton<IRule<SelectRequest, string>, SelectRule>()
				.AddSingleton<IValidationRule<SelectRequest>, SelectValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing updates.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="UpdateRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiUpdateRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<UpdateRequest, RowSet>, UpdateRule>()
				.AddSingleton<IRule<UpdateRequest, string>, UpdateRule>()
				.AddSingleton<IValidationRule<UpdateRequest>, UpdateValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for executing raw SQL.<br />
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiExecuteSqlRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<SqlRequest, RowSet[]>, ExecuteSqlRule>();
	}
}
