﻿// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;
using static System.FormattableString;
using static System.Net.Mime.MediaTypeNames;
using static System.Text.Encoding;

namespace TypeCache.Web.Handlers;

internal static class SqlApiHandler
{
	private static IDataSource GetDataSource(this HttpContext @this)
		=> (IDataSource)@this.Items[nameof(IDataSource)]!;

	private static ObjectSchema GetObjectSchema(this HttpContext @this)
		=> (ObjectSchema)@this.Items[nameof(ObjectSchema)]!;

	private static IMediator GetMediator(this HttpContext @this)
		=> (IMediator)@this.Items[nameof(IMediator)]!;

	public static async Task<IResult> CallProcedure(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string procedure)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(procedure);
		sqlCommand.Type = CommandType.StoredProcedure;

		foreach (var parameter in objectSchema.Parameters.Where(parameter => parameter.Direction.IsAny(ParameterDirection.Input, ParameterDirection.InputOutput)))
		{
			if (httpContext.Request.Query.TryGetValue(parameter.Name, out var values))
				sqlCommand.Parameters.Add(parameter.Name, values.First()!);
		}

		var mediator = httpContext.GetMediator();
		var result = await mediator.ApplyRuleAsync<SqlCommand, DataSet>(sqlCommand, httpContext.RequestAborted);

		foreach (var parameter in objectSchema.Parameters.Where(parameter => parameter.Direction.IsAny(ParameterDirection.Output, ParameterDirection.InputOutput)))
		{
			if (sqlCommand.Parameters.TryGetValue(parameter.Name, out var value))
				httpContext.Items[parameter.Name] = value;
		}

