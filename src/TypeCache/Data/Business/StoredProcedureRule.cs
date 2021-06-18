// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;

namespace TypeCache.Data.Business
{
	internal class StoredProcedureRule : IRule<ISqlApi, StoredProcedureRequest, StoredProcedureResponse>
	{
		public async ValueTask<StoredProcedureResponse> ApplyAsync(ISqlApi sqlApi, StoredProcedureRequest request, CancellationToken cancellationToken)
		{
			request.Procedure = sqlApi.GetObjectSchema(request.Procedure).Name;
			return new StoredProcedureResponse
			{
				Output = await sqlApi.CallAsync(request, cancellationToken),
				Parameters = request.Parameters
			};
		}
	}
}
