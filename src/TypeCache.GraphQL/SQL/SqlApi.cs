﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GraphQL;
using TypeCache.Business;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.SQL;

public class SqlApi<T>
	where T : class, new()
{
	private readonly string _DataSource;

	private readonly IMediator _Mediator;

	private readonly string _Table;

	public SqlApi(IMediator mediator, string dataSource, string table)
	{
		this._DataSource = dataSource;
		this._Mediator = mediator;
		this._Table = table;
	}

	[GraphName("Count{0}")]
	[GraphDescription("SELECT COUNT(1) FROM {0} WHERE ...")]
	public async Task<SqlCountResponse> Count(
		[AllowNull] Parameter[] parameters,
		[AllowNull] string where,
		IResolveFieldContext context)
	{
		var selections = context.GetQuerySelections().ToArray();
		var request = new CountRequest
		{
			DataSource = this._DataSource,
			From = this._Table,
			Where = where
		};

		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlCountResponse
		{
			Count = await this._Mediator.ApplyRulesAsync<CountRequest, long>(request)
		};

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<CountRequest, string>(request);

		return sqlResponse;
	}

	[GraphName("Delete{0}")]
	[GraphDescription("DELETE ... OUTPUT ... FROM {0} WHERE ...")]
	public async Task<SqlResponse<T>> Delete(
		[AllowNull] Parameter[] parameters,
		string where,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetQuerySelections().ToArray();
		var request = new DeleteRequest
		{
			DataSource = this._DataSource,
			From = this._Table,
			Output = selections.IfLeft(dataPrefix).EachReplace(dataPrefix, "DELETED.").ToArray(),
			Where = where
		};
		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<DeleteRequest, string>(request);

		if (request.Output.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<DeleteRequest, RowSet>(request);
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
		}
		return sqlResponse;
	}

	[GraphName("Delete{0}Data")]
	[GraphDescription("DELETE ... OUTPUT ... FROM {0} ... VALUES ...")]
	public async Task<SqlResponse<T>> DeleteData(
		[NotNull] T[] data,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetQuerySelections().ToArray();
		var columns = context.GetArgument<IDictionary<string, object>>(nameof(data))!.Keys.ToArray();
		var request = new DeleteDataRequest
		{
			DataSource = this._DataSource,
			From = this._Table,
			Input = data.MapRowSet(columns),
			Output = selections.IfLeft(dataPrefix).EachReplace(dataPrefix, "DELETED.").ToArray()
		};

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, string>(request);

		if (request.Output.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, RowSet>(request);
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
		}
		return sqlResponse;
	}

	[GraphName("Insert{0}Data")]
	[GraphDescription("INSERT INTO {0} ... VALUES ...")]
	public async Task<SqlResponse<T>> InsertBatch(
		[NotNull] T[] batch,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetQuerySelections().ToArray();
		var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array<string>.Empty;
		var request = new InsertDataRequest
		{
			DataSource = this._DataSource,
			Into = this._Table,
			Input = batch.MapRowSet(columns),
			Output = selections.IfLeft(dataPrefix).EachReplace(dataPrefix, "INSERTED.").ToArray()
		};

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<InsertDataRequest, string>(request);

		if (request.Output.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<InsertDataRequest, RowSet>(request);
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
		}
		return sqlResponse;
	}

	[GraphName("Page{0}")]
	[GraphDescription("SELECT ... FROM {0} HAVING ... WHERE ... ORDER BY ... OFFSET ... FETCH ...")]
	public async Task<SqlPagedResponse<T>> Page(
		uint first,
		uint after,
		[AllowNull] Parameter[] parameters,
		[AllowNull] string where,
		[AllowNull] string[] groupBy,
		[AllowNull] string having,
		[AllowNull] OrderBy<T>[] orderBy,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlPagedResponse<T>.Data)}.");
		var selections = context.GetQuerySelections().ToArray();
		var request = new SelectRequest
		{
			DataSource = this._DataSource,
			From = this._Table,
			GroupBy = groupBy,
			Having = having,
			OrderBy = orderBy.ToArray(_ => Invariant($"{_.Expression} {_.Sort.ToSQL()}")),
			Pager = new(first, after),
			Select = selections.IfLeft(dataPrefix).EachTrimStart(dataPrefix).ToArray(),
			Where = where
		};

		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlPagedResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<SelectRequest, string>(request);

		if (request.Select.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<SelectRequest, RowSet>(request);
			var data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			sqlResponse.Data = data.ToConnection((int)output!.Count, request.Pager!.Value);
		}
		return sqlResponse;
	}

	[GraphName("Select{0}")]
	[GraphDescription("SELECT ... FROM {0} HAVING ... WHERE ... ORDER BY ...")]
	public async Task<SqlResponse<T>> Select(
		[AllowNull] Parameter[] parameters,
		[AllowNull] string where,
		[AllowNull] string[] groupBy,
		[AllowNull] string having,
		[AllowNull] OrderBy<T>[] orderBy,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetQuerySelections().ToArray();
		var request = new SelectRequest
		{
			DataSource = this._DataSource,
			From = this._Table,
			GroupBy = groupBy,
			Having = having,
			OrderBy = orderBy.ToArray(_ => $"{_.Expression} {_.Sort.ToSQL()}"),
			Select = selections.IfLeft(dataPrefix).EachTrimStart(dataPrefix).ToArray(),
			Where = where
		};

		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<SelectRequest, string>(request);

		if (request.Select.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<SelectRequest, RowSet>(request);
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
		}
		return sqlResponse;
	}

	[GraphName("Update{0}")]
	[GraphDescription("UPDATE {0} SET ... OUTPUT ... WHERE ...")]
	public async Task<SqlUpdateResponse<T>> Update(
		[AllowNull] Parameter[] parameters,
		[NotNull] T set,
		[AllowNull] string where,
		IResolveFieldContext context)
	{
		var deletedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Deleted)}.");
		var insertedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Inserted)}.");
		var selections = context.GetQuerySelections().ToArray();
		var request = new UpdateRequest
		{
			DataSource = this._DataSource,
			Output = selections
				.If(selection => selection.Left(insertedPrefix) || selection.Left(deletedPrefix))
				.Each(selection => selection.Replace(insertedPrefix, "INSERTED.").Replace(deletedPrefix, "DELETED."))
				.ToArray(),
			Set = context.GetArgument<IDictionary<string, object>>(nameof(set))
				.To(pair => Invariant($"{pair.Key.EscapeIdentifier()} = {pair.Value.ToSQL()}"))
				.ToArray(),
			Table = this._Table,
			Where = where
		};

		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlUpdateResponse<T>();

		if (selections.Has(nameof(SqlUpdateResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlUpdateResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<UpdateRequest, string>(request);

		if (request.Output.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<UpdateRequest, RowSet>(request);
			if (output?.Rows is not null)
			{
				var outputColumns = output.Columns;

				output.Columns = outputColumns.IfLeft("DELETED.").EachTrimStart("DELETED.").ToArray();
				sqlResponse.Deleted = output.MapModels<T>();

				output.Columns = outputColumns.IfLeft("INSERTED.").EachTrimStart("INSERTED.").ToArray();
				sqlResponse.Inserted = output.MapModels<T>();
			}
			else
			{
				sqlResponse.Deleted = Array<T>.Empty;
				sqlResponse.Inserted = Array<T>.Empty;
			}
		}
		return sqlResponse;
	}

	[GraphName("Update{0}Data")]
	[GraphDescription("UPDATE {0} SET ... OUTPUT ...")]
	public async Task<SqlUpdateResponse<T>> UpdateData([NotNull] T[] data, IResolveFieldContext context)
	{
		var deletedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Deleted)}.");
		var insertedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Inserted)}.");
		var selections = context.GetQuerySelections().ToArray();
		var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(data)).First()?.Keys.ToArray() ?? Array<string>.Empty;
		var request = new UpdateDataRequest
		{
			DataSource = this._DataSource,
			Input = data.MapRowSet(columns),
			Output = selections
				.If(selection => selection.Left(insertedPrefix) || selection.Left(deletedPrefix))
				.Each(selection => selection.Replace(insertedPrefix, "INSERTED.").Replace(deletedPrefix, "DELETED."))
				.ToArray(),
			Table = this._Table,
		};

		var sqlResponse = new SqlUpdateResponse<T>();

		if (selections.Has(nameof(SqlUpdateResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlUpdateResponse<T>.SQL)))
			sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, string>(request);

		if (request.Output.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, RowSet>(request);
			if (output?.Rows is not null)
			{
				var outputColumns = output.Columns;

				output.Columns = outputColumns.IfLeft("DELETED.").EachTrimStart("DELETED.").ToArray();
				sqlResponse.Deleted = output.MapModels<T>();

				output.Columns = outputColumns.IfLeft("INSERTED.").EachTrimStart("INSERTED.").ToArray();
				sqlResponse.Inserted = output.MapModels<T>();
			}
			else
			{
				sqlResponse.Deleted = Array<T>.Empty;
				sqlResponse.Inserted = Array<T>.Empty;
			}
		}
		return sqlResponse;
	}
}
