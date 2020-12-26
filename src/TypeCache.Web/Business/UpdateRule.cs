// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class UpdateRule : IRule<DbConnection, UpdateRequest, RowSet>, IRule<DbConnection, UpdateRequest, string>
	{
		private readonly ISchemaStore _SchemaStore;

		public UpdateRule(ISchemaStore schemaStore)
			=> this._SchemaStore = schemaStore;

		async ValueTask<RowSet> IRule<DbConnection, UpdateRequest, RowSet>.ApplyAsync(DbConnection connection, UpdateRequest request, CancellationToken cancellationToken)
			=> await connection.UpdateAsync(request, this._SchemaStore, cancellationToken);

		async ValueTask<string> IRule<DbConnection, UpdateRequest, string>.ApplyAsync(DbConnection connection, UpdateRequest request, CancellationToken cancellationToken)
		{
			var schema = await this._SchemaStore.GetObjectSchema(connection, request.Table);
			request.Table = schema.Name;
			return await ValueTask.FromResult(request.ToSql());
		}
	}
}
