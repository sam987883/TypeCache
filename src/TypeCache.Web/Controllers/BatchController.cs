// Copyright (c) 2021 Samuel Abraham

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Data.Requests;
using TypeCache.Extensions;

namespace TypeCache.Web.Controllers;

[ApiController]
[Route("sql-api/[controller]/{dataSource}/{objectName}")]
public class BatchController : ControllerBase
{
	private readonly IMediator _Mediator;
	private readonly ISqlApi _SqlApi;

	public BatchController(IMediator mediator, ISqlApi sqlApi)
	{
		this._Mediator = mediator;
		this._SqlApi = sqlApi;
	}

	[HttpDelete(Name = "Batch DELETE")]
	[ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<long>> DeleteBatchAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromBody] RowSet input)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new DeleteDataRequest
		{
			DataSource = dataSource,
			From = objectName,
			Input = input,
			Output = output.Split(',')
		};
		var response = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, long>(request);
		return this.Ok(response);
	}

	[HttpGet("delete/sql", Name = "Batch DELETE SQL")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<string>> GetDeleteBatchSqlAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromBody] RowSet input)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new DeleteDataRequest
		{
			DataSource = dataSource,
			From = objectName,
			Input = input,
			Output = output.Split(',')
		};
		var response = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, string>(request);
		return this.Ok(response);
	}

	[HttpPost(Name = "Batch INSERT")]
	[ProducesResponseType(typeof(RowSet), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet>> InsertBatchAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromBody] RowSet input)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new InsertDataRequest
		{
			DataSource = dataSource,
			Into = objectName,
			Input = input,
			Output = output.Split(',')
		};
		var response = await this._Mediator.ApplyRulesAsync<InsertDataRequest, RowSet>(request);
		return this.Ok(response);
	}

	[HttpGet("insert/sql", Name = "Batch INSERT SQL")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<string>> GetInsertBatchSqlAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromBody] RowSet input)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new InsertDataRequest
		{
			DataSource = dataSource,
			Into = objectName,
			Input = input,
			Output = output.Split(',')
		};
		var response = await this._Mediator.ApplyRulesAsync<InsertDataRequest, string>(request);
		return this.Ok(response);
	}

	[HttpPut(Name = "Batch UPDATE")]
	[ProducesResponseType(typeof(RowSet), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet>> UpdateBatchAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromBody] RowSet input)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new UpdateDataRequest
		{
			DataSource = dataSource,
			Input = input,
			Output = output.Split(','),
			Table = objectName
		};
		var response = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, RowSet>(request);
		return this.Ok(response);
	}

	[HttpGet("update/sql", Name = "Batch UPDATE SQL")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<string>> GetUpdateBatchSqlAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromQuery] string output,
		[FromBody] RowSet input)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new UpdateDataRequest
		{
			DataSource = dataSource,
			Input = input,
			Output = output.Split(','),
			Table = objectName
		};
		var response = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, string>(request);
		return this.Ok(response);
	}
}
