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

namespace TypeCache.GraphQL.SQL
{
	public class SqlApi<T>
		where T : class, new()
	{
		private readonly string _DataSource;
		private readonly IMediator _Mediator;

		public string Table { get; }

		public SqlApi(IMediator mediator, string dataSource, string table)
		{
			this._DataSource = dataSource;
			this._Mediator = mediator;

			this.Table = table;
		}

		[GraphName("Delete{0}")]
		[GraphDescription("DELETE ... OUTPUT ... FROM {0} WHERE ...")]
		public async Task<SqlResponse<T>> Delete(string where, [AllowNull] Parameter[] parameters, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var request = new DeleteRequest
			{
				DataSource = this._DataSource,
				From = this.Table,
				Where = where
			};
			parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "DELETED");

			var sqlResponse = new SqlResponse<T>();

			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;

			if (selections.Has(nameof(SqlResponse<T>.SQL)))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<DeleteRequest, string>(request);

			if (request.Output.Any())
			{
				var output = await this._Mediator.ApplyRulesAsync<DeleteRequest, RowSet>(request);
				sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("DeleteData{0}")]
		[GraphDescription("DELETE ... OUTPUT ... FROM {0} ... VALUES")]
		public async Task<SqlResponse<T>> DeleteData([NotNull] T[] data, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>>(nameof(data)).Keys.ToArray();
			var request = new DeleteDataRequest
			{
				DataSource = this._DataSource,
				From = this.Table,
				Input = data.MapRowSet(columns)
			};
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "DELETED");

			var sqlResponse = new SqlResponse<T>();

			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;

			if (selections.Has(nameof(SqlResponse<T>.SQL)))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, string>(request);

			if (request.Output.Any())
			{
				var output = await this._Mediator.ApplyRulesAsync<DeleteDataRequest, RowSet>(request);
				sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("InsertData{0}")]
		[GraphDescription("INSERT INTO {0} ... VALUES")]
		public async Task<SqlResponse<T>> InsertBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array<string>.Empty;
			var request = new InsertDataRequest
			{
				DataSource = this._DataSource,
				Into = this.Table,
				Input = batch.MapRowSet(columns)
			};
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "INSERTED");

			var sqlResponse = new SqlResponse<T>();

			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;

			if (selections.Has(nameof(SqlResponse<T>.SQL)))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<InsertDataRequest, string>(request);

			if (request.Output.Any())
			{
				var output = await this._Mediator.ApplyRulesAsync<InsertDataRequest, RowSet>(request);
				sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("Select{0}")]
		[GraphDescription("SELECT ... FROM {0} HAVING ... WHERE ... ORDER BY ...")]
		public async Task<SqlResponse<T>> Select(
			[AllowNull] string where,
			[AllowNull] string having,
			[AllowNull] OrderBy<T>[] orderBy,
			[AllowNull] Parameter[] parameters,
			IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var request = new SelectRequest
			{
				DataSource = this._DataSource,
				From = this.Table,
				Having = having,
				Where = where
			};
			parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Select[selection] = selection);
			request.OrderBy = orderBy.To(_ => (_.Expression, _.Sort)).ToArray();

			var sqlResponse = new SqlResponse<T>();

			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;

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
		public async Task<SqlResponse<T>> Update([AllowNull] Parameter[] parameters, T set, string where, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>>(nameof(set)).Keys.ToArray();
			var request = new UpdateRequest
			{
				DataSource = this._DataSource,
				Table = this.Table,
				Where = where
			};
			parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "INSERTED");
			columns.Do(column =>
			{
				var property = TypeOf<T>.Properties.Values.FirstValue(property =>
					property.Attributes.First<GraphNameAttribute>()?.Name.Is(column) is true) ?? TypeOf<T>.Properties.GetValue(column)!;
				request.Set[property.Value.Name] = property.Value.GetValue(set);
			});

			var sqlResponse = new SqlResponse<T>();

			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;

			if (selections.Has(nameof(SqlResponse<T>.SQL)))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<UpdateRequest, string>(request);

			if (request.Output.Any())
			{
				var output = await this._Mediator.ApplyRulesAsync<UpdateRequest, RowSet>(request);
				sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("UpdateData{0}")]
		[GraphDescription("UPDATE {0} SET ... OUTPUT ... VALUES")]
		public async Task<SqlResponse<T>> UpdateData([NotNull] T[] data, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(data)).First()?.Keys.ToArray() ?? Array<string>.Empty;
			var request = new UpdateDataRequest
			{
				DataSource = this._DataSource,
				Input = data.MapRowSet(columns),
				Table = this.Table,
			};
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "INSERTED");

			var sqlResponse = new SqlResponse<T>();

			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;

			if (selections.Has(nameof(SqlResponse<T>.SQL)))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, string>(request);

			if (request.Output.Any())
			{
				var output = await this._Mediator.ApplyRulesAsync<UpdateDataRequest, RowSet>(request);
				sqlResponse.Data = output?.Rows is not null ? output.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}
	}
}
