// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class InsertRule : IRule<DbConnection, InsertRequest, RowSet>, IRule<DbConnection, InsertRequest, string>
	{
		private readonly ISchemaStore _SchemaStore;

		public InsertRule(ISchemaStore schemaStore)
			=> this._SchemaStore = schemaStore;

		async ValueTask<RowSet> IRule<DbConnection, InsertRequest, RowSet>.ApplyAsync(DbConnection connection, InsertRequest request, CancellationToken cancellationToken)
			=> await connection.InsertAsync(request, this._SchemaStore, cancellationToken);

		async ValueTask<string> IRule<DbConnection, InsertRequest, string>.ApplyAsync(DbConnection connection, InsertRequest request, CancellationToken cancellationToken)
		{
			var insertSchema = await this._SchemaStore.GetObjectSchema(connection, request.Into);
			request.Into = insertSchema.Name;
			var fromSchema = await this._SchemaStore.GetObjectSchema(connection, request.From);
			request.From = fromSchema.Name;
			return await ValueTask.FromResult(request.ToSql());
		}
	}
}
