// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class SelectRule<T> : IRule<SelectCommand, RowSetResponse<T>>, IRule<SelectCommand, string>
	where T : new()
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public SelectRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<RowSetResponse<T>> IRule<SelectCommand, RowSetResponse<T>>.ApplyAsync(SelectCommand request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		return await connection.SelectAsync<T>(request, token);
	}

	async ValueTask<string> IRule<SelectCommand, string>.ApplyAsync(SelectCommand request, CancellationToken token)
		=> await Task.Run(() => request.ToSQL());
}
