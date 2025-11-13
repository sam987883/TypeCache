// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlDataTableRule : IRule<SqlCommand, DataTable>
{
	public async ValueTask<DataTable> Send(SqlCommand request, CancellationToken token)
	{
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request);

		var result = await command.GetDataTableAsync(token);

		command.CopyOutputParameters(request);

		return result;
	}
}
