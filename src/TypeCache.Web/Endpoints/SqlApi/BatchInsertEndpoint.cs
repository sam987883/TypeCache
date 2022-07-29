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
/// Create a standalone file and implement the <see cref="BatchInsertEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL INSERT statement based on data passed in.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>POST /sql-api/customer/batch</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/batch")]<br/>
/// <see langword="public class"/> BatchInsertCustomerEndpoint : BatchInsertEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> BatchInsertCustomerEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class BatchInsertEndpoint<T> : SqlApiEndpoint
{
	public BatchInsertEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpPost]
	public async ValueTask<ObjectResult> InsertBatchAsync([FromQuery] string columns, [FromQuery] string output, [FromBody] T[] input)
	{
		var request = new InsertDataCommand<T>
		{
			Columns = columns.Any() ? columns.Split(',') : Array<string>.Empty,
			DataSource = this.DataSource,
			Table = TypeOf<T>.Name,
			Input = input,
			Output = !output.IsBlank() ? output.Split(',') : Array<string>.Empty,
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<InsertDataCommand<T>, RowSetResponse<T>>(request);
	}
}
