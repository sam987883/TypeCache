// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Nodes;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlResultJsonRule : IRule<SqlCommand, JsonArray>
{
	public async ValueTask<JsonArray> Send(SqlCommand request, CancellationToken token)
	{
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request);

		var result = await command.GetResultsAsJsonAsync(token);

		command.CopyOutputParameters(request);

		return result;
	}
}
