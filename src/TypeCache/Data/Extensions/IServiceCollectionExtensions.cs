// Copyright(c) 2020 Samuel Abraham

using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Data.Business;

namespace TypeCache.Data.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <list type="bullet">
		/// <item><term><see cref="IValidationRule&lt;StoredProcedureRequest&gt;"/>, <see cref="IValidationRule&lt;DbConnection,StoredProcedureRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="StoredProcedureRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;DeleteRequest&gt;"/>, <see cref="IValidationRule&lt;DbConnection,DeleteRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="DeleteRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;InsertRequest&gt;"/>, <see cref="IValidationRule&lt;DbConnection,InsertRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="InsertRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;BatchRequest&gt;"/>, <see cref="IValidationRule&lt;DbConnection,BatchRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="BatchRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;SelectRequest&gt;"/>, <see cref="IValidationRule&lt;DbConnection,SelectRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="SelectRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;UpdateRequest&gt;"/>, <see cref="IValidationRule&lt;DbConnection,UpdateRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="UpdateRequest" />.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterSqlApi(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, StoredProcedureRequest, StoredProcedureResponse>, CallRuleHandler>()
				.AddSingleton<IRuleHandler<DbConnection, DeleteRequest, RowSet>, DataRuleHandler<DeleteRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, DeleteRequest, string>, SqlRuleHandler<DeleteRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, InsertRequest, RowSet>, DataRuleHandler<InsertRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, InsertRequest, string>, SqlRuleHandler<InsertRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, BatchRequest, RowSet>, DataRuleHandler<BatchRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, BatchRequest, string>, SqlRuleHandler<BatchRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, SelectRequest, RowSet>, DataRuleHandler<SelectRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, SelectRequest, string>, SqlRuleHandler<SelectRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, UpdateRequest, RowSet>, DataRuleHandler<UpdateRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, UpdateRequest, string>, SqlRuleHandler<UpdateRequest>>()
				.AddSingleton<IRule<DbConnection, StoredProcedureRequest, StoredProcedureResponse>, StoredProcedureRule>()
				.AddSingleton<IRule<DbConnection, DeleteRequest, RowSet>, DeleteRule>()
				.AddSingleton<IRule<DbConnection, DeleteRequest, string>, DeleteRule>()
				.AddSingleton<IRule<DbConnection, InsertRequest, RowSet>, InsertRule>()
				.AddSingleton<IRule<DbConnection, InsertRequest, string>, InsertRule>()
				.AddSingleton<IRule<DbConnection, BatchRequest, RowSet>, MergeRule>()
				.AddSingleton<IRule<DbConnection, BatchRequest, string>, MergeRule>()
				.AddSingleton<IRule<DbConnection, SelectRequest, RowSet>, SelectRule>()
				.AddSingleton<IRule<DbConnection, SelectRequest, string>, SelectRule>()
				.AddSingleton<IRule<DbConnection, UpdateRequest, RowSet>, UpdateRule>()
				.AddSingleton<IRule<DbConnection, UpdateRequest, string>, UpdateRule>()
				.AddSingleton<IValidationRule<DbConnection, DeleteRequest>, DeleteValidationRule>()
				.AddSingleton<IValidationRule<DbConnection, InsertRequest>, InsertValidationRule>()
				.AddSingleton<IValidationRule<DbConnection, BatchRequest>, MergeValidationRule>()
				.AddSingleton<IValidationRule<DbConnection, SelectRequest>, SelectValidationRule>()
				.AddSingleton<IValidationRule<DbConnection, StoredProcedureRequest>, StoredProcedureValidationRule>()
				.AddSingleton<IValidationRule<DbConnection, UpdateRequest>, UpdateValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for calling procedures.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <list type="bullet">
		/// <item><term><see cref="IValidationRule&lt;StoredProcedureRequest&gt;"/></term> <description>where T = <see cref="StoredProcedureRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;DbConnection,StoredProcedureRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="StoredProcedureRequest" />.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterSqlApiCall(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, StoredProcedureRequest, StoredProcedureResponse>, CallRuleHandler>()
				.AddSingleton<IRule<DbConnection, StoredProcedureRequest, StoredProcedureResponse>, StoredProcedureRule>()
				.AddSingleton<IValidationRule<DbConnection, StoredProcedureRequest>, StoredProcedureValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing deletes.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <list type="bullet">
		/// <item><term><see cref="IValidationRule&lt;DeleteRequest&gt;"/></term> <description>where T = <see cref="DeleteRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;DbConnection,UpdateRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="UpdateRequest" />.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterSqlApiDelete(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, DeleteRequest, RowSet>, DataRuleHandler<DeleteRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, DeleteRequest, string>, SqlRuleHandler<DeleteRequest>>()
				.AddSingleton<IRule<DbConnection, DeleteRequest, RowSet>, DeleteRule>()
				.AddSingleton<IRule<DbConnection, DeleteRequest, string>, DeleteRule>()
				.AddSingleton<IValidationRule<DbConnection, DeleteRequest>, DeleteValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing inserts.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <list type="bullet">
		/// <item><term><see cref="IValidationRule&lt;InsertRequest&gt;"/></term> <description>where T = <see cref="InsertRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;DbConnection,InsertRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="InsertRequest" />.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterSqlApiInsert(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, InsertRequest, RowSet>, DataRuleHandler<InsertRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, InsertRequest, string>, SqlRuleHandler<InsertRequest>>()
				.AddSingleton<IRule<DbConnection, InsertRequest, RowSet>, InsertRule>()
				.AddSingleton<IRule<DbConnection, InsertRequest, string>, InsertRule>()
				.AddSingleton<IValidationRule<DbConnection, InsertRequest>, InsertValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing merges.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <list type="bullet">
		/// <item><term><see cref="IValidationRule&lt;BatchRequest&gt;"/></term> <description>where T = <see cref="BatchRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;DbConnection,BatchRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="BatchRequest" />.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterSqlApiMerge(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, BatchRequest, RowSet>, DataRuleHandler<BatchRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, BatchRequest, string>, SqlRuleHandler<BatchRequest>>()
				.AddSingleton<IRule<DbConnection, BatchRequest, RowSet>, MergeRule>()
				.AddSingleton<IRule<DbConnection, BatchRequest, string>, MergeRule>()
				.AddSingleton<IValidationRule<DbConnection, BatchRequest>, MergeValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing selects.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <list type="bullet">
		/// <item><term><see cref="IValidationRule&lt;SelectRequest&gt;"/></term> <description>where T = <see cref="SelectRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;DbConnection,SelectRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="SelectRequest" />.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterSqlApiSelect(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, SelectRequest, RowSet>, DataRuleHandler<SelectRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, SelectRequest, string>, SqlRuleHandler<SelectRequest>>()
				.AddSingleton<IRule<DbConnection, SelectRequest, RowSet>, SelectRule>()
				.AddSingleton<IRule<DbConnection, SelectRequest, string>, SelectRule>()
				.AddSingleton<IValidationRule<DbConnection, SelectRequest>, SelectValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for doing updates.<br />
		/// You can register the following validation rules to validate the request before the database call is made:
		/// <list type="bullet">
		/// <item><term><see cref="IValidationRule&lt;UpdateRequest&gt;"/></term> <description>where T = <see cref="UpdateRequest" />.</description></item>
		/// <item><term><see cref="IValidationRule&lt;DbConnection,UpdateRequest&gt;"/></term> <description>where M = <see cref="DbConnection" /> and T = <see cref="UpdateRequest" />.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterSqlApiUpdate(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, UpdateRequest, RowSet>, DataRuleHandler<UpdateRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, UpdateRequest, string>, SqlRuleHandler<UpdateRequest>>()
				.AddSingleton<IRule<DbConnection, UpdateRequest, RowSet>, UpdateRule>()
				.AddSingleton<IRule<DbConnection, UpdateRequest, string>, UpdateRule>()
				.AddSingleton<IValidationRule<DbConnection, UpdateRequest>, UpdateValidationRule>();

		/// <summary>
		/// Registers Singleton Rules and RuleHandlers consumed by SQL API for executing raw SQL.<br />
		/// </summary>
		public static IServiceCollection RegisterSqlApiExecuteSql(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, SqlRequest, RowSet[]>, ExecuteSqlRuleHandler>()
				.AddSingleton<IRule<DbConnection, SqlRequest, RowSet[]>, ExecuteSqlRule>();
	}
}
