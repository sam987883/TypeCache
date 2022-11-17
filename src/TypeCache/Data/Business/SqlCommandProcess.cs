// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal sealed class SqlCommandProcess : IProcess<SqlCommand>
{
	public async ValueTask PublishAsync(SqlCommand request, CancellationToken token = default)
	{
		using var transactionScope = request.StartTransaction();
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request);

		await dbCommand.ExecuteNonQueryAsync(token);

		dbCommand.CopyOutputParameters(request);
	}
}
