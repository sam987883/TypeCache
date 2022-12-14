// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlModelsRule : IRule<SqlModelsRequest, IList<object>>
{
	public async ValueTask<IList<object>> ApplyAsync(SqlModelsRequest request, CancellationToken token)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);

		var result = await command.GetModelsAsync(request.ModelType, request.ListInitialCapacity, token);
		request.Command.RecordsAffected = (int?)command.Parameters[nameof(request.Command.RecordsAffected)]?.Value ?? 0;

		command.CopyOutputParameters(request.Command);

		return result;
	}
}
