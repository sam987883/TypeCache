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
/// Create a standalone file and implement the <see cref="DeleteEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL DELETE statement.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>DELETE /sql-api/customer</c>
/// <code>
/// [RoutePrefix("/sql-api/customer")]<br/>
/// <see langword="public class"/> DeleteCustomerEndpoint : DeleteEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> DeleteCustomerEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class DeleteEndpoint<T> : SqlApiEndpoint
{
	public DeleteEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpDelete]
	public async ValueTask<ObjectResult> DeleteAsync([FromQuery] string output, [FromQuery] string where)
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

		return await this.ApplyRuleAsync<DeleteCommand, RowSetResponse<T>>(request);
	}
}
