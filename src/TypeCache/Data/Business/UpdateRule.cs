// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business
{
	internal class UpdateRule : IRule<UpdateRequest, RowSet>, IRule<UpdateRequest, string>
	{
		private readonly ISqlApi _SqlApi;

		public UpdateRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		async ValueTask<RowSet> IRule<UpdateRequest, RowSet>.ApplyAsync(UpdateRequest request, CancellationToken cancellationToken)
		{
			request.Table = this._SqlApi.GetObjectSchema(request.DataSource, request.Table).Name;
			return await this._SqlApi.UpdateAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<UpdateRequest, string>.ApplyAsync(UpdateRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
