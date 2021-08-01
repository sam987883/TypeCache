// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class MergeRule : IRule<BatchRequest, RowSet>, IRule<BatchRequest, string>
	{
		private readonly ISqlApi _SqlApi;

		public MergeRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		async ValueTask<RowSet> IRule<BatchRequest, RowSet>.ApplyAsync(BatchRequest request, CancellationToken cancellationToken)
		{
			var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.Table);
			request.Table = schema.Name;
			if (!request.On.Any())
				request.On = schema.Columns.If(column => column!.PrimaryKey).To(column => column!.Name).ToArray();
			return await this._SqlApi.MergeAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<BatchRequest, string>.ApplyAsync(BatchRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
