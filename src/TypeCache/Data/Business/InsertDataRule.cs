// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class InsertDataRule<T> : IRule<InsertDataCommand<T>, RowSetResponse<T>>, IRule<InsertDataCommand<T>, string>
	where T : new()
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public InsertDataRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<RowSetResponse<T>> IRule<InsertDataCommand<T>, RowSetResponse<T>>.ApplyAsync(InsertDataCommand<T> request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		await connection.OpenAsync(token);

		return await connection.InsertDataAsync(request, token);
	}

	async ValueTask<string> IRule<InsertDataCommand<T>, string>.ApplyAsync(InsertDataCommand<T> request, CancellationToken token)
		=> await Task.Run(() => request.ToSQL());
}
