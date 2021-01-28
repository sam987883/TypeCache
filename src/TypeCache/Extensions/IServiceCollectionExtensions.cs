// Copyright(c) 2020 Samuel Abraham

using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Data.Business;
using TypeCache.Reflection;
using TypeCache.Reflection.Mappers;
using TypeCache.Security;

namespace TypeCache.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IFieldMapper&lt;FROM, TO&gt;"/></term> <description>Field mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterFieldMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class
			=> @this.AddSingleton<IFieldMapper<FROM, TO>>(provider => new FieldMapper<FROM, TO>(overrides));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="DefaultProcessHandler&lt;T&gt;"/>, <see cref="DefaultProcessHandler&lt;M,T&gt;"/></term> <description>Default implementation of IProcessHandler.</description></item>
		/// <item><term><see cref="DefaultRuleHandler&lt;T,R&gt;"/>, <see cref="DefaultRuleHandler&lt;M,T,R&gt;"/></term> <description>Default implementation of IRuleHandler.</description></item>
		/// <item><term><see cref="DefaultRulesHandler&lt;T,R&gt;"/>, <see cref="DefaultRulesHandler&lt;M,T,R&gt;"/></term> <description>Default implementation of IRulesHandler.</description></item>
		/// </list>
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static IServiceCollection RegisterMediator(this IServiceCollection @this)
			=> @this.AddSingleton<IMediator, Mediator>()
				.AddSingleton(typeof(DefaultProcessHandler<>), typeof(DefaultProcessHandler<>))
				.AddSingleton(typeof(DefaultProcessHandler<,>), typeof(DefaultProcessHandler<,>))
				.AddSingleton(typeof(DefaultRuleHandler<,>), typeof(DefaultRuleHandler<,>))
				.AddSingleton(typeof(DefaultRuleHandler<,,>), typeof(DefaultRuleHandler<,,>))
				.AddSingleton(typeof(DefaultRulesHandler<,>), typeof(DefaultRulesHandler<,>))
				.AddSingleton(typeof(DefaultRulesHandler<,,>), typeof(DefaultRulesHandler<,,>));

		/// <summary>
		/// Registers Singleton:
		/// <list type="bullet">
		/// <item><term><see cref="IPropertyMapper&lt;FROM, TO&gt;"/></term> <description>Property mapper where types: FROM &lt;&gt; TO.</description></item>
		/// </list>
		/// </summary>
		public static IServiceCollection RegisterPropertyMapper<FROM, TO>(this IServiceCollection @this, params MapperSetting[] overrides)
			where FROM : class
			where TO : class
			=> @this.AddSingleton<IPropertyMapper<FROM, TO>>(provider => new PropertyMapper<FROM, TO>(overrides));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IHashMaker"/></term> <description>Utility class that encrypts a long to a simple string hashed ID and back.</description></item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random 16 bytes</param>
		/// <param name="rgbIV">Any random 16 bytes</param>
		public static IServiceCollection RegisterSecurity(this IServiceCollection @this, byte[] rgbKey, byte[] rgbIV)
		{
			@this.TryAddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));
			return @this;
		}

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IHashMaker"/></term> <description>Utility class that encrypts a long to a simple string hashed ID and back.</description></item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random decimal value (gets cionverted to 16 byte array)</param>
		/// <param name="rgbIV">Any random decimal value (gets cionverted to 16 byte array)</param>
		public static IServiceCollection RegisterSecurity(this IServiceCollection @this, decimal rgbKey, decimal rgbIV)
		{
			@this.TryAddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));
			return @this;
		}

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
