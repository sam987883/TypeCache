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
/// Create a standalone file and implement the <see cref="SelectSqlEndpoint{T}"/> SQL API endpoint.<br/>
/// Gets the SQL used by the SELECT command.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>GET /sql-api/customer/sql/select</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/sql")]<br/>
/// <see langword="public class"/> SelectCustomerSqlEndpoint : SelectSqlEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> SelectCustomerSqlEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class SelectSqlEndpoint<T> : SqlApiEndpoint
{
	public SelectSqlEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpGet("select", Name = "SELECT SQL")]
	public async ValueTask<ObjectResult> GetSelectSqlAsync(
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

		return await this.ApplyRuleAsync<SelectCommand, string>(request);
	}
}
