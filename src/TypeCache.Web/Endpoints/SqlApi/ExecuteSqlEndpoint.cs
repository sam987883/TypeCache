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
/// Create a standalone file and implement the <see cref="ExecuteSqlEndpoint{T}"/> SQL API endpoint.<br/>
/// Executes raw SQL.<br/>
/// Parameters are passed in as query parameters with names starting with @. ie. <c>?@Amount=10000&amp;@Age=30</c><br/>
/// Sample implementation: <c>POST /sql-api/sql/somework</c>
/// <code>
/// [RoutePrefix("/sql-api/sql/somework")]<br/>
/// <see langword="public class"/> ExecuteSomeWorkEndpoint : ExecuteSqlEndpoint&lt;Customer&gt;<br/>
/// {<br/>
/// <see langword="    public"/> ExecuteSomeWorkEndpoint(<see cref="IMediator"/> mediator)<br/>
/// <see langword="        "/>: <see langword="base"/>(mediator, "CustomerDB", "...", (reader, token) =&gt; ...)<br/>
/// <see langword="    "/>{<br/>
/// <see langword="    "/>}<br/>
/// }
/// </code>
/// </summary>
/// <typeparam name="T">Model type abstracting the data.</typeparam>
public class ExecuteSqlEndpoint : SqlApiEndpoint
{
	private readonly Func<DbDataReader, CancellationToken, ValueTask<object>> _ReadData;
	private readonly string _Sql;

	public ExecuteSqlEndpoint(IMediator mediator, string dataSource, string sql, Func<DbDataReader, CancellationToken, ValueTask<object>> readData)
		: base(mediator, dataSource)
	{
		this._ReadData = readData;
		this._Sql = sql;
	}

	[HttpPost]
	public async ValueTask<ObjectResult> ExecuteSqlAsync()
	{
		var request = new SqlCommand
		{
			DataSource = this.DataSource,
			ReadData = this._ReadData,
			SQL = this._Sql
		};
		this.Request.Query
			.If(pair => pair.Key.StartsWith('@'))
			.Do(pair => request.InputParameters.Add(pair.Key.TrimStart('@'), (string)pair.Value));

		return await this.ApplyRuleAsync<SqlCommand, object>(request);
	}
}
