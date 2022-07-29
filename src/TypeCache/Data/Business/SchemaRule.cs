// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Business;

internal class SchemaRule : IRule<SchemaRequest, ObjectSchema>, IRule<SchemaRequest, string>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public SchemaRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	async ValueTask<ObjectSchema> IRule<SchemaRequest, ObjectSchema>.ApplyAsync(SchemaRequest request, CancellationToken token)
	{
		var dataSource = this._DataSourceAccessor[request.DataSource];
		dataSource.AssertNotNull();

		var parts = request.Name.Split('.', StringSplitOptions.RemoveEmptyEntries).ToArray(part => part.TrimStart('[').TrimEnd(']'));
		var name = parts.Each(part => Invariant($"[{part}]")).Join('.');
		return await ValueTask.FromResult(dataSource.ObjectSchemas[name]);
	}

	async ValueTask<string> IRule<SchemaRequest, string>.ApplyAsync(SchemaRequest request, CancellationToken token)
		=> await ValueTask.FromResult(Invariant(@$"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

DECLARE @name AS NVARCHAR(MAX) = SELECT OBJECT_ID(@name);

{ObjectSchema.SQL}
{ColumnSchema.SQL}
{ParameterSchema.SQL}"));
}
