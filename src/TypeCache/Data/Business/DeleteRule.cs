// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class DeleteRule<T> : IRule<DeleteCommand, RowSetResponse<T>>, IRule<DeleteCommand, string>
	where T : new()
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public DeleteRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<RowSetResponse<T>> IRule<DeleteCommand, RowSetResponse<T>>.ApplyAsync(DeleteCommand command, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[command.DataSource].CreateDbConnection();
		await connection.OpenAsync(token);

		return await connection.DeleteAsync<T>(command, token);
	}

	async ValueTask<string> IRule<DeleteCommand, string>.ApplyAsync(DeleteCommand command, CancellationToken token)
		=> await Task.Run(() => command.ToSQL());
}
