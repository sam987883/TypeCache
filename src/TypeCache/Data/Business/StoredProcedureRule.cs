// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;

namespace TypeCache.Data.Business
{
	internal class StoredProcedureRule : IRule<(ISqlApi SqlApi, StoredProcedureRequest Procedure), StoredProcedureResponse>
	{
		public async ValueTask<StoredProcedureResponse> ApplyAsync((ISqlApi SqlApi, StoredProcedureRequest Procedure) request, CancellationToken cancellationToken)
		{
			var procedure = request.Procedure;

			procedure.Procedure = request.SqlApi.GetObjectSchema(procedure.Procedure).Name;
			return new StoredProcedureResponse
			{
				Output = await request.SqlApi.CallAsync(procedure, cancellationToken),
				Parameters = procedure.Parameters
			};
		}
	}
}
