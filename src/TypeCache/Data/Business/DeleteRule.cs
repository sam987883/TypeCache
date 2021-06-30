// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class DeleteRule : IRule<(ISqlApi SqlApi, DeleteRequest Delete), RowSet>, IRule<DeleteRequest, string>
	{
		async ValueTask<RowSet> IRule<(ISqlApi SqlApi, DeleteRequest Delete), RowSet>.ApplyAsync((ISqlApi SqlApi, DeleteRequest Delete) request, CancellationToken cancellationToken)
		{
			request.Delete.From = request.SqlApi.GetObjectSchema(request.Delete.From).Name;
			return await request.SqlApi.DeleteAsync(request.Delete, cancellationToken);
		}

		async ValueTask<string> IRule<DeleteRequest, string>.ApplyAsync(DeleteRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
