// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlScalarRule : IRule<SqlCommand, object?>
{
	public async ValueTask<object?> Send(SqlCommand request, CancellationToken token)
	{
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request);

		var result = await dbCommand.ExecuteScalarAsync(token);

		dbCommand.CopyOutputParameters(request);

		return result is not DBNull ? result : null;
	}
}
