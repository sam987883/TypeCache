// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class MergeRule : IRule<(ISqlApi SqlApi, BatchRequest Batch), RowSet>, IRule<BatchRequest, string>
	{
		async ValueTask<RowSet> IRule<(ISqlApi SqlApi, BatchRequest Batch), RowSet>.ApplyAsync((ISqlApi SqlApi, BatchRequest Batch) request, CancellationToken cancellationToken)
		{
			var schema = request.SqlApi.GetObjectSchema(request.Batch.Table);
			request.Batch.Table = schema.Name;
			if (!request.Batch.On.Any())
				request.Batch.On = schema.Columns.If(column => column!.PrimaryKey).To(column => column!.Name).ToArray();
			return await request.SqlApi.MergeAsync(request.Batch, cancellationToken);
		}

		async ValueTask<string> IRule<BatchRequest, string>.ApplyAsync(BatchRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
