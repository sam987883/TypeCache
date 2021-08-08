// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business
{
	internal class DeleteDataRule : IRule<DeleteDataRequest, RowSet>, IRule<DeleteDataRequest, string>
	{
		private readonly ISqlApi _SqlApi;

		public DeleteDataRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		async ValueTask<RowSet> IRule<DeleteDataRequest, RowSet>.ApplyAsync(DeleteDataRequest request, CancellationToken cancellationToken)
		{
			request.From = this._SqlApi.GetObjectSchema(request.DataSource, request.From).Name;
			return await this._SqlApi.DeleteDataAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<DeleteDataRequest, string>.ApplyAsync(DeleteDataRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
