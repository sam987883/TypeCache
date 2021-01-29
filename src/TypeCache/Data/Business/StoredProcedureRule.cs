// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class StoredProcedureRule : IRule<DbConnection, StoredProcedureRequest, StoredProcedureResponse>
	{
		public async ValueTask<StoredProcedureResponse> ApplyAsync(DbConnection dbConnection, StoredProcedureRequest request, CancellationToken cancellationToken)
		{
			var schema = dbConnection.GetObjectSchema(request.Procedure);
			request.Procedure = schema.Name;
			return new StoredProcedureResponse
			{
				Parameters = request.Parameters,
				Output = await dbConnection.CallAsync(request)
			};
		}
	}
}
