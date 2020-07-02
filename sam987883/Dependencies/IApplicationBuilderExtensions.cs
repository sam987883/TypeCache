// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using sam987883.Web.Middleware;

namespace sam987883.Dependencies
{
	public static class IApplicationBuilderExtensions
	{
		private static readonly PathString DefaultRoot = new PathString("/sql-api");

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this, string provider, string connectionString, string? root = null) =>
			@this.UseSqlApiBatchDelete(provider, connectionString, root)
				.UseSqlApiBatchInsert(provider, connectionString, root)
				.UseSqlApiBatchUpdate(provider, connectionString, root)
				.UseSqlApiBatchUpsert(provider, connectionString, root)
				.UseSqlApiDelete(provider, connectionString, root)
				.UseSqlApiSelect(provider, connectionString, root)
				.UseSqlApiUpdate(provider, connectionString, root);

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiBatchDelete(this IApplicationBuilder @this, string provider, string connectionString, PathString? root = null)
		{
			var path = (root ?? DefaultRoot) + new PathString("/batch-delete");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<BatchDeleteMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiBatchInsert(this IApplicationBuilder @this, string provider, string connectionString, PathString? root = null)
		{
			var path = (root ?? DefaultRoot) + new PathString("/batch-insert");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<BatchInsertMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiBatchUpdate(this IApplicationBuilder @this, string provider, string connectionString, PathString? root = null)
		{
			var path = (root ?? DefaultRoot) + new PathString("/batch-update");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<BatchUpdateMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiBatchUpsert(this IApplicationBuilder @this, string provider, string connectionString, PathString? root = null)
		{
			var path = (root ?? DefaultRoot) + new PathString("/batch-upsert");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<BatchUpsertMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiDelete(this IApplicationBuilder @this, string provider, string connectionString, PathString? root = null)
		{
			var path = (root ?? DefaultRoot) + new PathString("/delete");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<DeleteMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiSelect(this IApplicationBuilder @this, string provider, string connectionString, PathString? root = null)
		{
			var path = (root ?? DefaultRoot) + new PathString("/select");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<SelectMiddleware>(provider, connectionString));
		}

		/// <summary>
		/// Be sure to register your provider before calling this method.
		/// For example, if using Nuget Package [Microsoft.Data.SqlClient], register the provider:
		/// DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
		/// </summary>
		/// <param name="provider">Database provider</param>
		/// <param name="connectionString">Database connection String</param>
		/// <param name="root">[/sql-api] by default</param>
		/// <returns>IApplicationBuilder</returns>
		public static IApplicationBuilder UseSqlApiUpdate(this IApplicationBuilder @this, string provider, string connectionString, PathString? root = null)
		{
			var path = (root ?? DefaultRoot) + new PathString("/update");
			return @this.MapWhen(context => context.Request.Path.Equals(path), appBuilder =>
				appBuilder.UseMiddleware<UpdateMiddleware>(provider, connectionString));
		}
	}
}
