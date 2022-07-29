// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TypeCache.Business;
using TypeCache.Data.Domain;

namespace TypeCache.Data.Business;

internal class ExecuteCommandsRule : IRule<ExecuteCommands, object>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public ExecuteCommandsRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<object> IRule<ExecuteCommands, object>.ApplyAsync(ExecuteCommands request, CancellationToken token)
	{
		using var transactionScope = request.TransactionOptions.IsolationLevel switch
		{
			IsolationLevel.Unspecified => null,
			_ => new TransactionScope(request.TransactionScopeOption, request.TransactionOptions, request.TransactionScopeAsyncFlowOption)
		};

		await using var connection = this._DataSourceAccessor[request.DataSource].CreateDbConnection();
		await connection.OpenAsync(token);

		var result = await request.Execute!(connection, token);

		transactionScope?.Complete();
		await connection.CloseAsync();

		return result;
	}
}
