// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class DeleteDataRule<T> : IRule<DeleteDataCommand<T>, RowSetResponse<T>>, IRule<DeleteDataCommand<T>, string>
	where T : new()
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public DeleteDataRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<RowSetResponse<T>> IRule<DeleteDataCommand<T>, RowSetResponse<T>>.ApplyAsync(DeleteDataCommand<T> request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		return await connection.DeleteDataAsync(request, token);
	}

	async ValueTask<string> IRule<DeleteDataCommand<T>, string>.ApplyAsync(DeleteDataCommand<T> request, CancellationToken token)
		=> await Task.Run(() => request.ToSQL());
}
