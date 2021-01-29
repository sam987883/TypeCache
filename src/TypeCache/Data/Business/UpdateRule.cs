// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class UpdateRule : IRule<DbConnection, UpdateRequest, RowSet>, IRule<DbConnection, UpdateRequest, string>
	{
		async ValueTask<RowSet> IRule<DbConnection, UpdateRequest, RowSet>.ApplyAsync(DbConnection connection, UpdateRequest request, CancellationToken cancellationToken)
			=> await connection.UpdateAsync(request, cancellationToken);

		async ValueTask<string> IRule<DbConnection, UpdateRequest, string>.ApplyAsync(DbConnection connection, UpdateRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
