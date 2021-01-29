// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class MergeRule : IRule<DbConnection, BatchRequest, RowSet>, IRule<DbConnection, BatchRequest, string>
	{
		async ValueTask<RowSet> IRule<DbConnection, BatchRequest, RowSet>.ApplyAsync(DbConnection connection, BatchRequest request, CancellationToken cancellationToken)
			=> await connection.MergeAsync(request, cancellationToken);

		async ValueTask<string> IRule<DbConnection, BatchRequest, string>.ApplyAsync(DbConnection connection, BatchRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
