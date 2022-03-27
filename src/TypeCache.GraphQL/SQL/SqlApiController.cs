// Copyright (c) 2021 Samuel Abraham

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

public class SqlApiController<T>
	where T : class, new()
{
	private readonly string _DataSource;

	private readonly IMediator _Mediator;

	private readonly string _Table;

	public SqlApiController(IMediator mediator, string dataSource, string table)
	{
		this._DataSource = dataSource;
		this._Mediator = mediator;
		this._Table = table;
	}

	[GraphQLName("Delete{0}")]
	[GraphQLDescription("DELETE ... OUTPUT ... FROM {0} WHERE ...")]
	public async Task<SqlResponse<T>> Delete(
		[AllowNull] Parameter[] parameters,
		[AllowNull] string where,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetSelections().ToArray();
		var request = new DeleteRequest
		{
			DataSource = this._DataSource,
			From = this._Table,
			Output = selections.IfLeft(dataPrefix).EachReplace(dataPrefix, "DELETED.").ToArray(),
			Where = where
		};
		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			sqlResponse.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.Sql)))
			sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<DeleteRequest, string>(request);

		var output = await this._Mediator.ApplyRulesAsync<DeleteRequest, RowSet>(request);

		if (selections.Has(nameof(SqlResponse<T>.Count)))
			sqlResponse.Count = request.Output.Any() ? output.Rows.LongLength : (long)output[0, "RowCount"]!;

		if (selections.Has(nameof(SqlResponse<T>.Data)))
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;

		return sqlResponse;
	}

	[GraphQLName("Delete{0}Data")]
	[GraphQLDescription("DELETE ... OUTPUT ... FROM {0} ... VALUES ...")]
	public async Task<SqlResponse<T>> DeleteData(
		[NotNull] T[] data,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetSelections().ToArray();
		var inputs = context.GetInputs();
		var request = new DeleteDataRequest
		{
			DataSource = this._DataSource,
			From = this._Table,
			Input = (RowSet)inputs[nameof(data)]!,
			Output = selections.IfLeft(dataPrefix).EachReplace(dataPrefix, "DELETED.").ToArray()
		};

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			sqlResponse.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.Sql)))
			sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, string>(request);

		var output = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, RowSet>(request);

		if (selections.Has(nameof(SqlResponse<T>.Count)))
			sqlResponse.Count = request.Output.Any() ? output.Rows.LongLength : (long)output[0, "RowCount"]!;

		if (selections.Has(nameof(SqlResponse<T>.Data)))
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;

		return sqlResponse;
	}

	[GraphQLName("Insert{0}Data")]
	[GraphQLDescription("INSERT INTO {0} ... VALUES ...")]
	public async Task<SqlResponse<T>> InsertData(
		[NotNull] T[] data,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetSelections().ToArray();
		var inputs = context.GetInputs();
		var request = new InsertDataRequest
		{
			DataSource = this._DataSource,
			Into = this._Table,
			Input = (RowSet)inputs[nameof(data)]!,
			Output = selections.IfLeft(dataPrefix).EachReplace(dataPrefix, "INSERTED.").ToArray()
		};

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			sqlResponse.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlResponse<T>.Sql)))
			sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<InsertDataRequest, string>(request);

		var output = await this._Mediator.ApplyRulesAsync<InsertDataRequest, RowSet>(request);

		if (selections.Has(nameof(SqlResponse<T>.Count)))
			sqlResponse.Count = request.Output.Any() ? output.Rows.LongLength : (long)output[0, "RowCount"]!;

		if (selections.Has(nameof(SqlResponse<T>.Data)))
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;

		return sqlResponse;
	}

	[GraphQLName("Page{0}")]
	[GraphQLDescription("SELECT ... FROM {0} WHERE ... ORDER BY ... OFFSET ... FETCH ...")]
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
		var selections = context.GetSelections().ToArray();
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

		if (selections.Has(nameof(SqlPagedResponse<T>.DataSource)))
			sqlResponse.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlPagedResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlPagedResponse<T>.Sql)))
			sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<SelectRequest, string>(request);

		if (request.Select.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<SelectRequest, RowSet>(request);
			var data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			sqlResponse.Data = data.ToConnection((int)output!.Count, request.Pager!.Value);
		}
		return sqlResponse;
	}

	[GraphQLName("Select{0}")]
	[GraphQLDescription("SELECT ... FROM {0} WHERE ... ORDER BY ...")]
	public async Task<SqlResponse<T>> Select(
		[AllowNull] Parameter[] parameters,
		bool? distinct,
		int? top,
		bool? percent,
		bool? withTies,
		[AllowNull] string where,
		[AllowNull] OrderBy<T>[] orderBy,
		IResolveFieldContext context)
	{
		var dataPrefix = Invariant($"{nameof(SqlResponse<T>.Data)}.");
		var selections = context.GetSelections().ToArray();
		var request = new SelectRequest
		{
			DataSource = this._DataSource,
			Distinct = distinct ?? false,
			From = this._Table,
			OrderBy = orderBy.ToArray(_ => $"{_.Expression} {_.Sort.ToSQL()}"),
			Select = selections.IfLeft(dataPrefix).EachTrimStart(dataPrefix).ToArray(),
			Top = top ?? 0,
			Percent = percent ?? false,
			WithTies = withTies ?? false,
			Where = where
		};

		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			sqlResponse.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (request.Select.Any())
		{
			var output = await this._Mediator.ApplyRulesAsync<SelectRequest, RowSet>(request);
			sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			if (selections.Has(nameof(SqlResponse<T>.Count)))
				sqlResponse.Count = sqlResponse.Data.Length;

			if (selections.Has(nameof(SqlResponse<T>.Sql)))
				sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<SelectRequest, string>(request);
		}
		else if (selections.Has(nameof(SqlResponse<T>.Count)))
		{
			var countRequest = new CountRequest
			{
				DataSource = this._DataSource,
				From = this._Table,
				Where = where
			};
			sqlResponse.Count = await this._Mediator.ApplyRulesAsync<CountRequest, long>(countRequest);
			if (selections.Has(nameof(SqlResponse<T>.Sql)))
				sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<CountRequest, string>(countRequest);
		}

		return sqlResponse;
	}

	[GraphQLName("Update{0}")]
	[GraphQLDescription("UPDATE {0} SET ... OUTPUT ... WHERE ...")]
	public async Task<SqlUpdateResponse<T>> Update(
		[AllowNull] Parameter[] parameters,
		[NotNull] T set,
		[AllowNull] string where,
		IResolveFieldContext context)
	{
		var deletedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Deleted)}.");
		var insertedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Inserted)}.");
		var selections = context.GetSelections().ToArray();
		var request = new UpdateRequest
		{
			DataSource = this._DataSource,
			Output = selections
				.If(selection => selection.Left(insertedPrefix) || selection.Left(deletedPrefix))
				.Each(selection => selection.Replace(insertedPrefix, "INSERTED.").Replace(deletedPrefix, "DELETED."))
				.ToArray(),
			Set = context.GetArgument<IDictionary<string, object>>(nameof(set))
				.Map(pair => Invariant($"{pair.Key.EscapeIdentifier()} = {pair.Value.ToSQL()}"))
				.ToArray(),
			Table = this._Table,
			Where = where
		};

		parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);

		var sqlResponse = new SqlUpdateResponse<T>();

		if (selections.Has(nameof(SqlUpdateResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlUpdateResponse<T>.Sql)))
			sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<UpdateRequest, string>(request);

		var output = await this._Mediator.ApplyRulesAsync<UpdateRequest, RowSet>(request);

		if (selections.Has(nameof(SqlResponse<T>.Count)))
			sqlResponse.Count = request.Output.Any() ? output.Rows.LongLength : (long)output[0, "RowCount"]!;

		if (selections.Has(nameof(SqlResponse<T>.Data)))
		{
			output.Columns = output.Columns.IfLeft("DELETED.").EachTrimStart("DELETED.").ToArray();
			sqlResponse.Deleted = output.MapModels<T>();

			output.Columns = output.Columns.IfLeft("INSERTED.").EachTrimStart("INSERTED.").ToArray();
			sqlResponse.Inserted = output.MapModels<T>();
		}

		return sqlResponse;
	}

	[GraphQLName("Update{0}Data")]
	[GraphQLDescription("UPDATE {0} SET ... OUTPUT ...")]
	public async Task<SqlUpdateResponse<T>> UpdateData(
		[NotNull] T[] data,
		IResolveFieldContext context)
	{
		var deletedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Deleted)}.");
		var insertedPrefix = Invariant($"{nameof(SqlUpdateResponse<T>.Inserted)}.");
		var selections = context.GetSelections().ToArray();
		var inputs = context.GetInputs();
		var request = new UpdateDataRequest
		{
			DataSource = this._DataSource,
			Input = (RowSet)inputs[nameof(data)]!,
			Output = selections
				.If(selection => selection.Left(insertedPrefix) || selection.Left(deletedPrefix))
				.Each(selection => selection.Replace(insertedPrefix, "INSERTED.").Replace(deletedPrefix, "DELETED."))
				.ToArray(),
			Table = this._Table,
		};

		var sqlResponse = new SqlUpdateResponse<T>();

		if (selections.Has(nameof(SqlUpdateResponse<T>.Table)))
			sqlResponse.Table = this._Table;

		if (selections.Has(nameof(SqlUpdateResponse<T>.Sql)))
			sqlResponse.Sql = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, string>(request);

		var output = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, RowSet>(request);

		if (selections.Has(nameof(SqlResponse<T>.Count)))
			sqlResponse.Count = request.Output.Any() ? output.Rows.LongLength : (long)output[0, "RowCount"]!;

		if (selections.Has(nameof(SqlResponse<T>.Data)))
		{
			output.Columns = output.Columns.IfLeft("DELETED.").EachTrimStart("DELETED.").ToArray();
			sqlResponse.Deleted = output.MapModels<T>();

			output.Columns = output.Columns.IfLeft("INSERTED.").EachTrimStart("INSERTED.").ToArray();
			sqlResponse.Inserted = output.MapModels<T>();
		}

		return sqlResponse;
	}
}
