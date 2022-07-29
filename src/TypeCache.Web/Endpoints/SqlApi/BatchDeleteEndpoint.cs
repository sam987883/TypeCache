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
/// Create a standalone file and implement the <see cref="BatchDeleteEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL DELETE statement based on data passed in.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>DELETE /sql-api/customer/batch</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/batch")]<br/>
/// <see langword="public class"/> BatchDeleteCustomerEndpoint : BatchDeleteEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> BatchDeleteCustomerEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class BatchDeleteEndpoint<T> : SqlApiEndpoint
{
	public BatchDeleteEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpDelete]
	public async ValueTask<ObjectResult> DeleteBatchAsync([FromQuery] string output, [FromBody] T[] input)
	{
		var request = new DeleteDataCommand<T>
		{
			DataSource = this.DataSource,
			Input = input,
			Output = !output.IsBlank() ? output.Split(',') : Array<string>.Empty,
			Table = TypeOf<T>.Name
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<DeleteDataCommand<T>, RowSetResponse<T>>(request);
	}
}
