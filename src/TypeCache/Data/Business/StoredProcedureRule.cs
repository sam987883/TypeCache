// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Requests;
using TypeCache.Data.Responses;

namespace TypeCache.Data.Business
{
	internal class StoredProcedureRule : IRule<StoredProcedureRequest, StoredProcedureResponse>
	{
		private readonly ISqlApi _SqlApi;

		public StoredProcedureRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		public async ValueTask<StoredProcedureResponse> ApplyAsync(StoredProcedureRequest request, CancellationToken cancellationToken)
		{
			request.Procedure = this._SqlApi.GetObjectSchema(request.DataSource, request.Procedure).Name;
			return new StoredProcedureResponse
			{
				Output = await this._SqlApi.CallAsync(request, cancellationToken),
				Parameters = request.Parameters
			};
		}
	}
}
