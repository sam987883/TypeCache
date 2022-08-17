// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class StoredProcedureRule : IRule<StoredProcedureCommand, object>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public StoredProcedureRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<object> IRule<StoredProcedureCommand, object>.ApplyAsync(StoredProcedureCommand request, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		await connection.OpenAsync(token);

		return request.ReadData is not null
			? await connection.ExecuteAsync(request, request.ReadData, token)
			: await connection.ExecuteAsync(request, token);
	}
}
