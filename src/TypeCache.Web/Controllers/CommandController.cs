// Copyright (c) 2021 Samuel Abraham

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Requests;
using TypeCache.Extensions;

namespace TypeCache.Web.Controllers;

[ApiController]
[Route("sql-api/[controller]/{dataSource}/{objectName}")]
public class CommandController : ControllerBase
{
	private readonly IMediator _Mediator;
	private readonly ISqlApi _SqlApi;

	public CommandController(IMediator mediator, ISqlApi sqlApi)
	{
		this._Mediator = mediator;
		this._SqlApi = sqlApi;
	}

	[HttpDelete(Name = "DELETE")]
	[ProducesResponseType(typeof(RowSet), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet>> DeleteAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new DeleteRequest
		{
			DataSource = dataSource,
			From = objectName,
			Output = !output.IsBlank() ? output.Split(',') : Array<string>.Empty,
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<DeleteRequest, RowSet>(request);
		return this.Ok(response);
	}

	[HttpGet("delete/sql", Name = "DELETE SQL")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<string>> GetDeleteSqlAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new DeleteRequest
		{
			DataSource = dataSource,
			From = objectName,
			Output = !output.IsBlank() ? output.Split(',') : Array<string>.Empty,
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<DeleteRequest, string>(request);
		return this.Ok(response);
	}

	[HttpGet(Name = "INSERT")]
	[ProducesResponseType(typeof(RowSet), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet>> InsertAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] uint first,
		[FromQuery] uint after,
		[FromQuery] string from,
		[FromQuery] string groupBy,
		[FromQuery] string having,
		[FromQuery] string orderBy,
		[FromQuery] string output,
		[FromQuery] string select,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new InsertRequest
		{
			DataSource = dataSource,
			From = from,
			GroupBy = groupBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Having = having,
			Into = objectName,
			Output = output.Any() ? output.Split(',') : Array<string>.Empty,
			OrderBy = orderBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Pager = new() { After = after, First = first },
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Select = select.Any() ? select.Split(',') : Array<string>.Empty,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<InsertRequest, RowSet>(request);
		return this.Ok(response);
	}

	[HttpGet("insert/sql", Name = "INSERT SQL")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<string>> GetInsertSqlAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] uint first,
		[FromQuery] uint after,
		[FromQuery] string from,
		[FromQuery] string groupBy,
		[FromQuery] string having,
		[FromQuery] string orderBy,
		[FromQuery] string output,
		[FromQuery] string select,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new InsertRequest
		{
			DataSource = dataSource,
			From = from,
			GroupBy = groupBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Having = having,
			Into = objectName,
			Output = output.Any() ? output.Split(',') : Array<string>.Empty,
			OrderBy = orderBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Pager = new() { After = after, First = first },
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Select = select.Any() ? select.Split(',') : Array<string>.Empty,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<InsertRequest, string>(request);
		return this.Ok(response);
	}

	[HttpGet(Name = "SELECT")]
	[ProducesResponseType(typeof(RowSet), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet>> GetSelectAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] uint first,
		[FromQuery] uint after,
		[FromQuery] string groupBy,
		[FromQuery] string having,
		[FromQuery] string orderBy,
		[FromQuery] string select,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new SelectRequest
		{
			DataSource = dataSource,
			From = objectName,
			GroupBy = groupBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Having = having,
			OrderBy = orderBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Pager = new() { After = after, First = first },
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Select = select.Any() ? select.Split(',') : Array<string>.Empty,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<SelectRequest, RowSet>(request);
		return this.Ok(response);
	}

	[HttpGet("select/sql", Name = "SELECT SQL")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<string>> GetSelectSqlAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] uint first,
		[FromQuery] uint after,
		[FromQuery] string groupBy,
		[FromQuery] string having,
		[FromQuery] string orderBy,
		[FromQuery] string select,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new SelectRequest
		{
			DataSource = dataSource,
			From = objectName,
			GroupBy = groupBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Having = having,
			OrderBy = orderBy.Any() ? orderBy.Split(',') : Array<string>.Empty,
			Pager = new() { After = after, First = first },
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Select = select.Any() ? select.Split(',') : Array<string>.Empty,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<SelectRequest, string>(request);
		return this.Ok(response);
	}

	[HttpPut(Name = "UPDATE")]
	[ProducesResponseType(typeof(RowSet), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet>> UpdateAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromQuery] string set,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new UpdateRequest
		{
			DataSource = dataSource,
			Output = !output.IsBlank() ? output.Split(',') : Array<string>.Empty,
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Set = !set.IsBlank() ? set.Split(',') : Array<string>.Empty,
			Table = objectName,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<UpdateRequest, RowSet>(request);
		return this.Ok(response);
	}

	[HttpGet("update/sql", Name = "UUPDATE SQL")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<string>> GetUpdateSqlAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromQuery] string set,
		[FromQuery] string where)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new UpdateRequest
		{
			DataSource = dataSource,
			Output = !output.IsBlank() ? output.Split(',') : Array<string>.Empty,
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			Set = !set.IsBlank() ? set.Split(',') : Array<string>.Empty,
			Table = objectName,
			Where = where
		};
		var response = await this._Mediator.ApplyRulesAsync<UpdateRequest, string>(request);
		return this.Ok(response);
	}
}
