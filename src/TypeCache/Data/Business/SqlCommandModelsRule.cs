// Copyright (c) 2021 Samuel Abraham

using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal sealed class SqlCommandModelsRule<T> : IRule<SqlCommand, IList<T>>
	where T : new()
{
	public async ValueTask<IList<T>> ApplyAsync(SqlCommand request, CancellationToken token)
	{
		using var transactionScope = request.StartTransaction();
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request);

		var result = await dbCommand.GetModelsAsync<T>(request.InitialCapacity, token);
		request.RecordsAffected = (int?)dbCommand.Parameters[nameof(request.RecordsAffected)]?.Value ?? 0;

		dbCommand.CopyOutputParameters(request);

		return result;
	}
}
