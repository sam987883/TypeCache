// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal sealed class SqlCommandDataSetRule : IRule<SqlCommand, DataSet>
{
	public async ValueTask<DataSet> ApplyAsync(SqlCommand request, CancellationToken token)
	{
		using var transactionScope = request.StartTransaction();
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
