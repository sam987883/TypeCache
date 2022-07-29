// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Extensions;

namespace TypeCache.Web.Endpoints.SqlApi;

/// <summary>
/// Create a standalone file and implement the <see cref="ExecuteProcedureEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes a SQL Stored Procedure.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>POST /sql-api/procedure/processcustomers</c>
/// <code>
/// [RoutePrefix("/sql-api/procedure/processcustomers")]<br/>
/// <see langword="public class"/> ExecuteProcessCustomersEndpoint : ExecuteProcedureEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> ExecuteProcessCustomersEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB", "ProcessCustomers", (reader, token) =&gt; ...)<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class ExecuteProcedureEndpoint : SqlApiEndpoint
{
	private readonly string _Procedure;
	private readonly Func<DbDataReader, CancellationToken, ValueTask<object>> _ReadData;

	public ExecuteProcedureEndpoint(IMediator mediator, string dataSource, string procedure, Func<DbDataReader, CancellationToken, ValueTask<object>> readData)
		: base(mediator, dataSource)
	{
		this._Procedure = procedure;
		this._ReadData = readData;
	}

	[HttpPost]
	public async ValueTask<ObjectResult> ExecuteProcedureAsync()
	{
		var request = new StoredProcedureCommand(this._Procedure)
		{
			DataSource = this.DataSource,
			ReadData = this._ReadData
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<StoredProcedureCommand, object>(request);
	}
}
