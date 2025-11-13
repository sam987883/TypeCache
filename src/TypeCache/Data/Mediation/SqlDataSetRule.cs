// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Data.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlDataSetRule : IRule<SqlCommand, DataSet>
{
	public async ValueTask<DataSet> Send(SqlCommand request, CancellationToken token)
	{
		await using var connection = request.DataSource.CreateDbConnection();
		await connection.OpenAsync(token);
		await using var dbCommand = connection.CreateCommand(request);
		using var adapter = request.DataSource.Factory.CreateDataAdapter()!;
		adapter.SelectCommand = dbCommand;

		var result = new DataSet();
		adapter.Fill(result);

		dbCommand.CopyOutputParameters(request);

		return result;
	}
}
