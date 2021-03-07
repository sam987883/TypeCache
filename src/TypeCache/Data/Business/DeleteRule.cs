// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class DeleteRule : IRule<ISqlApi, DeleteRequest, RowSet>, IRule<DeleteRequest, string>
	{
		async ValueTask<RowSet> IRule<ISqlApi, DeleteRequest, RowSet>.ApplyAsync(ISqlApi sqlApi, DeleteRequest request, CancellationToken cancellationToken)
			=> await sqlApi.DeleteAsync(request, cancellationToken);

		async ValueTask<string> IRule<DeleteRequest, string>.ApplyAsync(DeleteRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
