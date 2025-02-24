// Copyright (c) 2021 Samuel Abraham

using System.ComponentModel;
using System.Data;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Data;
using TypeCache.Data.Mediation;
using TypeCache.Extensions;
using TypeCache.Mediation;
using static System.Net.Mime.MediaTypeNames;
using static System.StringSplitOptions;
using static System.Text.Encoding;

namespace TypeCache.Web.Handlers;

internal static class SqlApiHandler
{
	private static IDataSource GetDataSource(this HttpContext @this)
		=> (IDataSource)@this.Items[nameof(IDataSource)]!;

	private static ObjectSchema GetObjectSchema(this HttpContext @this)
		=> (ObjectSchema)@this.Items[nameof(ObjectSchema)]!;

	public static async Task<IResult> ExecuteProcedure(
		HttpContext httpContext
		, IMediator mediator
		, [Description("The name of the connection string from appSettings.")][FromRoute(Name = "dataSource")] string dataSourceName
		, [Description("The name of the database.")][FromRoute] string database
		, [Description("The name of the database object schema.")][FromRoute] string schema
		, [Description("The name of the stored procedure.")][FromRoute] string procedure)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(procedure);
		sqlCommand.Type = CommandType.StoredProcedure;

		foreach (var parameter in objectSchema.Parameters.Where(parameter =>
			parameter.Direction is ParameterDirection.Input || parameter.Direction is ParameterDirection.InputOutput))
		{
			if (httpContext.Request.Query.TryGetValue(parameter.Name, out var values))
				sqlCommand.Parameters.Add(parameter.Name, values.First());
		}

		var request = new SqlDataSetRequest { Command = sqlCommand };
		var response = await mediator.Map(request, httpContext.RequestAborted);

		foreach (var parameter in objectSchema.Parameters.Where(parameter =>
			parameter.Direction is ParameterDirection.InputOutput || parameter.Direction is ParameterDirection.Output))
		{
			if (sqlCommand.Parameters.TryGetValue(parameter.Name, out var value))
				httpContext.Items[parameter.Name] = value;
		}

