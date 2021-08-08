// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business
{
	internal class InsertDataRule : IRule<InsertDataRequest, RowSet>, IRule<InsertDataRequest, string>
	{
		private readonly ISqlApi _SqlApi;

		public InsertDataRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		async ValueTask<RowSet> IRule<InsertDataRequest, RowSet>.ApplyAsync(InsertDataRequest request, CancellationToken cancellationToken)
		{
			request.Into = this._SqlApi.GetObjectSchema(request.DataSource, request.Into).Name;
			return await this._SqlApi.InsertDataAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<InsertDataRequest, string>.ApplyAsync(InsertDataRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
