// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Extensions;

namespace TypeCache.Web.Endpoints.SqlApi;

/// <summary>
/// Create a standalone file and implement the <see cref="CountSqlEndpoint{T}"/> SQL API endpoint.<br/>
/// Gets the SQL used by the SELECT COUNT command.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>GET /sql-api/customer/sql/count</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/sql")]<br/>
/// <see langword="public class"/> CountCustomerSqlEndpoint : CountSqlEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> CountCustomerSqlEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class CountSqlEndpoint<T> : SqlApiEndpoint
{
	public CountSqlEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpGet("count", Name = "SELECT COUNT SQL")]
	public async ValueTask<ObjectResult> GetCountSqlAsync([FromQuery] string distinct, [FromQuery] string tableHints, [FromQuery] string where)
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

		return await this.ApplyRuleAsync<CountCommand, string>(request);
	}
}
