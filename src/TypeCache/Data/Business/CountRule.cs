// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business;

internal class CountRule : IRule<CountCommand, long>, IRule<CountCommand, string>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public CountRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<long> IRule<CountCommand, long>.ApplyAsync(CountCommand command, CancellationToken token)
	{
		await using var connection = this._DataSourceAccessor[command.DataSource].CreateDbConnection();
		await connection.OpenAsync(token);

		return await connection.CountAsync(command, token);
	}

	async ValueTask<string> IRule<CountCommand, string>.ApplyAsync(CountCommand command, CancellationToken token)
		=> await Task.Run(() => command.ToSQL());
}
