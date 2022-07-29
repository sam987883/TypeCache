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
/// Create a standalone file and implement the <see cref="BatchUpdateEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL UPDATE statement based on data passed in.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>PUT /sql-api/customer/batch</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/batch")]<br/>
/// <see langword="public class"/> BatchUpdateCustomerEndpoint : BatchUpdateEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> BatchUpdateCustomerEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class BatchUpdateEndpoint<T> : SqlApiEndpoint
{
	public BatchUpdateEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpPut]
	public async ValueTask<ObjectResult> UpdateBatchAsync([FromQuery] string set, [FromQuery] string output, [FromBody] T[] input)
	{
		var request = new UpdateDataCommand<T>
		{
			Columns = set.IsNotBlank() ? set.Split(',') : Array<string>.Empty,
			DataSource = this.DataSource,
			Input = input,
			Output = output.IsNotBlank() ? output.Split(',') : Array<string>.Empty,
			Table = TypeOf<T>.Name
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<UpdateDataCommand<T>, UpdateRowSetResponse<T>>(request);
	}
}
