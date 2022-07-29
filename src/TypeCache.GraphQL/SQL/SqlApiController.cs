// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GraphQL;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.SQL;

public class SqlApiController<T>
	where T : class, new()
{
	private readonly string[] _Columns;

	private readonly string _DataSource;

	private readonly IMediator _Mediator;

	private readonly string _Table;

	public SqlApiController(IMediator mediator, string dataSource, string table)
	{
		this._Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray();
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
		var selections = context.GetSelections().ToArray();
		var output = this._Columns
			.If(column => selections.AnyRight(Invariant($"{nameof(SqlResponse<T>.Data)}.{column}")))
			.Each(column => Invariant($"DELETED.{column}"))
			.ToArray();
		var request = new DeleteCommand
		{
			DataSource = this._DataSource,
			Table = this._Table,
			Output = output,
			Where = where
		};
		parameters.Do(parameter => request.InputParameters[parameter.Name] = parameter.Value);

		var response = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			response.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlResponse<T>.Sql)))
			await this._Mediator.ApplyRuleAsync<DeleteCommand, string>(request, sql => response.Sql = sql, context.CancellationToken);

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			response.Table = request.Table;

		var hasCount = selections.Has(nameof(SqlResponse<T>.Count));
		var hasData = selections.Has(nameof(SqlResponse<T>.Data));
		if (hasCount || hasData)
			await this._Mediator.ApplyRuleAsync<DeleteCommand, RowSetResponse<T>>(request
				, output =>
				{
					if (hasCount)
						response.Count = output.Count;

					if (hasData)
						response.Data = output.Rows;
				}
				, context.CancellationToken);

		return response;
	}

	[GraphQLName("Delete{0}Data")]
	[GraphQLDescription("DELETE ... OUTPUT ... FROM {0} ... VALUES ...")]
	public async Task<SqlResponse<T>> DeleteData(
		[NotNull] T[] data,
		IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var output = this._Columns
			.If(column => selections.AnyRight(Invariant($"{nameof(SqlResponse<T>.Data)}.{column}")))
			.Each(column => Invariant($"DELETED.{column}"))
			.ToArray();
		var request = new DeleteDataCommand<T>
		{
			DataSource = this._DataSource,
			Table = this._Table,
			Input = data,
			Output = output
		};

		var response = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			response.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlResponse<T>.Sql)))
			await this._Mediator.ApplyRuleAsync<DeleteDataCommand<T>, string>(request, sql => response.Sql = sql, context.CancellationToken);

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			response.Table = request.Table;

		var hasCount = selections.Has(nameof(SqlResponse<T>.Count));
		var hasData = selections.Has(nameof(SqlResponse<T>.Data));
		if (hasCount || hasData)
			await this._Mediator.ApplyRuleAsync<DeleteDataCommand<T>, RowSetResponse<T>>(request
				, output =>
				{
					if (hasCount)
						response.Count = output.Count;

					if (hasData)
						response.Data = output.Rows;
				}
				, context.CancellationToken);

		return response;
	}

	[GraphQLName("Insert{0}Data")]
	[GraphQLDescription("INSERT INTO {0} ... VALUES ...")]
	public async Task<SqlResponse<T>> InsertData(
		[NotNull] T[] data,
		IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var output = this._Columns
			.If(column => selections.AnyRight(Invariant($"{nameof(SqlResponse<T>.Data)}.{column}")))
			.Each(column => Invariant($"INSERTED.{column}"))
			.ToArray();
		var inputs = context.GetInputs().Keys.ToArray();
		inputs = this._Columns
			.If(column => inputs.AnyRight(Invariant($".{column}")))
			.ToArray();
		var request = new InsertDataCommand<T>
		{
			Columns = inputs,
			DataSource = this._DataSource,
			Table = this._Table,
			Input = data,
			Output = output
		};

		var response = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			response.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlResponse<T>.Sql)))
			await this._Mediator.ApplyRuleAsync<InsertDataCommand<T>, string>(request, sql => response.Sql = sql, context.CancellationToken);

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			response.Table = request.Table;

		var hasCount = selections.Has(nameof(SqlResponse<T>.Count));
		var hasData = selections.Has(nameof(SqlResponse<T>.Data));
		if (hasCount || hasData)
			await this._Mediator.ApplyRuleAsync<InsertDataCommand<T>, RowSetResponse<T>>(request
				, output =>
				{
					if (hasCount)
						response.Count = output.Count;

					if (hasData)
						response.Data = output.Rows;
				}
				, context.CancellationToken);

		return response;
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
		var selections = context.GetSelections().ToArray();
		var output = this._Columns
			.If(column => selections.AnyRight(Invariant($"{nameof(SqlPagedResponse<T>.Data)}.{column}")))
			.ToArray();
		var request = new SelectCommand
		{
			DataSource = this._DataSource,
			From = this._Table,
			GroupBy = groupBy,
			Having = having,
			OrderBy = orderBy.ToArray(_ => Invariant($"{_.Expression} {_.Sort.ToSQL()}")),
			Pager = new(first, after),
			Select = output,
			Where = where
		};

		parameters.Do(parameter => request.InputParameters[parameter.Name] = parameter.Value);

		var response = new SqlPagedResponse<T>();

		if (selections.Has(nameof(SqlPagedResponse<T>.Data)))
			await this._Mediator.ApplyRuleAsync<SelectCommand, RowSetResponse<T>>(request
				, output => response.Data = output.Rows.ToConnection((int)output.Count, request.Pager!.Value)
				, context.CancellationToken);

		if (selections.Has(nameof(SqlPagedResponse<T>.DataSource)))
			response.DataSource = this._DataSource;

		if (selections.Has(nameof(SqlPagedResponse<T>.Sql)))
			await this._Mediator.ApplyRuleAsync<SelectCommand, string>(request, sql => response.Sql = sql, context.CancellationToken);

		if (selections.Has(nameof(SqlPagedResponse<T>.Table)))
			response.Table = request.From;

		return response;
	}

	[GraphQLName("Select{0}")]
	[GraphQLDescription("SELECT ... FROM {0} WHERE ... ORDER BY ...")]
	public async ValueTask<SqlResponse<T>> Select(
		[AllowNull] Parameter[] parameters,
		bool? distinct,
		int? top,
		bool? percent,
		bool? withTies,
		[AllowNull] string where,
		[AllowNull] OrderBy<T>[] orderBy,
		IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var output = this._Columns
			.If(column => selections.AnyRight(Invariant($"{nameof(SqlResponse<T>.Data)}.{column}")))
			.ToArray();
		var request = new SelectCommand
		{
			DataSource = this._DataSource,
			Distinct = distinct ?? false,
			From = this._Table,
			OrderBy = orderBy.ToArray(_ => Invariant($"{_.Expression} {_.Sort.ToSQL()}")),
			Select = output,
			Top = (uint)(top ?? 0),
			Percent = percent ?? false,
			WithTies = withTies ?? false,
			Where = where
		};

		parameters.Do(parameter => request.InputParameters[parameter.Name] = parameter.Value);

		var response = new SqlResponse<T>();

		if (selections.Has(nameof(SqlResponse<T>.DataSource)))
			response.DataSource = this._DataSource;

		var hasCount = selections.Has(nameof(SqlResponse<T>.Count));
		var hasData = selections.Has(nameof(SqlResponse<T>.Data));
		var hasSql = selections.Has(nameof(SqlResponse<T>.Sql));

		if (hasData)
		{
			await this._Mediator.ApplyRuleAsync<SelectCommand, RowSetResponse<T>>(request, output => response.Data = output.Rows, context.CancellationToken);
			if (hasCount)
				response.Count = response.Data.LongLength;

			if (hasSql)
				await this._Mediator.ApplyRuleAsync<SelectCommand, string>(request, sql => response.Sql = sql, context.CancellationToken);
		}
		else if (hasCount)
		{
			var countRequest = new CountCommand
			{
				DataSource = this._DataSource,
				Table = this._Table,
				Where = where
			};
			await this._Mediator.ApplyRuleAsync<CountCommand, long>(countRequest, count => response.Count = count, context.CancellationToken);

			if (hasSql)
				await this._Mediator.ApplyRuleAsync<CountCommand, string>(countRequest, sql => response.Sql = sql, context.CancellationToken);
		}

		if (selections.Has(nameof(SqlResponse<T>.Table)))
			response.Table = request.From;

		return response;
	}

	[GraphQLName("Update{0}")]
	[GraphQLDescription("UPDATE {0} SET ... OUTPUT ... WHERE ...")]
	public async Task<SqlUpdateResponse<T>> Update(
		[AllowNull] Parameter[] parameters,
		[NotNull] T set,
		[AllowNull] string where,
		IResolveFieldContext context)
	{
		const string DELETED = nameof(SqlUpdateResponse<T>.Deleted);
		const string INSERTED = nameof(SqlUpdateResponse<T>.Inserted);

		var selections = context.GetSelections().ToArray();
		var output = this._Columns
			.If(column => selections.AnyRight(Invariant($"{DELETED}.{column}")))
			.Each(column => Invariant($"DELETED.{column} AS [DELETED_{column}]"))
			.Union(this._Columns
				.If(column => selections.AnyRight(Invariant($"{INSERTED}.{column}")))
				.Each(column => Invariant($"INSERTED.{column} AS [INSERTED_{column}]")))
			.ToArray();
		var request = new UpdateCommand
		{
			DataSource = this._DataSource,
			Output = output,
			Columns = context.GetArgument<IDictionary<string, object>>(nameof(set))
				.Map(pair => Invariant($"{this._Columns.If(column => column.Is(pair.Key)).First()} = {pair.Value.ToSQL()}"))
				.ToArray(),
			Table = this._Table,
			Where = where
		};

		parameters.Do(parameter => request.InputParameters[parameter.Name] = parameter.Value);

		var response = new SqlUpdateResponse<T>();

		await this._Mediator.ApplyRuleAsync<UpdateCommand, UpdateRowSetResponse<T>>(request
			, output =>
			{
				if (selections.Has(nameof(SqlUpdateResponse<T>.Count)))
					response.Count = output.Count;

				if (selections.Has(DELETED))
					response.Deleted = output.Deleted;

				if (selections.Has(INSERTED))
					response.Inserted = output.Inserted;
			}
			, context.CancellationToken);

		if (selections.Has(nameof(SqlUpdateResponse<T>.Sql)))
			await this._Mediator.ApplyRuleAsync<UpdateCommand, string>(request, sql => response.Sql = sql, context.CancellationToken);

		if (selections.Has(nameof(SqlUpdateResponse<T>.Table)))
			response.Table = request.Table;

		return response;
	}

	[GraphQLName("Update{0}Data")]
	[GraphQLDescription("UPDATE {0} SET ... OUTPUT ...")]
	public async Task<SqlUpdateResponse<T>> UpdateData(
		[NotNull] T[] data,
		IResolveFieldContext context)
	{
		const string DELETED = nameof(SqlUpdateResponse<T>.Deleted);
		const string INSERTED = nameof(SqlUpdateResponse<T>.Inserted);

		var selections = context.GetSelections().ToArray();
		var output = this._Columns
			.If(column => selections.AnyRight(Invariant($"{DELETED}.{column}")))
			.Each(column => Invariant($"DELETED.{column} AS [DELETED_{column}]"))
			.Union(this._Columns
				.If(column => selections.AnyRight(Invariant($"{INSERTED}.{column}")))
				.Each(column => Invariant($"INSERTED.{column} AS [INSERTED_{column}]")))
			.ToArray();
		var request = new UpdateDataCommand<T>
		{
			DataSource = this._DataSource,
			Input = data,
			Output = output,
			Table = this._Table,
		};

		var response = new SqlUpdateResponse<T>();

		await this._Mediator.ApplyRuleAsync<UpdateDataCommand<T>, UpdateRowSetResponse<T>>(request
			, output =>
			{
				if (selections.Has(nameof(SqlUpdateResponse<T>.Count)))
					response.Count = output.Count;

				if (selections.Has(DELETED))
					response.Deleted = output.Deleted;

				if (selections.Has(INSERTED))
					response.Inserted = output.Inserted;
			}
			, context.CancellationToken);

		if (selections.Has(nameof(SqlUpdateResponse<T>.Sql)))
			await this._Mediator.ApplyRuleAsync<UpdateDataCommand<T>, string>(request, sql => response.Sql = sql, context.CancellationToken);

		if (selections.Has(nameof(SqlUpdateResponse<T>.Table)))
			response.Table = request.Table;

		return response;
	}
}
