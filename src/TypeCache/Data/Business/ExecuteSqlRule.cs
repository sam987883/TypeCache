// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;

namespace TypeCache.Data.Business
{
	internal class ExecuteSqlRule : IRule<ISqlApi, SqlRequest, RowSet[]>
	{
		public async ValueTask<RowSet[]> ApplyAsync(ISqlApi sqlApi, SqlRequest request, CancellationToken cancellationToken)
			=> await sqlApi.RunAsync(request, cancellationToken);
	}
}
