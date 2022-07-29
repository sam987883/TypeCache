// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Attributes;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Security;

namespace TypeCache.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers singleton <c><see cref="DataSource"/></c> to be available via: <c>IAccessor&lt;<see cref="DataSource"/>&gt;</c><br/>
	/// <i><b>Requires call to:</b></i>
	/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
	/// </summary>
	public static IServiceCollection RegisterDataSource(this IServiceCollection @this, string name, string databaseProvider, string connectionString)
		=> @this.AddSingleton<DataSource>(new DataSource(name, databaseProvider, connectionString));

	/// <summary>
	/// Provides data source related information: <c>IAccessor&lt;<see cref="DataSource"/>&gt;</c><br/>
	/// <i><b>Requires call to:</b></i>
	/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
	/// </summary>
	public static IServiceCollection RegisterDataSourceAccessor(this IServiceCollection @this)
		=> @this.AddSingleton<IAccessor<DataSource>>(provider => new DataSourceAccessor(provider.GetServices<DataSource>().ToArray()));

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
	/// Registers Singleton Rules and RuleHandlers for the SQL API Commands.<br/>
	/// You can implement the following validation rules to validate the request before the database call is made:
	/// <c>
	/// <list type="table">
	/// <item>IValidationRule&lt;<see cref="StoredProcedureCommand" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="DeleteDataCommand{T}" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="DeleteCommand" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="InsertDataCommand{T}" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="InsertCommand" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="SelectCommand" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="UpdateDataCommand{T}" />&gt;</item>
	/// <item>IValidationRule&lt;<see cref="UpdateCommand" />&gt;</item>
	/// </list>
	/// </c>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="RegisterMediator(IServiceCollection)"/><br/>
	/// <see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/>
	/// </code>
	/// </summary>
	public static IServiceCollection RegisterSqlApiRules(this IServiceCollection @this, Assembly? typeAssembly)
	{
		@this.AddSingleton<IValidationRule<SchemaRequest>, SchemaValidationRule>()
			.AddSingleton<IValidationRule<CountCommand>, CountValidationRule>()
			.AddSingleton<IValidationRule<StoredProcedureCommand>, StoredProcedureValidationRule>()
			.AddSingleton<IRule<SchemaRequest, ObjectSchema>, SchemaRule>()
			.AddSingleton<IRule<SchemaRequest, string>, SchemaRule>()
			.AddSingleton<IRule<StoredProcedureCommand, object>, StoredProcedureRule>()
			.AddSingleton<IRule<CountCommand, long>, CountRule>()
			.AddSingleton<IRule<CountCommand, string>, CountRule>()
			.AddSingleton<IRule<ExecuteCommands, object>, ExecuteCommandsRule>()
			.AddSingleton<IRule<SqlCommand, object>, ExecuteSqlRule>();

		if (typeAssembly is not null)
		{
			@this.AddSingleton<IValidationRule<DeleteCommand>, DeleteValidationRule>()
				.AddSingleton<IValidationRule<InsertCommand>, InsertValidationRule>()
				.AddSingleton<IValidationRule<SelectCommand>, SelectValidationRule>()
				.AddSingleton<IValidationRule<UpdateCommand>, UpdateValidationRule>();

			typeAssembly.DefinedTypes.If(type => type.IsDefined(typeof(SqlApiAttribute), false)).Do(type =>
			{
				var deleteDataCommandType = typeof(DeleteDataCommand<>).MakeGenericType(type);
				var insertDataCommandType = typeof(InsertDataCommand<>).MakeGenericType(type);
				var updateDataCommandType = typeof(UpdateDataCommand<>).MakeGenericType(type);

				var rowSetResponseType = typeof(RowSetResponse<>).MakeGenericType(type);
				var updateRowSetResponseType = typeof(UpdateRowSetResponse<>).MakeGenericType(type);

				var deleteDataRuleType = typeof(DeleteDataRule<>).MakeGenericType(type);
				var deleteRuleType = typeof(DeleteRule<>).MakeGenericType(type);
				var insertDataRuleType = typeof(InsertDataRule<>).MakeGenericType(type);
				var insertRuleType = typeof(InsertRule<>).MakeGenericType(type);
				var selectRuleType = typeof(SelectRule<>).MakeGenericType(type);
				var updateDataRuleType = typeof(UpdateDataRule<>).MakeGenericType(type);
				var updateRuleType = typeof(UpdateRule<>).MakeGenericType(type);

				@this.AddSingleton(typeof(IValidationRule<>).MakeGenericType(deleteDataCommandType), typeof(DeleteDataValidationRule<>).MakeGenericType(type))
					.AddSingleton(typeof(IValidationRule<>).MakeGenericType(insertDataCommandType), typeof(InsertDataValidationRule<>).MakeGenericType(type))
					.AddSingleton(typeof(IValidationRule<>).MakeGenericType(updateDataCommandType), typeof(UpdateDataValidationRule<>).MakeGenericType(type))
					.AddSingleton(typeof(IRule<,>).MakeGenericType(deleteDataCommandType, rowSetResponseType), deleteDataRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(deleteDataCommandType, typeof(string)), deleteDataRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(typeof(DeleteCommand), rowSetResponseType), deleteRuleType)
					.AddSingleton(typeof(IRule<DeleteCommand, string>), deleteRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(insertDataCommandType, rowSetResponseType), insertDataRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(insertDataCommandType, typeof(string)), insertDataRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(typeof(InsertCommand), rowSetResponseType), insertRuleType)
					.AddSingleton(typeof(IRule<InsertCommand, string>), insertRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(typeof(SelectCommand), rowSetResponseType), selectRuleType)
					.AddSingleton(typeof(IRule<SelectCommand, string>), selectRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(updateDataCommandType, updateRowSetResponseType), updateDataRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(updateDataCommandType, typeof(string)), updateDataRuleType)
					.AddSingleton(typeof(IRule<,>).MakeGenericType(typeof(UpdateCommand), updateRowSetResponseType), updateRuleType)
					.AddSingleton(typeof(IRule<UpdateCommand, string>), updateRuleType);
			});
		}

		return @this;
	}
}
