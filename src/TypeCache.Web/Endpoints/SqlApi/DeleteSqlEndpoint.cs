// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Extensions;

namespace TypeCache.Web.Endpoints.SqlApi;

/// <summary>
/// Create a standalone file and implement the <see cref="DeleteSqlEndpoint{T}"/> SQL API endpoint.<br/>
/// Gets the SQL used by the DELETE command.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>GET /sql-api/customer/sql/delete</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/sql")]<br/>
/// <see langword="public class"/> DeleteCustomerSqlEndpoint : DeleteSqlEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> DeleteCustomerSqlEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class DeleteSqlEndpoint<T> : SqlApiEndpoint
{
	public DeleteSqlEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpGet("delete", Name = "DELETE SQL")]
	public async ValueTask<ObjectResult> GetDeleteSqlAsync([FromQuery] string output, [FromQuery] string where)
	{
		var request = new DeleteCommand
		{
			DataSource = this.DataSource,
			Table = TypeOf<T>.Name,
			Output = output.IsNotBlank() ? output.Split(',') : Array<string>.Empty,
			Where = where
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<DeleteCommand, string>(request);
	}
}
