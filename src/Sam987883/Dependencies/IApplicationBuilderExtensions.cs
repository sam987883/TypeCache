// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Sam987883.Web.Middleware;
using System;

namespace Sam987883.Dependencies
{
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection string</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this, string provider, string connectionString) =>
			@this.UseSqlApiMerge(provider, connectionString, null)
				.UseSqlApiCall(provider, connectionString, null)
				.UseSqlApiDelete(provider, connectionString, null)
				.UseSqlApiSelect(provider, connectionString, null)
				.UseSqlApiUpdate(provider, connectionString, null);

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection string</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiTestSQL(this IApplicationBuilder @this, string provider, string connectionString) =>
			@this.UseSqlApiMergeSQL(provider, connectionString, null)
				.UseSqlApiDeleteSQL(provider, connectionString, null)
				.UseSqlApiSchemaSQL(null)
				.UseSqlApiSelectSQL(provider, connectionString, null)
				.UseSqlApiUpdateSQL(provider, connectionString, null);

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/call</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiCall(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/call");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<BatchMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// COMING SOON...  
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/graph-ql</code></param>
		/// <returns></returns>
		public static IApplicationBuilder UseSqlApiGraphQL(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			throw new NotImplementedException("COMING SOON...");
			//path ??= new PathString("/sql-api/graph-ql");
			//return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
			//	appBuilder.UseMiddleware<GraphMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/delete</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiDelete(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/delete");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<DeleteMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/delete/sql</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiDeleteSQL(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/delete/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<DeleteSQLMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/merge</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiMerge(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/merge");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<BatchMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/merge/sql</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiMergeSQL(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/merge/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<BatchSQLMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/schema</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiSchema(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/schema");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SchemaMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Gets the SQL used to retrieve schema information about database objects.
		/// </summary>
		/// <param name="path">Default: <code>/sql-api/schema/sql</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiSchemaSQL(this IApplicationBuilder @this, PathString? path = null)
		{
			path ??= new PathString("/sql-api/schema/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SchemaSQLMiddleware>());
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/select</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiSelect(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/select");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SelectMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/select/sql</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiSelectSQL(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/select/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SelectSQLMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/update</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiUpdate(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/update");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<UpdateMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package <b>Microsoft.Data.SqlClient</b>, register the provider:
		/// <code>DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);</code>
		/// </summary>
		/// <param name="provider">Database provider ie. <b>Microsoft.Data.SqlClient</b></param>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="path">Default: <code>/sql-api/update/sql</code></param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiUpdateSQL(this IApplicationBuilder @this, string provider, string connectionString, PathString? path = null)
		{
			path ??= new PathString("/sql-api/update/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<UpdateSQLMiddleware>(provider, connectionString));
		}
	}
}
