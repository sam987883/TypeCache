// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;

namespace TypeCache.Data.Business
{
	internal class ExecuteSqlRule : IRule<(ISqlApi SqlApi, SqlRequest SQL), RowSet[]>
	{
		public async ValueTask<RowSet[]> ApplyAsync((ISqlApi SqlApi, SqlRequest SQL) request, CancellationToken cancellationToken)
			=> await request.SqlApi.RunAsync(request.SQL, cancellationToken);
	}
}
