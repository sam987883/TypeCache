// Copyright (c) 2021 Samuel Abraham

using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal sealed class SqlCommandScalarRule : IRule<SqlCommand, object?>
{
	public async ValueTask<object?> ApplyAsync(SqlCommand request, CancellationToken token)
	{
		using var transactionScope = request.StartTransaction();
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request);

		var result = await dbCommand.ExecuteScalarAsync(token);
		if (result is DBNull)
			result = null;

		dbCommand.CopyOutputParameters(request);

		return result;
	}
}
