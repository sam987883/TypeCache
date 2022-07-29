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
/// Create a standalone file and implement the <see cref="InsertEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL INSERT FROM SELECT statement.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>POST /sql-api/customer</c>
/// <code>
/// [RoutePrefix("/sql-api/customer")]<br/>
/// <see langword="public class"/> InsertCustomerEndpoint : InsertEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> InsertCustomerEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class InsertEndpoint<T> : SqlApiEndpoint
{
	public InsertEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpPost]
	public async ValueTask<ObjectResult> InsertAsync(
		[FromQuery] uint after,
		[FromQuery] string columns,
		[FromQuery] bool distinct,
		[FromQuery] string from,
		[FromQuery] uint first,
		[FromQuery] string groupBy,
		[FromQuery] string having,
		[FromQuery] string orderBy,
		[FromQuery] string output,
		[FromQuery] string select,
		[FromQuery] uint top,
		[FromQuery] string where)
	{
		var request = new InsertCommand
		{
			Columns = columns.IsNotBlank() ? columns.Split(',') : Array<string>.Empty,
			DataSource = this.DataSource,
			Distinct = distinct,
			From = from,
			GroupBy = groupBy.IsNotBlank() ? orderBy.Split(',') : Array<string>.Empty,
			Having = having,
			Table = TypeOf<T>.Name,
			Output = output.IsNotBlank() ? output.Split(',') : Array<string>.Empty,
			OrderBy = orderBy.IsNotBlank() ? orderBy.Split(',') : Array<string>.Empty,
			Pager = new() { After = after, First = first },
			Select = select.IsNotBlank() ? select.Split(',') : Array<string>.Empty,
			Top = top,
			Where = where
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<InsertCommand, RowSetResponse<T>>(request);
	}
}
