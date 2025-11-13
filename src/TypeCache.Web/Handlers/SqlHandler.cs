// Copyright (c) 2021 Samuel Abraham

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Converters;
using TypeCache.Data;
using TypeCache.Extensions;
using static System.Net.Mime.MediaTypeNames;
using static System.Text.Encoding;

namespace TypeCache.Web.Handlers;

internal static class SqlHandler
{
	private static IDataSource GetDataSource(this HttpContext @this)
		=> (IDataSource)@this.Items[nameof(IDataSource)]!;

	private static ObjectSchema GetObjectSchema(this HttpContext @this)
		=> (ObjectSchema)@this.Items[nameof(ObjectSchema)]!;

	public static async Task<IResult> GetDatabaseSchema(
		HttpContext httpContext
		, [FromHeader(Name = "Content-Type")] string? contentType
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database to pull schema data for.")] string database
		, [FromRoute][Description("The name of the schema collection data to pull.")] SchemaCollection collection
		, [FromQuery][Description("example: TABLE_SCHEMA = 'Sales'.")] string? where
		, [FromQuery][Description("example: TABLE_NAME DESC.")] string? orderBy)
	{
		var dataSource = httpContext.GetDataSource();
		var table = await dataSource.GetDatabaseSchemaAsync(collection, database, httpContext.RequestAborted);
		if (table is null)
			return Results.NoContent();

		var data = table.Select(where, orderBy);
		if (data is null || data.Length is 0)
			return Results.NoContent();

		if (contentType?.EqualsIgnoreCase(Text.Html) is true)
		{
			var htmlBuilder = new StringBuilder();
			htmlBuilder.AppendLine("<table>");

			htmlBuilder.Append("<tr>");
			data.First().Table.Columns.OfType<DataColumn>()
				.ForEach(column => htmlBuilder.Append("<th>").Append(column.ColumnName).Append("</th>"));
			htmlBuilder.AppendLine("</tr>");

			data.ForEach(row =>
			{
				htmlBuilder.Append("<tr>");
				row.ItemArray.ForEach(item => htmlBuilder.Append("<td>").Append(item).Append("</td>"));
				htmlBuilder.AppendLine("</tr>");
			});

			htmlBuilder.AppendLine("</table>");
			return Results.Text(htmlBuilder.ToString(), contentType, Encoding.UTF8);
		}

		if (contentType?.EqualsIgnoreCase(Application.Xml) is true
			|| contentType?.EqualsIgnoreCase(Text.Xml) is true)
		{
			var collectionElement = new XElement(collection.Name);
			if (!where.IsBlank)
				collectionElement.Add(new XAttribute(nameof(where), where));

			if (!orderBy.IsBlank)
				collectionElement.Add(new XAttribute(nameof(orderBy), orderBy));

			var columns = data.First().Table.Columns.OfType<DataColumn>().ToArray();
			data.ForEach(row =>
			{
				var itemElement = new XElement("item");
				columns.ForEach(column => itemElement.Add(new XElement(column.ColumnName, row[column])));
				collectionElement.Add(itemElement);
			});

			return Results.Text(new XDocument(collectionElement).ToString(), contentType, Encoding.UTF8);
		}

		var jsonOptions = new JsonSerializerOptions();
		jsonOptions.Converters.Add(new DataRowJsonConverter());
		return Results.Json(data, jsonOptions, Application.Json, StatusCodes.Status200OK);
	}

	public static IResult GetDeleteSQL(
		HttpContext httpContext
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to delete data from.")] string table
		, [FromQuery][Description("The comma delimited columns to return (SQL syntax).")] string output
		, [FromQuery][Description("The DELETE statement WHERE clause (SQL syntax).")] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		return Results.Text(objectSchema.CreateDeleteSQL(where, [output]), Text.Plain, UTF8);
	}

	public static IResult GetDeleteValuesSQL(
		HttpContext httpContext
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to delete data from.")] string table
		, [FromQuery][Description("The comma delimited columns to return (SQL syntax).")] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateDeleteSQL(data, output);
		return Results.Text(sql, Text.Plain, UTF8);
	}

