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
/// Create a standalone file and implement the <see cref="UpdateSqlEndpoint{T}"/> SQL API endpoint.<br/>
/// Gets the SQL used by the UPDATE command.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>GET /sql-api/customer/sql/update</c>
/// <code>
/// [RoutePrefix("/sql-api/customer/sql")]<br/>
/// <see langword="public class"/> UpdateCustomerSqlEndpoint : UpdateSqlEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> UpdateCustomerSqlEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB")<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class UpdateSqlEndpoint<T> : SqlApiEndpoint
{
	public UpdateSqlEndpoint(IMediator mediator, string dataSource)
		: base(mediator, dataSource)
	{
	}

	[HttpGet("update", Name = "UPDATE SQL")]
	public async ValueTask<ObjectResult> GetUpdateSqlAsync(
		[FromQuery] string output,
		[FromQuery] string set,
		[FromQuery] string where)
	{
		var request = new UpdateCommand
		{
			DataSource = this.DataSource,
			Output = output.IsNotBlank() ? output.Split(',') : Array<string>.Empty,
			Set = set.IsNotBlank() ? set.Split(',') : Array<string>.Empty,
			Table = TypeOf<T>.Name,
			Where = where
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<UpdateCommand, string>(request);
	}
}
