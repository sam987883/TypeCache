// Copyright (c) 2021 Samuel Abraham

using System.ComponentModel;
using System.Data;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Web.Handlers;

internal static class SqlApiHandler
{
	extension(HttpContext @this)
	{
		private ObjectSchema ObjectSchema
			=> (ObjectSchema)@this.Items[nameof(ObjectSchema)]!;
	}

	public static async Task<IResult> ExecuteProcedure(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the stored procedure.")] string procedure)
	{
		var objectSchema = httpContext.ObjectSchema;
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(procedure);
		sqlCommand.Type = CommandType.StoredProcedure;

		foreach (var parameter in objectSchema.Parameters.Where(parameter =>
			parameter.Direction is ParameterDirection.Input || parameter.Direction is ParameterDirection.InputOutput))
		{
			if (httpContext.Request.Query.TryGetValue(parameter.Name, out var values))
				sqlCommand.Parameters.Add(parameter.Name, values.First());
		}

		var response = await mediator.Request<ValueTask<DataSet>>().Send(sqlCommand, httpContext.RequestAborted);

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
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to delete data from.")] string table
		, [FromQuery][Description("The comma delimited columns to return (SQL syntax).")] string output
		, [FromQuery][Description("The DELETE statement WHERE clause (SQL syntax).")] string where)
	{
		var objectSchema = httpContext.ObjectSchema;
		var sql = objectSchema.CreateDeleteSQL(where, [output]);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		if (output.IsNotBlank)
		{
			var response = await mediator.Request<ValueTask<JsonArray>>().Send(sqlCommand, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			await mediator.Dispatch(sqlCommand, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> DeleteTableValues(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to delete data from.")] string table
		, [FromQuery][Description("The comma delimited columns to return (SQL syntax).")] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.ObjectSchema;
		var sql = objectSchema.CreateDeleteSQL(data, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		if (output.IsNotBlank)
		{
			var response = await mediator.Request<ValueTask<JsonArray>>().Send(sqlCommand, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			await mediator.Dispatch(sqlCommand, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> InsertTable(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to insert data into.")] string table
		, [FromQuery][Description("The INSERT statement COLUMNS clause comma delimited list of columns to insert data into (SQL syntax).")] string columns
		, [FromQuery][Description("The INSERT statement OUTPUT clause comma delimited list of columns/expressions (SQL syntax).")] string? output
		, SelectParameter selectParameter
		, [FromQuery][Description("SELECT [DISTINCT(...)].")] string? distinct
		, [FromQuery][Description("FETCH NEXT {fetch} ROWS ONLY.")] uint? fetch
		, [FromQuery][Description("The SELECT statement FROM caluse specifying the table/view/user defined function to pull data to be inserted (SQL syntax).")] string from
		, [FromQuery(Name = "group-by")][Description("The GROUP BY clause comma delimited list of columns/expressions (SQL syntax).")] string? groupBy
		, [FromQuery][Description("The SELECT statement's GROUP BY ... HAVING clause (SQL syntax).")] string? having
		, [FromQuery][Description("The SELECT statement FROM clause table hints (SQL syntax).")] string? hints
		, [FromQuery][Description("OFFSET {offset} ROWS.")] uint? offset
		, [FromQuery(Name = "order-by")][Description("The SELECT statement ORDER BY clause (SQL syntax).")] string? orderBy
		, [FromQuery][Description("The SELECT statement comma delimted list of columns/expressions (SQL syntax).")] string select
		, [FromQuery][Description("The SELECT statement TOP clause value, either a number (ie. 10000) or a percentage (ie. 50%) (SQL syntax).")] string? top
		, [FromQuery][Description("The SELECT statement WHERE clause (SQL syntax).")] string where)
	{
		var objectSchema = httpContext.ObjectSchema;
		var sql = objectSchema.CreateInsertSQL(columns.SplitEx(','), selectParameter, output?.SplitEx(',') ?? []);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		if (output.IsNotBlank)
		{
			var response = await mediator.Request<ValueTask<JsonArray>>().Send(sqlCommand, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			await mediator.Dispatch(sqlCommand, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> InsertTableValues(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to insert data into.")] string table
		, [FromQuery][Description("The INSERT statement OUTPUT clause comma delimited list of columns/expressions (SQL syntax).")] string? output
		, [FromBody][Description("The data to insert.")] JsonArray data)
	{
		var objectSchema = httpContext.ObjectSchema;
		var sql = objectSchema.CreateInsertSQL(data, output is not null ? [output] : []);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		if (output.IsNotBlank)
		{
			var response = await mediator.Request<ValueTask<JsonArray>>().Send(sqlCommand, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			await mediator.Dispatch(sqlCommand, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> SelectData(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to retrieve data from.")] string table
		, SelectParameter selectParameter
		, [FromQuery][Description("SELECT [DISTINCT(...)].")] string? distinct
		, [FromQuery][Description("FETCH NEXT {fetch} ROWS ONLY.")] uint? fetch
		, [FromQuery][Description("The GROUP BY clause comma delimited list of columns/expressions (SQL syntax).")] string? groupBy
		, [FromQuery(Name = "group-by")][Description("The SELECT statement's GROUP BY ... HAVING clause (SQL syntax).")] string? having
		, [FromQuery][Description("The SELECT statement FROM clause table hints (SQL syntax).")] string? hints
		, [FromQuery][Description("OFFSET {offset} ROWS.")] uint? offset
		, [FromQuery(Name = "order-by")][Description("The SELECT statement ORDER BY clause (SQL syntax).")] string? orderBy
		, [FromQuery][Description("The SELECT statement comma delimted list of columns/expressions (SQL syntax).")] string select
		, [FromQuery][Description("The SELECT statement TOP clause value, either a number (ie. 10000) or a percentage (ie. 50%) (SQL syntax).")] string? top
		, [FromQuery][Description("The SELECT statement WHERE clause (SQL syntax).")] string where)
	{
		var objectSchema = httpContext.ObjectSchema;
		if (select.IsBlank)
			return Results.BadRequest(Invariant($"[{nameof(select)}] query parameter must be specified."));

		var sql = objectSchema.CreateSelectSQL(selectParameter);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		var response = await mediator.Request<ValueTask<JsonArray>>().Send(sqlCommand, httpContext.RequestAborted);
		return Results.Ok(response);
	}

	public static async Task<IResult> UpdateTable(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to update its data.")] string table
		, [FromQuery][Description("The UPDATE statement OUTPUT clause comma delimited list of columns/expressions (SQL syntax).")] string? output
		, [FromQuery][Description("The UPDATE statement SET clause comma delimited list of columns=expressions (SQL syntax).")] string set
		, [FromQuery][Description("The UPDATE statement WHERE clause (SQL syntax).")] string where)
	{
		var objectSchema = httpContext.ObjectSchema;
		var sql = objectSchema.CreateUpdateSQL(set.SplitEx(','), where, output is not null ? [output] : []);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		if (output.IsNotBlank)
		{
			var response = await mediator.Request<ValueTask<JsonArray>>().Send(sqlCommand, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			await mediator.Dispatch(sqlCommand, httpContext.RequestAborted);
			return Results.Ok();
		}
	}

	public static async Task<IResult> UpdateTableValues(
		HttpContext httpContext
		, IMediator mediator
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to update its data.")] string table
		, [FromQuery][Description("The UPDATE statement OUTPUT clause comma delimited list of columns/expressions (SQL syntax).")] string? output
		, [FromBody][Description("The data used for updating the table.")] JsonArray data)
	{
		var objectSchema = httpContext.ObjectSchema;
		var sql = objectSchema.CreateUpdateSQL(data, output is not null ? [output] : []);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		if (output.IsNotBlank)
		{
			var response = await mediator.Request<ValueTask<JsonArray>>().Send(sqlCommand, httpContext.RequestAborted);
			return Results.Ok(response);
		}
		else
		{
			await mediator.Dispatch(sqlCommand, httpContext.RequestAborted);
			return Results.Ok();
		}
	}
}
