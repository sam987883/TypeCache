// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class MergeRule : IRule<ISqlApi, BatchRequest, RowSet>, IRule<BatchRequest, string>
	{
		async ValueTask<RowSet> IRule<ISqlApi, BatchRequest, RowSet>.ApplyAsync(ISqlApi sqlApi, BatchRequest request, CancellationToken cancellationToken)
		{
			var schema = sqlApi.GetObjectSchema(request.Table);
			request.Table = schema.Name;
			if (!request.On.Any())
				request.On = schema.Columns.If(column => column!.PrimaryKey).To(column => column!.Name).ToArray();
			return await sqlApi.MergeAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<BatchRequest, string>.ApplyAsync(BatchRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
