// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class UpdateRule<T> : IRule<UpdateCommand, UpdateRowSetResponse<T>>, IRule<UpdateCommand, string>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public UpdateRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<UpdateRowSetResponse<T>> IRule<UpdateCommand, UpdateRowSetResponse<T>>.ApplyAsync(UpdateCommand request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		return await connection.UpdateAsync<T>(request, token);
	}

	async ValueTask<string> IRule<UpdateCommand, string>.ApplyAsync(UpdateCommand request, CancellationToken token)
		=> await Task.Run(() => request.ToSQL());
}
