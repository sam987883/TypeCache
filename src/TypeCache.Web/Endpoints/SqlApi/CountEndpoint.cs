// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Extensions;

namespace TypeCache.Web.Endpoints.SqlApi;

/// <summary>
/// Create a standalone file and implement the <see cref="CountEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL SELECT COUNT(1) statement.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>GET /sql-api/customer/count</c>
/// <code>
/// [RoutePrefix("/sql-api/customer")]<br/>
/// <see langword="public class"/> CountCustomerEndpoint : CountEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> CountCustomerEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class CountEndpoint<T> : SqlApiEndpoint
{
	public CountEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpGet("count")]
	public async ValueTask<ObjectResult> CountAsync([FromQuery] string distinct, [FromQuery] string tableHints, [FromQuery] string where)
	{
		var request = new CountCommand
		{
			DataSource = this.DataSource,
			Distinct = distinct,
			Table = TypeOf<T>.Name,
			TableHints = tableHints,
			Where = where
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<CountCommand, long>(request);
	}
}
