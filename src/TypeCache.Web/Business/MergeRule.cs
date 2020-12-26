// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	public class MergeRule : IRule<DbConnection, BatchRequest, RowSet>, IRule<DbConnection, BatchRequest, string>
	{
		private readonly ISchemaStore _SchemaStore;

		public MergeRule(ISchemaStore schemaStore)
			=> this._SchemaStore = schemaStore;

		async ValueTask<RowSet> IRule<DbConnection, BatchRequest, RowSet>.ApplyAsync(DbConnection connection, BatchRequest request, CancellationToken cancellationToken)
			=> await connection.MergeAsync(request, this._SchemaStore, cancellationToken);

		async ValueTask<string> IRule<DbConnection, BatchRequest, string>.ApplyAsync(DbConnection connection, BatchRequest request, CancellationToken cancellationToken)
		{
			var schema = await this._SchemaStore.GetObjectSchema(connection, request.Table);
			request.Table = schema.Name;
			return await ValueTask.FromResult(request.ToSql());
		}
	}
}
