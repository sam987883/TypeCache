// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlDataSetRule : IRule<SqlDataSetRequest, DataSet>
{
	public async ValueTask<DataSet> ApplyAsync(SqlDataSetRequest request, CancellationToken token)
	{
		await using var connection = request.Command.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var command = connection.CreateCommand(request.Command);
		using var adapter = request.Command.DataSource.Factory.CreateDataAdapter()!;
		adapter.SelectCommand = command;

		var result = new DataSet();
		adapter.Fill(result);

		command.CopyOutputParameters(request.Command);

		return result;
	}
}
