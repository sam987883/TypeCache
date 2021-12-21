// Copyright (c) 2021 Samuel Abraham

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Requests;
using TypeCache.Extensions;

namespace TypeCache.Web.Controllers;

[ApiController]
[Route("sql-api/[controller]/{dataSource}/{objectName}")]
public class ExecuteController : ControllerBase
{
	private readonly IMediator _Mediator;
	private readonly ISqlApi _SqlApi;

	public ExecuteController(IMediator mediator, ISqlApi sqlApi)
	{
		this._Mediator = mediator;
		this._SqlApi = sqlApi;
	}

	[HttpPost(Name = "Execute SQL")]
	[ProducesResponseType(typeof(RowSet[]), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet[]>> ExecuteAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName,
		[FromBody] string sql)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new SqlRequest
		{
			DataSource = dataSource,
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
			SQL = sql
		};
		var response = await this._Mediator.ApplyRulesAsync<SqlRequest, RowSet[]>(request);
		return this.Ok(response);
	}
}