		return Results.Ok(response);
	}

	public static async Task<IResult> DeleteTable(
		HttpContext httpContext
		, IMediator mediator
		, [Description("The name of the connection string from appSettings.")][FromRoute(Name = "dataSource")] string dataSourceName
		, [Description("The name of the database.")][FromRoute] string database
		, [Description("The name of the database object schema.")][FromRoute] string schema
		, [Description("The name of the database table to delete data from.")][FromRoute] string table
		, [Description("The comma delimited columns to return (SQL syntax).")][FromQuery] string output
		, [Description("The DELETE statement WHERE clause (SQL syntax).")][FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateDeleteSQL(where, [output]);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		if (output.IsNotBlank())
		{
			var request = new SqlJsonArrayRequest { Command = sqlCommand };
			var response = await mediator.Map(request, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.Execute(request, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> DeleteTableValues(
		HttpContext httpContext
		, IMediator mediator
		, [Description("The name of the connection string from appSettings.")][FromRoute(Name = "dataSource")] string dataSourceName
		, [Description("The name of the database.")][FromRoute] string database
		, [Description("The name of the database object schema.")][FromRoute] string schema
		, [Description("The name of the database table to delete data from.")][FromRoute] string table
		, [Description("The comma delimited columns to return (SQL syntax).")][FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateDeleteSQL(data, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		if (output.IsNotBlank())
		{
			var request = new SqlJsonArrayRequest { Command = sqlCommand };
			var response = await mediator.Map(request, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.Execute(request, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> InsertTable(
		HttpContext httpContext
		, IMediator mediator
		, [Description("The name of the connection string from appSettings.")][FromRoute(Name = "dataSource")] string dataSourceName
		, [Description("The name of the database.")][FromRoute] string database
		, [Description("The name of the database object schema.")][FromRoute] string schema
		, [Description("The name of the database table to insert data into.")][FromRoute] string table
		, [Description("The INSERT statement COLUMNS clause comma delimited list of columns to insert data into (SQL syntax).")][FromQuery] string columns
		, [Description("The INSERT statement OUTPUT clause comma delimited list of columns/expressions (SQL syntax).")][FromQuery] string? output
		, SelectParameter selectParameter
		, [Description("SELECT [DISTINCT(...)].")][FromQuery] string? distinct
		, [Description("FETCH NEXT {fetch} ROWS ONLY.")][FromQuery] uint? fetch
		, [Description("The SELECT statement FROM caluse specifying the table/view/user defined function to pull data to be inserted (SQL syntax).")][FromQuery] string from
		, [Description("The GROUP BY clause comma delimited list of columns/expressions (SQL syntax).")][FromQuery(Name = "group-by")] string? groupBy
		, [Description("The SELECT statement's GROUP BY ... HAVING clause (SQL syntax).")][FromQuery] string? having
		, [Description("The SELECT statement FROM clause table hints (SQL syntax).")][FromQuery] string? hints
		, [Description("OFFSET {offset} ROWS.")][FromQuery] uint? offset
		, [Description("The SELECT statement ORDER BY clause (SQL syntax).")][FromQuery] string? orderBy
		, [Description("The SELECT statement comma delimted list of columns/expressions (SQL syntax).")][FromQuery] string select
		, [Description("The SELECT statement TOP clause value, either a number (ie. 10000) or a percentage (ie. 50%) (SQL syntax).")][FromQuery] string? top
		, [Description("The SELECT statement WHERE clause (SQL syntax).")][FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateInsertSQL(columns.SplitEx(','), selectParameter, output?.SplitEx(',') ?? []);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		if (output.IsNotBlank())
		{
			var request = new SqlJsonArrayRequest { Command = sqlCommand };
			var response = await mediator.Map(request, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.Execute(request, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> InsertTableValues(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateInsertSQL(data, [output]);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		if (output.IsNotBlank())
		{
			var request = new SqlJsonArrayRequest { Command = sqlCommand };
			var response = await mediator.Map(request, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.Execute(request, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> SelectData(
		HttpContext httpContext
		, IMediator mediator
		, [Description("The name of the connection string from appSettings.")][FromRoute(Name = "dataSource")] string dataSourceName
		, [Description("The name of the database.")][FromRoute] string database
		, [Description("The name of the database object schema.")][FromRoute] string schema
		, [Description("The name of the database table to retrieve data from.")][FromRoute] string table
		, SelectParameter selectParameter
		, [Description("SELECT [DISTINCT(...)].")][FromQuery] string? distinct
		, [Description("FETCH NEXT {fetch} ROWS ONLY.")][FromQuery] uint? fetch
		, [Description("The GROUP BY clause comma delimited list of columns/expressions (SQL syntax).")][FromQuery] string? groupBy
		, [Description("The SELECT statement's GROUP BY ... HAVING clause (SQL syntax).")][FromQuery] string? having
		, [Description("The SELECT statement FROM clause table hints (SQL syntax).")][FromQuery] string? hints
		, [Description("OFFSET {offset} ROWS.")][FromQuery] uint? offset
		, [Description("The SELECT statement ORDER BY clause (SQL syntax).")][FromQuery] string? orderBy
		, [Description("The SELECT statement comma delimted list of columns/expressions (SQL syntax).")][FromQuery] string select
		, [Description("The SELECT statement TOP clause value, either a number (ie. 10000) or a percentage (ie. 50%) (SQL syntax).")][FromQuery] string? top
		, [Description("The SELECT statement WHERE clause (SQL syntax).")][FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		if (select.IsBlank())
			return Results.BadRequest(Invariant($"[{nameof(select)}] query parameter must be specified."));

		var sql = objectSchema.CreateSelectSQL(selectParameter);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		var request = new SqlJsonArrayRequest { Command = sqlCommand };
		var response = await mediator.Map(request, httpContext.RequestAborted);
		return Results.Ok(response);
	}

	public static async Task<IResult> UpdateTable(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromQuery] string set
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateUpdateSQL(set.SplitEx(','), where, [output]);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		if (output.IsNotBlank())
		{
			var request = new SqlJsonArrayRequest { Command = sqlCommand };
			var response = await mediator.Map(request, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.Execute(request, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> UpdateTableValues(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateUpdateSQL(data, [output]);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		if (output.IsNotBlank())
		{
			var request = new SqlJsonArrayRequest { Command = sqlCommand };
			var response = await mediator.Map(request, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			var request = new SqlExecuteRequest { Command = sqlCommand };
			await mediator.Execute(request, httpContext.RequestAborted);
			return Results.Ok();
		}
	}
}
