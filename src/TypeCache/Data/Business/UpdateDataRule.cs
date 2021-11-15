// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business
{
	internal class UpdateDataRule : IRule<UpdateDataRequest, RowSet>, IRule<UpdateDataRequest, string>
	{
		private readonly ISqlApi _SqlApi;

		public UpdateDataRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		async ValueTask<RowSet> IRule<UpdateDataRequest, RowSet>.ApplyAsync(UpdateDataRequest request, CancellationToken cancellationToken)
		{
			return await this._SqlApi.UpdateDataAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<UpdateDataRequest, string>.ApplyAsync(UpdateDataRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
