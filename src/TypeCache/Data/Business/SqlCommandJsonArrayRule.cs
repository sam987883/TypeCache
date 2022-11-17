// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal sealed class SqlCommandJsonArrayRule : IRule<SqlCommand, JsonArray>
{
	public async ValueTask<JsonArray> ApplyAsync(SqlCommand request, CancellationToken token)
	{
		using var transactionScope = request.StartTransaction();
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request);

		var result = await dbCommand.GetJsonArrayAsync(token);

		dbCommand.CopyOutputParameters(request);

		return result;
	}
}
