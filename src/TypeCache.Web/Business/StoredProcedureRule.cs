// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class StoredProcedureRule : IRule<DbConnection, StoredProcedureRequest, StoredProcedureResponse>
	{
		private readonly ISchemaStore _SchemaStore;

		public StoredProcedureRule(ISchemaStore schemaStore)
			=> this._SchemaStore = schemaStore;

		public async ValueTask<StoredProcedureResponse> ApplyAsync(DbConnection connection, StoredProcedureRequest request, CancellationToken cancellationToken)
		{
			var schema = await this._SchemaStore.GetObjectSchema(connection, request.Procedure);
			request.Procedure = schema.Name;
			return new StoredProcedureResponse
			{
				Parameters = request.Parameters,
				Output = await connection.CallAsync(request)
			};
		}
	}
}
