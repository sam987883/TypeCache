// Copyright(c) 2020 Samuel Abraham

using System;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Data.Business;

namespace TypeCache.Data.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Provides various database operations: <see cref="ISqlApi"/><br /><br />
		/// <i><b>Requires call to:</b></i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// </summary>
		/// <param name="databaseProvider">The database provider string ie. "Microsoft.Data.SqlClient".</param>
		/// <param name="connectionString">The database connection string.</param>
		public static IServiceCollection RegisterSqlApi(this IServiceCollection @this, string databaseProvider, string connectionString)
			=> @this.AddSingleton<ISqlApi>(new SqlApi(databaseProvider, connectionString));

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API.<br />
		/// You can implement the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="StoredProcedureRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="StoredProcedureRequest" />&gt;</code>
		/// <code>IValidationRule&lt;<see cref="DeleteRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="DeleteRequest" />&gt;</code>
		/// <code>IValidationRule&lt;<see cref="InsertRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="InsertRequest" />&gt;</code>
		/// <code>IValidationRule&lt;<see cref="BatchRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="BatchRequest" />&gt;</code>
		/// <code>IValidationRule&lt;<see cref="SelectRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="SelectRequest" />&gt;</code>
		/// <code>IValidationRule&lt;<see cref="UpdateRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="UpdateRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
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
		/// <code>IValidationRule&lt;<see cref="StoredProcedureRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="StoredProcedureRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiCallRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<ISqlApi, StoredProcedureRequest, StoredProcedureResponse>, StoredProcedureRule>()
				.AddSingleton<IValidationRule<ISqlApi, StoredProcedureRequest>, StoredProcedureValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing deletes.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="DeleteRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="DeleteRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiDeleteRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<ISqlApi, DeleteRequest, RowSet>, DeleteRule>()
				.AddSingleton<IRule<DeleteRequest, string>, DeleteRule>()
				.AddSingleton<IValidationRule<ISqlApi, DeleteRequest>, DeleteValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing inserts.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="InsertRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="InsertRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiInsertRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<ISqlApi, InsertRequest, RowSet>, InsertRule>()
				.AddSingleton<IRule<InsertRequest, string>, InsertRule>()
				.AddSingleton<IValidationRule<ISqlApi, InsertRequest>, InsertValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing merges.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="BatchRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="BatchRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiMergeRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<ISqlApi, BatchRequest, RowSet>, MergeRule>()
				.AddSingleton<IRule<BatchRequest, string>, MergeRule>()
				.AddSingleton<IValidationRule<ISqlApi, BatchRequest>, MergeValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing selects.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="SelectRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="SelectRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiSelectRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<ISqlApi, SelectRequest, RowSet>, SelectRule>()
				.AddSingleton<IRule<SelectRequest, string>, SelectRule>()
				.AddSingleton<IValidationRule<ISqlApi, SelectRequest>, SelectValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing updates.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <code>IValidationRule&lt;<see cref="UpdateRequest" />&gt;, IValidationRule&lt;<see cref="ISqlApi" />, <see cref="UpdateRequest" />&gt;</code>
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiUpdateRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<ISqlApi, UpdateRequest, RowSet>, UpdateRule>()
				.AddSingleton<IRule<UpdateRequest, string>, UpdateRule>()
				.AddSingleton<IValidationRule<ISqlApi, UpdateRequest>, UpdateValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for executing raw SQL.<br />
		/// <i><b>Requires calls to:</b></i>
		/// <code><see cref="TypeCache.Business.Extensions.IServiceCollectionExtensions.RegisterMediator"/></code>
		/// <code><see cref="RegisterSqlApi"/></code>
		/// </summary>
		public static IServiceCollection RegisterSqlApiExecuteSqlRules(this IServiceCollection @this)
			=> @this.AddSingleton<IRule<ISqlApi, SqlRequest, RowSet[]>, ExecuteSqlRule>();
	}
}
