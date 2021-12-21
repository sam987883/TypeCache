// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business;

internal class DeleteRule : IRule<DeleteRequest, RowSet>, IRule<DeleteRequest, string>
{
	private readonly ISqlApi _SqlApi;

	public DeleteRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	async ValueTask<RowSet> IRule<DeleteRequest, RowSet>.ApplyAsync(DeleteRequest request, CancellationToken cancellationToken)
	{
		request.From = this._SqlApi.GetObjectSchema(request.DataSource, request.From).Name;
		return await this._SqlApi.DeleteAsync(request, cancellationToken);
	}

	async ValueTask<string> IRule<DeleteRequest, string>.ApplyAsync(DeleteRequest request, CancellationToken cancellationToken)
		=> await ValueTask.FromResult(request.ToSQL());
}
