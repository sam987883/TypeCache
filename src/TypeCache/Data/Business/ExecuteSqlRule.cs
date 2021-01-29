// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class ExecuteSqlRule : IRule<DbConnection, SqlRequest, RowSet[]>
	{
		public async ValueTask<RowSet[]> ApplyAsync(DbConnection connection, SqlRequest request, CancellationToken cancellationToken)
			=> await connection.RunAsync(request, cancellationToken);
	}
}