		return Results.Ok(result);
	}

	public static async Task<IResult> DeleteTable(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateDeleteSQL(where, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		var mediator = httpContext.GetMediator();
		JsonArray? result = null;
		if (output.IsNotBlank())
			result = await mediator.ApplyRuleAsync<SqlCommand, JsonArray>(sqlCommand, httpContext.RequestAborted);
		else
			await mediator.RunProcessAsync<SqlCommand>(sqlCommand, httpContext.RequestAborted);

		return Results.Ok(result);
	}

	public static async Task<IResult> DeleteTableBatch(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateDeleteSQL(data, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		var mediator = httpContext.GetMediator();
		JsonArray? result = null;
		if (output.IsNotBlank())
			result = await mediator.ApplyRuleAsync<SqlCommand, JsonArray>(sqlCommand, httpContext.RequestAborted);
		else
			await mediator.RunProcessAsync<SqlCommand>(sqlCommand, httpContext.RequestAborted);

		return Results.Ok(result);
	}

	public static async Task<IResult> InsertTable(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string columns
		, [FromQuery] string output
		, [FromBody] SelectQuery selectQuery)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateInsertSQL(columns.Split(','), selectQuery, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		var mediator = httpContext.GetMediator();
		JsonArray? result = null;
		if (output.IsNotBlank())
			result = await mediator.ApplyRuleAsync<SqlCommand, JsonArray>(sqlCommand, httpContext.RequestAborted);
		else
			await mediator.RunProcessAsync<SqlCommand>(sqlCommand, httpContext.RequestAborted);

		return Results.Ok(result);
	}

	public static async Task<IResult> InsertTableBatch(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateInsertSQL(data, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		var mediator = httpContext.GetMediator();
		JsonArray? result = null;
		if (output.IsNotBlank())
			result = await mediator.ApplyRuleAsync<SqlCommand, JsonArray>(sqlCommand, httpContext.RequestAborted);
		else
			await mediator.RunProcessAsync<SqlCommand>(sqlCommand, httpContext.RequestAborted);

		return Results.Ok(result);
	}

	public static IResult GetDeleteSQL(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		return Results.Text(objectSchema.CreateDeleteSQL(where, output), Text.Plain, UTF8);
	}

	public static IResult GetDeleteBatchSQL(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		return Results.Text(objectSchema.CreateDeleteSQL(data, output), Text.Plain, UTF8);
	}

	public static IResult GetInsertSQL(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string columns
		, [FromQuery] bool distinct
		, [FromQuery] string distinctOn
		, [FromQuery] uint fetch
		, [FromQuery] string groupBy
		, [FromQuery] GroupBy groupByOption
		, [FromQuery] string having
		, [FromQuery] uint offset
		, [FromQuery] string orderBy
		, [FromQuery] string output
		, [FromQuery] string select
		, [FromQuery] string tableHints
		, [FromQuery] string top
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();

		if (columns.IsBlank())
			return Results.BadRequest(Invariant($"[{nameof(columns)}] query parameter must be specified."));

		if (select.IsBlank())
			return Results.BadRequest(Invariant($"[{nameof(select)}] query parameter must be specified."));

		if (columns.Split(',').Length != select.Split(',').Length)
			return Results.BadRequest(Invariant($"[{nameof(columns)}] and [{nameof(select)}] query parameters must have same number of columns/expressions."));

		var selectQuery = new SelectQuery
		{
			Distinct = distinct,
			DistinctOn = distinctOn,
			Fetch = fetch,
			From = objectSchema.Name,
			GroupBy = groupBy?.Split(','),
			GroupByOption = groupByOption,
			Having = having,
			Offset = offset,
			OrderBy = orderBy?.Split(','),
			Select = select?.Split(','),
			TableHints = tableHints,
			Top = top,
			Where = where
		};
		return Results.Text(objectSchema.CreateInsertSQL(columns.Split(','), selectQuery, output), Text.Plain, UTF8);
	}

	public static IResult GetInsertBatchSQL(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		return Results.Text(objectSchema.CreateInsertSQL(data, output), Text.Plain, UTF8);
	}

	public static async Task<IResult> GetSchema(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] SchemaCollection collection
		, [FromQuery] string? where
		, [FromQuery] string? orderBy)
	{
		var dataSource = httpContext.GetDataSource();
		var table = await dataSource.GetDatabaseSchemaAsync(collection, database, httpContext.RequestAborted);
		return Results.Ok(table?.Select(where, orderBy));
	}

	public static IResult GetSelectSQL(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] bool distinct
		, [FromQuery] string distinctOn
		, [FromQuery] uint fetch
		, [FromQuery] string groupBy
		, [FromQuery] GroupBy groupByOption
		, [FromQuery] string having
		, [FromQuery] uint offset
		, [FromQuery] string orderBy
		, [FromQuery] string select
		, [FromQuery] string tableHints
		, [FromQuery] string top
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		if (select.IsBlank())
			return Results.BadRequest(Invariant($"[{nameof(select)}] query parameter must be specified."));

		var selectQuery = new SelectQuery
		{
			Distinct = distinct,
			DistinctOn = distinctOn,
			Fetch = fetch,
			From = objectSchema.Name,
			GroupBy = groupBy?.Split(','),
			GroupByOption = groupByOption,
			Having = having,
			Offset = offset,
			OrderBy = orderBy?.Split(','),
			Select = select?.Split(','),
			TableHints = tableHints,
			Top = top,
			Where = where
		};
		return Results.Text(objectSchema.CreateSelectSQL(selectQuery), Text.Plain, UTF8);
	}

	public static IResult GetUpdateSQL(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromQuery] string set
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		return Results.Text(objectSchema.CreateUpdateSQL(set.Split(','), where, output), Text.Plain, UTF8);
	}

	public static IResult GetUpdateBatchSQL(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		return Results.Text(objectSchema.CreateUpdateSQL(data, output), Text.Plain, UTF8);
	}

	public static async Task<IResult> Select(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] bool distinct
		, [FromQuery] string distinctOn
		, [FromQuery] uint fetch
		, [FromQuery] string groupBy
		, [FromQuery] GroupBy groupByOption
		, [FromQuery] string having
		, [FromQuery] uint offset
		, [FromQuery] string orderBy
		, [FromQuery] string select
		, [FromQuery] string tableHints
		, [FromQuery] string top
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		if (select.IsBlank())
			return Results.BadRequest(Invariant($"[{nameof(select)}] query parameter must be specified."));

		var selectQuery = new SelectQuery
		{
			Distinct = distinct,
			DistinctOn = distinctOn,
			Fetch = fetch,
			From = objectSchema.Name,
			GroupBy = groupBy?.Split(','),
			GroupByOption = groupByOption,
			Having = having,
			Offset = offset,
			OrderBy = orderBy?.Split(','),
			Select = select?.Split(','),
			TableHints = tableHints,
			Top = top,
			Where = where
		};
		var sql = objectSchema.CreateSelectSQL(selectQuery);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		var mediator = httpContext.GetMediator();
		return Results.Ok(await mediator.ApplyRuleAsync<SqlCommand, JsonArray>(sqlCommand, httpContext.RequestAborted));
	}

	public static async Task<IResult> UpdateTable(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromQuery] string set
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateUpdateSQL(set.Split(','), where, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		foreach (var pair in httpContext.Request.Query.Where(pair => pair.Key.StartsWith('@')))
			sqlCommand.Parameters.Add(pair.Key.TrimStart('@'), pair.Value);

		var mediator = httpContext.GetMediator();
		JsonArray? result = null;
		if (output.IsNotBlank())
			result = await mediator.ApplyRuleAsync<SqlCommand, JsonArray>(sqlCommand, httpContext.RequestAborted);
		else
			await mediator.RunProcessAsync<SqlCommand>(sqlCommand, httpContext.RequestAborted);

		return Results.Ok(result);
	}

	public static async Task<IResult> UpdateTableBatch(
		HttpContext httpContext
		, [FromRoute(Name = "dataSource")] string dataSourceName
		, [FromRoute] string database
		, [FromRoute] string schema
		, [FromRoute] string table
		, [FromQuery] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateUpdateSQL(data, output);
		var sqlCommand = objectSchema.DataSource.CreateSqlCommand(sql);

		var mediator = httpContext.GetMediator();
		JsonArray? result = null;
		if (output.IsNotBlank())
			result = await mediator.ApplyRuleAsync<SqlCommand, JsonArray>(sqlCommand, httpContext.RequestAborted);
		else
			await mediator.RunProcessAsync<SqlCommand>(sqlCommand, httpContext.RequestAborted);

		return Results.Ok(result);
	}
}