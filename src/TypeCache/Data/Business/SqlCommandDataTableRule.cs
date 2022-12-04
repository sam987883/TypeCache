// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal sealed class SqlCommandDataTableRule : IRule<SqlCommand, DataTable>
{
	public async ValueTask<DataTable> ApplyAsync(SqlCommand request, CancellationToken token)
	{
		using var transactionScope = request.StartTransaction();
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request);

		var result = await dbCommand.GetDataTableAsync(token);

		dbCommand.CopyOutputParameters(request);

		return result;
	}
}
