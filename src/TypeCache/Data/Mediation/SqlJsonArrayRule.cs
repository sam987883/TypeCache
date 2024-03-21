// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Nodes;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlJsonArrayRule : IRule<SqlJsonArrayRequest, JsonArray>
{
	public async Task<JsonArray> Map(SqlJsonArrayRequest request, CancellationToken token)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);

		var result = await command.GetJsonArrayAsync(token);

		command.CopyOutputParameters(request.Command);

		return result;
	}
}
