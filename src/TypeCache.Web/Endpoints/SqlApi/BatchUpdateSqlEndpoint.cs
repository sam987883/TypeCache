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
/// Create a standalone file and implement the <see cref="BatchUpdateSqlEndpoint{T}"/> SQL API endpoint.<br/>
/// Gets the SQL used by the INSERT command based on data passed in.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>GET /sql-api/customer/batch/sql/update</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/batch/sql")]<br/>
/// <see langword="public class"/> BatchUpdateCustomerSqlEndpoint : BatchUpdateSqlEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> BatchUpdateCustomerSqlEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class BatchUpdateSqlEndpoint<T> : SqlApiEndpoint
{
	public BatchUpdateSqlEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpGet("update", Name = "Batch UPDATE SQL")]
	public async ValueTask<ObjectResult> GetUpdateBatchSqlAsync([FromQuery] string set, [FromQuery] string output, [FromBody] T[] input)
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

		return await this.ApplyRuleAsync<UpdateDataCommand<T>, string>(request);
	}
}
