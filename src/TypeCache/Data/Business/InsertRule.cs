// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class InsertRule<T> : IRule<InsertCommand, RowSetResponse<T>>, IRule<InsertCommand, string>
	where T : new()
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public InsertRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<RowSetResponse<T>> IRule<InsertCommand, RowSetResponse<T>>.ApplyAsync(InsertCommand request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		await connection.OpenAsync(token);

		return await connection.InsertAsync<T>(request, token);
	}

	async ValueTask<string> IRule<InsertCommand, string>.ApplyAsync(InsertCommand request, CancellationToken token)
		=> await Task.Run(() => request.ToSQL());
}
