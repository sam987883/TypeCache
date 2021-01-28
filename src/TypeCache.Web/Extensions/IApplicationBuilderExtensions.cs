// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TypeCache.Extensions;
using TypeCache.Web.Middleware;

namespace TypeCache.Web.Extensions
{
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="number">
		/// <item><term>/sql-api/call</term> <description><see cref="StoredProcedureMiddleware"/></description></item>
		/// <item><term>/sql-api/delete</term> <description><see cref="DeleteMiddleware"/></description></item>
		/// <item><term>/sql-api/insert</term> <description><see cref="InsertMiddleware"/></description></item>
		/// <item><term>/sql-api/merge</term> <description><see cref="MergeMiddleware"/></description></item>
		/// <item><term>/sql-api/select</term> <description><see cref="SelectMiddleware"/></description></item>
		/// <item><term>/sql-api/update</term> <description><see cref="UpdateMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApi"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this, string databaseProvider, string connectionString)
			=> @this.UseSqlApiCall(databaseProvider, connectionString, null)
				.UseSqlApiDelete(databaseProvider, connectionString, null)
				.UseSqlApiInsert(databaseProvider, connectionString, null)
				.UseSqlApiMerge(databaseProvider, connectionString, null)
				.UseSqlApiSelect(databaseProvider, connectionString, null)
				.UseSqlApiUpdate(databaseProvider, connectionString, null);

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="number">
		/// <item><term>/sql-api/schema/sql</term> <description><see cref="SchemaSqlMiddleware"/></description></item>
		/// <item><term>/sql-api/delete/sql</term> <description><see cref="DeleteSqlMiddleware"/></description></item>
		/// <item><term>/sql-api/insert/sql</term> <description><see cref="InsertSqlMiddleware"/></description></item>
		/// <item><term>/sql-api/merge/sql</term> <description><see cref="MergeSqlMiddleware"/></description></item>
		/// <item><term>/sql-api/select/sql</term> <description><see cref="SelectSqlMiddleware"/></description></item>
		/// <item><term>/sql-api/update/sql</term> <description><see cref="UpdateSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApi"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiTestSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString)
			=> @this.UseSqlApiSchemaSQL(null)
				.UseSqlApiDeleteSQL(databaseProvider, connectionString, null)
				.UseSqlApiInsertSQL(databaseProvider, connectionString, null)
				.UseSqlApiMergeSQL(databaseProvider, connectionString, null)
				.UseSqlApiSelectSQL(databaseProvider, connectionString, null)
				.UseSqlApiUpdateSQL(databaseProvider, connectionString, null);

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/sql</term> <description><see cref="ExecuteSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiCall"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<ExecuteSqlMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/call</term> <description><see cref="StoredProcedureMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiCall"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiCall(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/call");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<StoredProcedureMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/delete</term> <description><see cref="DeleteMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDelete"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDelete(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/delete");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<DeleteMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/delete/sql</term> <description><see cref="DeleteSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDelete"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDeleteSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/delete/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<DeleteSqlMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/insert</term> <description><see cref="InsertMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiInsert"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsert(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insert");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<InsertMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/insert/sql</term> <description><see cref="InsertSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiInsert"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsertSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insert/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<InsertSqlMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/merge</term> <description><see cref="MergeMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiMerge"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiMerge(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/merge");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<MergeMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/merge/sql</term> <description><see cref="MergeSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiMerge"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiMergeSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/merge/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<MergeSqlMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/schema</term> <description><see cref="SchemaMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSchema(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/schema");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<SchemaMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/schema/sql</term> <description><see cref="SchemaSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSchemaSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/schema/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<SchemaSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/select</term> <description><see cref="SelectMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiSelect"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSelect(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/select");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<SelectMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/select/sql</term> <description><see cref="SelectSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiSelect"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSelectSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/select/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<SelectSqlMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/update</term> <description><see cref="UpdateMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiUpdate"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdate(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/update");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<UpdateMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/update/sql</term> <description><see cref="UpdateSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiUpdate"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdateSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/update/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<UpdateSqlMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/execute</term> <description><see cref="ExecuteSqlMiddleware"/></description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiExecuteSql"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiExecuteSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/execute");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<ExecuteSqlMiddleware>(databaseProvider, connectionString));
		}
	}
}
