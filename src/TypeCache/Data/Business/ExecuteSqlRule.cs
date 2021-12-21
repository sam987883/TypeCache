// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business;

internal class ExecuteSqlRule : IRule<SqlRequest, RowSet[]>
{
	private readonly ISqlApi _SqlApi;

	public ExecuteSqlRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	public async ValueTask<RowSet[]> ApplyAsync(SqlRequest request, CancellationToken cancellationToken)
		=> await this._SqlApi.RunAsync(request, cancellationToken);
}
