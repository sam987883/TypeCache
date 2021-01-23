// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class DeleteRule : IRule<DbConnection, DeleteRequest, RowSet>, IRule<DbConnection, DeleteRequest, string>
	{
		async ValueTask<RowSet> IRule<DbConnection, DeleteRequest, RowSet>.ApplyAsync(DbConnection connection, DeleteRequest request, CancellationToken cancellationToken)
			=> await connection.DeleteAsync(request, cancellationToken);

		async ValueTask<string> IRule<DbConnection, DeleteRequest, string>.ApplyAsync(DbConnection connection, DeleteRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
