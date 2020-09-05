// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Sam987883.Common.Extensions;
using Sam987883.Web.Middleware;

namespace Sam987883.Web.Extenstions
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
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this, string databaseProvider, string connectionString) =>
			@this.UseSqlApiCall(databaseProvider, connectionString, null)
				.UseSqlApiDelete(databaseProvider, connectionString, null)
				.UseSqlApiInsert(databaseProvider, connectionString, null)
				.UseSqlApiMerge(databaseProvider, connectionString, null)
				.UseSqlApiSelect(databaseProvider, connectionString, null)
				.UseSqlApiUpdate(databaseProvider, connectionString, null);

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="number">
		/// <item><term>/sql-api/schema/sql</term> <description><see cref="SchemaSQLMiddleware"/></description></item>
		/// <item><term>/sql-api/delete/sql</term> <description><see cref="DeleteSQLMiddleware"/></description></item>
		/// <item><term>/sql-api/insert/sql</term> <description><see cref="InsertSQLMiddleware"/></description></item>
		/// <item><term>/sql-api/merge/sql</term> <description><see cref="MergeSQLMiddleware"/></description></item>
		/// <item><term>/sql-api/select/sql</term> <description><see cref="SelectSQLMiddleware"/></description></item>
		/// <item><term>/sql-api/update/sql</term> <description><see cref="UpdateSQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiTestSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString) =>
			@this.UseSqlApiSchemaSQL(null)
				.UseSqlApiDeleteSQL(databaseProvider, connectionString, null)
				.UseSqlApiInsertSQL(databaseProvider, connectionString, null)
				.UseSqlApiMergeSQL(databaseProvider, connectionString, null)
				.UseSqlApiSelectSQL(databaseProvider, connectionString, null)
				.UseSqlApiUpdateSQL(databaseProvider, connectionString, null);

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/sql</term> <description><see cref="SQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SQLMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/call</term> <description><see cref="StoredProcedureMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiCall(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/call");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<StoredProcedureMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/delete</term> <description><see cref="DeleteMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDelete(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/delete");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<DeleteMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/delete/sql</term> <description><see cref="DeleteSQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDeleteSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/delete/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<DeleteSQLMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/insert</term> <description><see cref="InsertMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsert(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insert");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<InsertMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/insert/sql</term> <description><see cref="InsertSQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsertSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insert/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<InsertSQLMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/merge</term> <description><see cref="MergeMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiMerge(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/merge");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<MergeMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/merge/sql</term> <description><see cref="MergeSQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiMergeSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/merge/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<MergeSQLMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/schema</term> <description><see cref="SchemaMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSchema(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/schema");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SchemaMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/schema/sql</term> <description><see cref="SchemaSQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSchemaSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/schema/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SchemaSQLMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/select</term> <description><see cref="SelectMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSelect(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/select");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SelectMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/select/sql</term> <description><see cref="SelectSQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSelectSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/select/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SelectSQLMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/update</term> <description><see cref="UpdateMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdate(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/update");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<UpdateMiddleware>(databaseProvider, connectionString));
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term>route or /sql-api/update/sql</term> <description><see cref="UpdateSQLMiddleware"/></description></item>
		/// </list>
		/// <i>Requires a call to:</i>
		/// <code><see cref="Database.Extensions.IServiceCollectionExtensions.RegisterDatabaseProviderFactory"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdateSQL(this IApplicationBuilder @this, string databaseProvider, string connectionString, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/update/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<UpdateSQLMiddleware>(databaseProvider, connectionString));
		}
	}
}
