// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlExecuteRule : IRule<SqlCommand>
{
	public async Task Execute(SqlCommand request, CancellationToken token = default)
	{
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request);

		await command.ExecuteNonQueryAsync(token);

		command.CopyOutputParameters(request);
	}
}
