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
public class ProcedureController : ControllerBase
{
	private readonly IMediator _Mediator;

	public ProcedureController(IMediator mediator)
	{
		this._Mediator = mediator;
	}

	[HttpGet(Name = "Stored Procedure")]
	[ProducesResponseType(typeof(RowSet[]), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
	public async ValueTask<ActionResult<RowSet[]>> CallAsync(
		[FromRoute] string dataSource,
		[FromRoute] string objectName)
	{
		if (dataSource.IsBlank())
			return this.BadRequest($"Missing {nameof(dataSource)}.");

		if (objectName.IsBlank())
			return this.BadRequest($"Missing {nameof(objectName)}.");

		var request = new StoredProcedureRequest
		{
			DataSource = dataSource,
			Procedure = objectName,
			Parameters = this.HttpContext.Request.Query
				.If(pair => pair.Key.StartsWith('$'))
				.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!
		};
		var response = await this._Mediator.ApplyRulesAsync<StoredProcedureRequest, RowSet[]>(request);
		return this.Ok(response);
	}
}
