// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlResultsRule : IRule<SqlResultsRequest, IList<object>>
{
	public async ValueTask<IList<object>> Send(SqlResultsRequest request, CancellationToken token)
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

internal sealed class SqlResultsRule<T> : IRule<SqlResultsRequest<T>, IList<T>>
	where T : notnull, new()
{
	public async ValueTask<IList<T>> Send(SqlResultsRequest<T> request, CancellationToken token)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);

		var result = await command.GetModelsAsync<T>(request.ListInitialCapacity, token);
		request.Command.RecordsAffected = (int?)command.Parameters[nameof(request.Command.RecordsAffected)]?.Value ?? 0;

		command.CopyOutputParameters(request.Command);

		return result;
	}
}
