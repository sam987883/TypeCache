// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.Web.Middleware;

namespace TypeCache.Web.Extensions
{
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="number">
		/// <item><term>/sql-api/call</term> <see cref="StoredProcedureMiddleware"/></item>
		/// <item><term>/sql-api/count</term> <see cref="CountMiddleware"/></item>
		/// <item><term>/sql-api/delete</term> <see cref="DeleteMiddleware"/></item>
		/// <item><term>/sql-api/deletedata</term> <see cref="DeleteDataMiddleware"/></item>
		/// <item><term>/sql-api/insert</term> <see cref="InsertMiddleware"/></item>
		/// <item><term>/sql-api/insertdata</term> <see cref="InsertDataMiddleware"/></item>
		/// <item><term>/sql-api/select</term> <see cref="SelectMiddleware"/></item>
		/// <item><term>/sql-api/update</term> <see cref="UpdateMiddleware"/></item>
		/// <item><term>/sql-api/updatedata</term> <see cref="UpdateDataMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this)
			=> @this.UseSqlApiCall()
				.UseSqlApiCount()
				.UseSqlApiDelete()
				.UseSqlApiDeleteData()
				.UseSqlApiInsert()
				.UseSqlApiInsertData()
				.UseSqlApiSelect()
				.UseSqlApiUpdate()
				.UseSqlApiUpdateData();

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="number">
		/// <item><term>/sql-api/schema/sql</term> <see cref="SchemaSqlMiddleware"/></item>
		/// <item><term>/sql-api/count/sql</term> <see cref="CountSqlMiddleware"/></item>
		/// <item><term>/sql-api/delete/sql</term> <see cref="DeleteSqlMiddleware"/></item>
		/// <item><term>/sql-api/deletedata/sql</term> <see cref="DeleteDataSqlMiddleware"/></item>
		/// <item><term>/sql-api/insert/sql</term> <see cref="InsertSqlMiddleware"/></item>
		/// <item><term>/sql-api/insertdata/sql</term> <see cref="InsertDataSqlMiddleware"/></item>
		/// <item><term>/sql-api/select/sql</term> <see cref="SelectSqlMiddleware"/></item>
		/// <item><term>/sql-api/update/sql</term> <see cref="UpdateSqlMiddleware"/></item>
		/// <item><term>/sql-api/updatedata/sql</term> <see cref="UpdateDataSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiTestSQL(this IApplicationBuilder @this)
			=> @this.UseSqlApiSchemaSQL()
				.UseSqlApiCountSQL()
				.UseSqlApiDeleteSQL()
				.UseSqlApiDeleteDataSQL()
				.UseSqlApiInsertSQL()
				.UseSqlApiInsertDataSQL()
				.UseSqlApiSelectSQL()
				.UseSqlApiUpdateSQL()
				.UseSqlApiUpdateDataSQL();

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/call</term> <see cref="StoredProcedureMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiCallRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiCall(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/call");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<StoredProcedureMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/count</term> <see cref="CountMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDeleteDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiCount(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/count");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<CountMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/count/sql</term> <see cref="CountSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDeleteDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiCountSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/count/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<CountSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/deletedata</term> <see cref="DeleteDataMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDeleteDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDeleteData(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/deletedata");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<DeleteDataMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/deletedata/sql</term> <see cref="DeleteDataSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDeleteDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDeleteDataSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/deletedata/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<DeleteDataSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/delete</term> <see cref="DeleteMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDeleteRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDelete(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/delete");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<DeleteMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/delete/sql</term> <see cref="DeleteSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiDeleteRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiDeleteSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/delete/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<DeleteSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/insertdata</term> <see cref="InsertDataMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiInsertDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsertData(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insertdata");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<InsertDataMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/insertdata/sql</term> <see cref="InsertDataSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiInsertDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsertDataSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insertdata/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<InsertDataSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/insert</term> <see cref="InsertMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiInsertRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsert(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insert");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<InsertMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/insert/sql</term> <see cref="InsertSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiInsertRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiInsertSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/insert/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<InsertSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/schema?dataSource={dataSource}&amp;object={object}</term> <see cref="SchemaMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="IServiceCollectionExtensions.RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSchema(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/schema");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<SchemaMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/schema/sql</term> <see cref="SchemaSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApi(IServiceCollection, IConfigurationSection)"/> or <see cref="IServiceCollectionExtensions.RegisterSqlApi(IServiceCollection, DataSource[])"/></code>
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
		/// <item><term><paramref name="route"/> or /sql-api/select</term> <see cref="SelectMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSelect(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/select");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<SelectMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/select/sql</term> <see cref="SelectSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiSelectSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/select/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<SelectSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/updatedata</term> <see cref="UpdateDataMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiUpdateDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdateData(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/updatedata");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<UpdateDataMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/updatedata/sql</term> <see cref="UpdateDataSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiUpdateDataRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdateDataSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/updatedata/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<UpdateDataSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/update</term> <see cref="UpdateMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiUpdateRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdate(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/update");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<UpdateMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/update/sql</term> <see cref="UpdateSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiUpdateRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiUpdateSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/update/sql");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<UpdateSqlMiddleware>());
		}

		/// <summary>
		/// Maps Routes to Middlewares:
		/// <list type="table">
		/// <item><term><paramref name="route"/> or /sql-api/execute</term> <see cref="ExecuteSqlMiddleware"/></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="IServiceCollectionExtensions.RegisterSqlApiExecuteSqlRules"/></code>
		/// </summary>
		public static IApplicationBuilder UseSqlApiExecuteSQL(this IApplicationBuilder @this, string? route = null)
		{
			var path = new PathString(!route.IsBlank() ? route : "/sql-api/execute");
			return @this.MapWhen(context => context.Request.Path.Equals(path),
				_ => _.UseMiddleware<ExecuteSqlMiddleware>());
		}
	}
}
