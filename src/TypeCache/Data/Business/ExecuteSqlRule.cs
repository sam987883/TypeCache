// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class ExecuteSqlRule : IRule<SqlCommand, object>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public ExecuteSqlRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	public async ValueTask<object> ApplyAsync(SqlCommand request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		return request.ReadData is not null
			? await connection.ExecuteAsync(request, request.ReadData, token)
			: await connection.ExecuteAsync(request, token);
	}
}
