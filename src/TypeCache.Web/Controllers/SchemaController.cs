// Copyright (c) 2021 Samuel Abraham

using System.Net;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Web.Controllers
{
	[ApiController]
	[Route("sql-api/[controller]/{dataSource}/{objectName}")]
	public class SchemaController : ControllerBase
	{
		private readonly ISqlApi _SqlApi;

		public SchemaController(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		[HttpGet(Name = "Schema")]
		[ProducesResponseType(typeof(ObjectSchema), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
		public ActionResult<ObjectSchema> GetSchema(
			[FromRoute] string dataSource,
			[FromRoute] string objectName)
		{
			if (dataSource.IsBlank())
				return this.BadRequest($"Missing {nameof(dataSource)}.");

			if (objectName.IsBlank())
				return this.BadRequest($"Missing {nameof(objectName)}.");

			var schema = this._SqlApi.GetObjectSchema(dataSource, objectName);
			return this.Ok(schema);
		}

		[HttpGet("sql", Name = "Schema SQL")]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest)]
		public ActionResult<string> GetSchemaSql(
			[FromRoute] string dataSource,
			[FromRoute] string objectName)
		{
			if (dataSource.IsBlank())
				return this.BadRequest($"Missing {nameof(dataSource)}.");

			if (objectName.IsBlank())
				return this.BadRequest($"Missing {nameof(objectName)}.");

			return this.Ok(Invariant($"DECLARE @ObjectName AS VARCHAR(MAX) = '{objectName.EscapeValue()}';\r\n{ObjectSchema.SQL}"));
		}
	}
}
