// Copyright (c) 2021 Samuel Abraham

using TypeCache.Attributes;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

[Singleton]
internal sealed class SqlExecuteRule : IRule<SqlCommand, Task>
{
	public async Task Send(SqlCommand request, CancellationToken token)
	{
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request);

		await command.ExecuteNonQueryAsync(token);

		command.CopyOutputParameters(request);
	}
}
