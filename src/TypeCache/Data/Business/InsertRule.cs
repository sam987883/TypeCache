// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business;

internal class InsertRule : IRule<InsertRequest, RowSet>, IRule<InsertRequest, string>
{
	private readonly ISqlApi _SqlApi;

	public InsertRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	async ValueTask<RowSet> IRule<InsertRequest, RowSet>.ApplyAsync(InsertRequest request, CancellationToken cancellationToken)
	{
		request.From = this._SqlApi.GetObjectSchema(request.DataSource, request.From).Name;
		request.Into = this._SqlApi.GetObjectSchema(request.DataSource, request.Into).Name;
		return await this._SqlApi.InsertAsync(request, cancellationToken);
	}

	async ValueTask<string> IRule<InsertRequest, string>.ApplyAsync(InsertRequest request, CancellationToken cancellationToken)
	{
		var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.From);
		return await ValueTask.FromResult(request.ToSQL());
	}
}
