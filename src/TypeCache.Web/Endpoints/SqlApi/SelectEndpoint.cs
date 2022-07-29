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
/// Create a standalone file and implement the <see cref="SelectEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL SELECT statement.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>GET /sql-api/customer</c>
/// <code>
/// [RoutePrefix("/sql-api/customer")]<br/>
/// <see langword="public class"/> SelectCustomerEndpoint : SelectEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> SelectCustomerEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class SelectEndpoint<T> : SqlApiEndpoint
{
	public SelectEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpGet]
	public async ValueTask<ObjectResult> SelectAsync(
		[FromQuery] uint after,
		[FromQuery] bool distinct,
		[FromQuery] uint first,
		[FromQuery] string groupBy,
		[FromQuery] string having,
		[FromQuery] string orderBy,
		[FromQuery] bool percent,
		[FromQuery] string select,
		[FromQuery] string tableHints,
		[FromQuery] uint top,
		[FromQuery] string where,
		[FromQuery] bool withTies)
	{
		var request = new SelectCommand
		{
			DataSource = this.DataSource,
			Distinct = distinct,
			From = TypeOf<T>.Name,
			GroupBy = groupBy.IsNotBlank() ? orderBy.Split(',') : Array<string>.Empty,
			Having = having,
			OrderBy = orderBy.IsNotBlank() ? orderBy.Split(',') : Array<string>.Empty,
			Pager = new() { After = after, First = first },
			Percent = percent,
			Select = select.IsNotBlank() ? select.Split(',') : Array<string>.Empty,
			TableHints = tableHints,
			Top = top,
			Where = where,
			WithTies = withTies
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<SelectCommand, RowSetResponse<T>>(request);
	}
}
