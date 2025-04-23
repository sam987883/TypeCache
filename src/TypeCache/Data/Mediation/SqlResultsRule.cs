// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed class SqlModelsRequest : IRequest<IList<object>>
{
	public required SqlCommand Command { get; set; }

	public int ListInitialCapacity { get; set; }

	public required Type ModelType { get; set; }
}

internal sealed class SqlResultsRule : IRule<SqlModelsRequest, IList<object>>
{
	public async Task<IList<object>> Map(SqlModelsRequest request, CancellationToken token)
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

public sealed class SqlResultsRequest<T> : IRequest<IList<T>>
	where T : notnull, new()
{
	public required SqlCommand Command { get; set; }

	public int ListInitialCapacity { get; set; }
}

internal sealed class SqlResultsRule<T> : IRule<SqlResultsRequest<T>, IList<T>>
	where T : notnull, new()
{
	public async Task<IList<T>> Map(SqlResultsRequest<T> request, CancellationToken token)
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
