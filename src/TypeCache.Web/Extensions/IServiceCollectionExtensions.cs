// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Web.Business;

namespace TypeCache.Web.Extenstions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singletons Rules and RuleHandlers consumed by SQL API.
		/// </summary>
		public static IServiceCollection RegisterSqlApi(this IServiceCollection @this)
			=> @this.AddSingleton<IRuleHandler<DbConnection, DeleteRequest, RowSet>, DataRuleHandler<DeleteRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, DeleteRequest, string>, SqlRuleHandler<DeleteRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, InsertRequest, RowSet>, DataRuleHandler<InsertRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, InsertRequest, string>, SqlRuleHandler<InsertRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, BatchRequest, RowSet>, DataRuleHandler<BatchRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, BatchRequest, string>, SqlRuleHandler<BatchRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, SelectRequest, RowSet>, DataRuleHandler<SelectRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, SelectRequest, string>, SqlRuleHandler<SelectRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, UpdateRequest, RowSet>, DataRuleHandler<UpdateRequest>>()
				.AddSingleton<IRuleHandler<DbConnection, UpdateRequest, string>, SqlRuleHandler<UpdateRequest>>()
				.AddSingleton<IRule<DbConnection, DeleteRequest, RowSet>, DeleteRule>()
				.AddSingleton<IRule<DbConnection, DeleteRequest, string>, DeleteRule>()
				.AddSingleton<IRule<DbConnection, InsertRequest, RowSet>, InsertRule>()
				.AddSingleton<IRule<DbConnection, InsertRequest, string>, InsertRule>()
				.AddSingleton<IRule<DbConnection, BatchRequest, RowSet>, MergeRule>()
				.AddSingleton<IRule<DbConnection, BatchRequest, string>, MergeRule>()
				.AddSingleton<IRule<DbConnection, SelectRequest, RowSet>, SelectRule>()
				.AddSingleton<IRule<DbConnection, SelectRequest, string>, SelectRule>()
				.AddSingleton<IRule<DbConnection, UpdateRequest, RowSet>, UpdateRule>()
				.AddSingleton<IRule<DbConnection, UpdateRequest, string>, UpdateRule>();
	}
}
