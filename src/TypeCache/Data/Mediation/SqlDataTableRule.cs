// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlDataTableRule : IRule<SqlDataTableRequest, DataTable>
{
	public async Task<DataTable> MapAsync(SqlDataTableRequest request, CancellationToken token)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);

		var result = await command.GetDataTableAsync(token);

		command.CopyOutputParameters(request.Command);

		return result;
	}
}
