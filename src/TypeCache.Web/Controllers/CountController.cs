// Copyright (c) 2021 Samuel Abraham

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Requests;
using TypeCache.Extensions;

namespace TypeCache.Web.Controllers
{
	[ApiController]
	[Route("sql-api/[controller]/{dataSource}/{objectName}")]
	public class CountController : ControllerBase
	{
		private readonly IMediator _Mediator;
		private readonly ISqlApi _SqlApi;

		public CountController(IMediator mediator, ISqlApi sqlApi)
		{
			this._Mediator = mediator;
			this._SqlApi = sqlApi;
		}

		[HttpGet(Name = "COUNT")]
		[ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
		public async ValueTask<ActionResult<long>> GetCountAsync(
			[FromRoute] string dataSource,
			[FromRoute] string objectName,
			[FromQuery] string where)
		{
			if (dataSource.IsBlank())
				return this.BadRequest($"Missing {nameof(dataSource)}.");

			if (objectName.IsBlank())
				return this.BadRequest($"Missing {nameof(objectName)}.");

			var request = new CountRequest
			{
				DataSource = dataSource,
				From = objectName,
				Parameters = this.HttpContext.Request.Query
					.If(pair => pair.Key.StartsWith('$'))
					.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
				Where = where
			};
			var response = await this._Mediator.ApplyRulesAsync<CountRequest, long>(request);
			return this.Ok(response);
		}

		[HttpGet("sql", Name = "COUNT SQL")]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
		public async ValueTask<ActionResult<string>> GetCountSqlAsync(
			[FromRoute] string dataSource,
			[FromRoute] string objectName,
			[FromQuery] string where)
		{
			if (dataSource.IsBlank())
				return this.BadRequest($"Missing {nameof(dataSource)}.");

			if (objectName.IsBlank())
				return this.BadRequest($"Missing {nameof(objectName)}.");

			var request = new CountRequest
			{
				DataSource = dataSource,
				From = objectName,
				Parameters = this.HttpContext.Request.Query
					.If(pair => pair.Key.StartsWith('$'))
					.ToDictionary(pair => pair.Key.TrimStart('$'), pair => (object?)pair.Value.First())!,
				Where = where
			};
			var response = await this._Mediator.ApplyRulesAsync<CountRequest, string>(request);
			return this.Ok(response);
		}
	}
}
