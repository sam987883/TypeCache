// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class SelectRule : IRule<DbConnection, SelectRequest, RowSet>, IRule<DbConnection, SelectRequest, string>
	{
		async ValueTask<RowSet> IRule<DbConnection, SelectRequest, RowSet>.ApplyAsync(DbConnection connection, SelectRequest request, CancellationToken cancellationToken)
			=> await connection.SelectAsync(request, cancellationToken);

		async ValueTask<string> IRule<DbConnection, SelectRequest, string>.ApplyAsync(DbConnection connection, SelectRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
