// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class SelectRule : IRule<DbConnection, SelectRequest, RowSet>, IRule<DbConnection, SelectRequest, string>
	{
		private readonly ISchemaStore _SchemaStore;

		public SelectRule(ISchemaStore schemaStore)
			=> this._SchemaStore = schemaStore;

		async ValueTask<RowSet> IRule<DbConnection, SelectRequest, RowSet>.ApplyAsync(DbConnection connection, SelectRequest request, CancellationToken cancellationToken)
			=> await connection.SelectAsync(request, this._SchemaStore, cancellationToken);

		async ValueTask<string> IRule<DbConnection, SelectRequest, string>.ApplyAsync(DbConnection connection, SelectRequest request, CancellationToken cancellationToken)
		{
			var schema = await this._SchemaStore.GetObjectSchema(connection, request.From);
			request.From = schema.Name;
			return await ValueTask.FromResult(request.ToSql());
		}
	}
}
