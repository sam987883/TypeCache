// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Nodes;
using TypeCache.Attributes;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

[Singleton]
internal sealed class SqlResultSetJsonRule : IRule<SqlCommand, ValueTask<JsonObject>>
{
	public async ValueTask<JsonObject> Send(SqlCommand request, CancellationToken token)
	{
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request);

		var result = await command.GetResultSetAsJsonAsync(token);

		command.CopyOutputParameters(request);

		return result;
	}
}