	public static IResult GetInsertSQL(
		HttpContext httpContext
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
		var objectSchema = httpContext.GetObjectSchema();

		if (columns.IsBlank)
			return Results.BadRequest(Invariant($"[{nameof(columns)}] query parameter must be specified."));

		if (select.IsBlank)
			return Results.BadRequest(Invariant($"[{nameof(select)}] query parameter must be specified."));

		if (from.IsBlank)
			return Results.BadRequest(Invariant($"[{nameof(from)}] query parameter must be specified."));

		if (columns.SplitEx(',').Length != select.SplitEx(',').Length)
			return Results.BadRequest(Invariant($"[{nameof(columns)}] and [{nameof(select)}] query parameters must have same number of columns/expressions."));

		var sql = objectSchema.CreateInsertSQL(columns.SplitEx(','), selectParameter, output?.SplitEx(',') ?? []);
		return Results.Text(sql, Text.Plain, UTF8);
	}

	public static IResult GetInsertValuesSQL(
		HttpContext httpContext
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to insert data into.")] string table
		, [FromQuery][Description("The comma delimited columns to return (SQL syntax).")] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateInsertSQL(data, [output]);
		return Results.Text(sql, Text.Plain, UTF8);
	}

	public static IResult GetSelectSQL(
		HttpContext httpContext
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table to retrieve data from.")] string table
		, SelectParameter selectParameter
		, [FromQuery][Description("SELECT [DISTINCT(...)].")] string? distinct
		, [FromQuery][Description("FETCH NEXT {fetch} ROWS ONLY.")] uint? fetch
		, [FromQuery(Name = "group-by")][Description("The GROUP BY clause comma delimited list of columns/expressions (SQL syntax).")] string? groupBy
		, [FromQuery][Description("The SELECT statement's GROUP BY ... HAVING clause (SQL syntax).")] string? having
		, [FromQuery][Description("The SELECT statement FROM clause table hints (SQL syntax).")] string? hints
		, [FromQuery][Description("OFFSET {offset} ROWS.")] uint? offset
		, [FromQuery(Name = "order-by")][Description("The SELECT statement ORDER BY clause (SQL syntax).")] string? orderBy
		, [FromQuery][Description("The SELECT statement comma delimted list of columns/expressions (SQL syntax).")] string select
		, [FromQuery][Description("The SELECT statement TOP clause value, either a number (ie. 10000) or a percentage (ie. 50%) (SQL syntax).")] string? top
		, [FromQuery][Description("The SELECT statement WHERE clause (SQL syntax).")] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		if (select is null)
			return Results.BadRequest(Invariant($"[{nameof(select)}] query parameter must be specified."));

		//var selectQuery = new SelectQuery
		//{
		//	Distinct = distinct ?? false,
		//	DistinctOn = distinctOn,
		//	Fetch = fetch ?? default,
		//	From = objectSchema.Name,
		//	GroupBy = groupBy?.SplitEx(','),
		//	GroupByOption = groupByOption ?? GroupBy.Default,
		//	Having = having,
		//	Offset = offset ?? default,
		//	OrderBy = orderBy?.SplitEx(','),
		//	Select = select?.SplitEx(','),
		//	TableHints = hints,
		//	Top = top.IsNotBlank() ? uint.Parse(top.TrimEnd('%')) : null,
		//	TopPercent = top?.EndsWith('%') is true,
		//	Where = where
		//};
		var sql = objectSchema.CreateSelectSQL(selectParameter);
		return Results.Text(sql, Text.Plain, UTF8);
	}

	public static IResult GetUpdateSQL(
		HttpContext httpContext
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table whose data will be updated.")] string table
		, [FromQuery][Description("The comma delimited columns to return (SQL syntax).")] string output
		, [FromQuery] string set
		, [FromQuery] string where)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateUpdateSQL(set.SplitEx(','), where, [output]);
		return Results.Text(sql, Text.Plain, UTF8);
	}

	public static IResult GetUpdateValuesSQL(
		HttpContext httpContext
		, [FromRoute][Description("The name of the connection string from appSettings.")] string source
		, [FromRoute][Description("The name of the database.")] string database
		, [FromRoute][Description("The name of the database object schema.")] string schema
		, [FromRoute][Description("The name of the database table whose data will be updated.")] string table
		, [FromQuery][Description("The comma delimited columns to return (SQL syntax).")] string output
		, [FromBody] JsonArray data)
	{
		var objectSchema = httpContext.GetObjectSchema();
		var sql = objectSchema.CreateUpdateSQL(data, [output]);
		return Results.Text(sql, Text.Plain, UTF8);
	}
}
