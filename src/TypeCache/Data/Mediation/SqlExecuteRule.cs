// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlExecuteRule : IRule<SqlExecuteRequest>
{
	public async Task ExecuteAsync(SqlExecuteRequest request, CancellationToken token = default)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);

		await command.ExecuteNonQueryAsync(token);

		command.CopyOutputParameters(request.Command);
	}
}
