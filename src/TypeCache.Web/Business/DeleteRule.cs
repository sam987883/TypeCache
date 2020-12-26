// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class DeleteRule : IRule<DbConnection, DeleteRequest, RowSet>, IRule<DbConnection, DeleteRequest, string>
	{
		private readonly ISchemaStore _SchemaStore;

		public DeleteRule(ISchemaStore schemaStore)
			=> this._SchemaStore = schemaStore;

		async ValueTask<RowSet> IRule<DbConnection, DeleteRequest, RowSet>.ApplyAsync(DbConnection connection, DeleteRequest request, CancellationToken cancellationToken)
			=> await connection.DeleteAsync(request, this._SchemaStore, cancellationToken);

		async ValueTask<string> IRule<DbConnection, DeleteRequest, string>.ApplyAsync(DbConnection connection, DeleteRequest request, CancellationToken cancellationToken)
		{
			var schema = await this._SchemaStore.GetObjectSchema(connection, request.From);
			request.From = schema.Name;
			return await ValueTask.FromResult(request.ToSql());
		}
	}
}
