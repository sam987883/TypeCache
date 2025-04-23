// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed class SqlExecuteRequest : IRequest
{
	public required SqlCommand Command { get; set; }
}

internal sealed class SqlExecuteRule : IRule<SqlExecuteRequest>
{
	public async Task Execute(SqlExecuteRequest request, CancellationToken token = default)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);

		await command.ExecuteNonQueryAsync(token);

		command.CopyOutputParameters(request.Command);
	}
}
