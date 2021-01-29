// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class InsertRule : IRule<DbConnection, InsertRequest, RowSet>, IRule<DbConnection, InsertRequest, string>
	{
		async ValueTask<RowSet> IRule<DbConnection, InsertRequest, RowSet>.ApplyAsync(DbConnection connection, InsertRequest request, CancellationToken cancellationToken)
			=> await connection.InsertAsync(request, cancellationToken);

		async ValueTask<string> IRule<DbConnection, InsertRequest, string>.ApplyAsync(DbConnection connection, InsertRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
