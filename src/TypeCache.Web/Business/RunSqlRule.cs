// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class RunSqlRule : IRule<DbConnection, SqlRequest, RowSet[]>
	{
		private readonly ISchemaStore _SchemaStore;

		public RunSqlRule(ISchemaStore schemaStore)
			=> this._SchemaStore = schemaStore;

		public async ValueTask<RowSet[]> ApplyAsync(DbConnection connection, SqlRequest request, CancellationToken cancellationToken)
			=> await connection.RunAsync(request, cancellationToken);
	}
}
