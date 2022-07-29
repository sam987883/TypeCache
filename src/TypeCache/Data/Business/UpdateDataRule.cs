// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class UpdateDataRule<T> : IRule<UpdateDataCommand<T>, UpdateRowSetResponse<T>>, IRule<UpdateDataCommand<T>, string>
	where T : new()
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public UpdateDataRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<UpdateRowSetResponse<T>> IRule<UpdateDataCommand<T>, UpdateRowSetResponse<T>>.ApplyAsync(UpdateDataCommand<T> request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		return await connection.UpdateDataAsync(request, token);
	}

	async ValueTask<string> IRule<UpdateDataCommand<T>, string>.ApplyAsync(UpdateDataCommand<T> request, CancellationToken token)
		=> await Task.Run(() => request.ToSQL());
}
