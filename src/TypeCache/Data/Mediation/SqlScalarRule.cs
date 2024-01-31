// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlScalarRule : IRule<SqlScalarRequest, object?>
{
	public async Task<object?> Map(SqlScalarRequest request, CancellationToken token)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request.Command);

		var result = await dbCommand.ExecuteScalarAsync(token);

		dbCommand.CopyOutputParameters(request.Command);

		return result is not DBNull ? result : null;
	}
}
