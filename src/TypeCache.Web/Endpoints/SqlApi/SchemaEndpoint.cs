// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;

namespace TypeCache.Web.Endpoints.SqlApi;

/// <summary>
/// Create a standalone file and implement the <see cref="SchemaEndpoint{T}"/> SQL API endpoint.<br/>
/// Returns schema data for a specified database object.<br/>
/// Sample implementation: <c>GET /sql-api/schema/customer</c>
/// <code>
/// [RoutePrefix("/sql-api/schema/customer")]<br/>
/// <see langword="public class"/> CustomerSchemaEndpoint : SchemaEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> CustomerSchemaEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB", "...", (reader, token) =&gt; ...)<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class SchemaEndpoint<T> : SqlApiEndpoint
{
	public SchemaEndpoint(IMediator mediator, string dataSource, IRule<SchemaRequest, ObjectSchema> schemaRule)
		: base(mediator, dataSource)
	{
	}

	[HttpGet]
	public async ValueTask<ObjectResult> GetSchemaAsync()
	{
		var request = new SchemaRequest(this.DataSource, TypeOf<T>.Name);
		return await this.ApplyRuleAsync<SchemaRequest, ObjectSchema>(request);
	}
}
