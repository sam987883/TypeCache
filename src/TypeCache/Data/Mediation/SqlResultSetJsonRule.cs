// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Nodes;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed class SqlResultSetJsonRequest : IRequest<JsonObject>
{
	public required SqlCommand Command { get; set; }

	public JsonNodeOptions JsonOptions { get; set; }
}

internal sealed class SqlResultSetJsonRule : IRule<SqlResultSetJsonRequest, JsonObject>
{
	public async Task<JsonObject> Map(SqlResultSetJsonRequest request, CancellationToken token)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);

		var result = await command.GetResultSetAsJsonAsync(request.JsonOptions, token);

		command.CopyOutputParameters(request.Command);

		return result;
	}
}
