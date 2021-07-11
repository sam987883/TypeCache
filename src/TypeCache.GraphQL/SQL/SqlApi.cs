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
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.SQL
{
	public class SqlApi<T>
		where T : class, new()
	{
		private readonly IMediator _Mediator;
		private readonly ISqlApi _SqlApi;

		public string Table { get; }

		public SqlApi(IMediator mediator, ISqlApi sqlApi, string tableName)
		{
			this._Mediator = mediator;
			this._SqlApi = sqlApi;

			this.Table = tableName;
		}

		[GraphName("Delete{0}")]
		[GraphDescription("DELETE FROM {0} WHERE ...")]
		public async Task<SqlResponse<T>> Delete(string where, [AllowNull] Parameter[] parameters, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var request = new DeleteRequest
			{
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
			if (selections.Any(selection => selection.Left(nameof(SqlResponse<T>.SQL))))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<DeleteRequest, string>(request);
			if (request.Output.Any())
			{
				var data = await this._Mediator.ApplyRulesAsync<(ISqlApi, DeleteRequest), RowSet>((this._SqlApi, request));
				sqlResponse.Data = data?.Rows is not null ? data.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("DeleteBatch{0}")]
		[GraphDescription("DELETE FROM {0} ...")]
		public async Task<SqlResponse<T>> DeleteBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>>(nameof(batch)).Keys.ToArray();
			var request = new BatchRequest
			{
				Delete = true,
				Table = this.Table,
				Input = batch.MapRowSet(columns)
			};
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "DELETED");

			var sqlResponse = new SqlResponse<T>();
			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;
			if (selections.Any(selection => selection.Left(nameof(SqlResponse<T>.SQL))))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<BatchRequest, string>(request);
			if (request.Output.Any())
			{
				var data = await this._Mediator.ApplyRulesAsync<(ISqlApi, BatchRequest), RowSet>((this._SqlApi, request));
				sqlResponse.Data = data?.Rows is not null ? data.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("InsertBatch{0}")]
		[GraphDescription("INSERT INTO {0} ...")]
		public async Task<SqlResponse<T>> InsertBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array<string>.Empty;
			var request = new BatchRequest
			{
				Table = this.Table,
				Input = batch.MapRowSet(columns),
				Insert = columns
			};
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "INSERTED");

			var sqlResponse = new SqlResponse<T>();
			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;
			if (selections.Any(selection => selection.Left(nameof(SqlResponse<T>.SQL))))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<BatchRequest, string>(request);
			if (request.Output.Any())
			{
				var data = await this._Mediator.ApplyRulesAsync<(ISqlApi, BatchRequest), RowSet>((this._SqlApi, request));
				sqlResponse.Data = data?.Rows is not null ? data.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("Select{0}")]
		[GraphDescription("SELLECT ... FROM {0} HAVING ... WHERE ... ORDER BY ...")]
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
				From = this.Table,
				Having = having,
				Where = where
			};
			parameters.Do(parameter => request.Parameters[parameter.Name] = parameter.Value);
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Select[selection] = selection);
			orderBy.Do(_ => request.OrderBy[_.Expression] = _.Sort);

			var sqlResponse = new SqlResponse<T>();
			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;
			if (selections.Any(selection => selection.Left(nameof(SqlResponse<T>.SQL))))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<SelectRequest, string>(request);
			if (request.Select.Any())
			{
				var data = await this._Mediator.ApplyRulesAsync<(ISqlApi, SelectRequest), RowSet>((this._SqlApi, request));
				sqlResponse.Data = data?.Rows is not null ? data.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("Update{0}")]
		[GraphDescription("UPDATE {0} SET ... WHERE ...")]
		public async Task<SqlResponse<T>> Update([AllowNull] Parameter[] parameters, T set, string where, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>>(nameof(set)).Keys.ToArray();
			var request = new UpdateRequest
			{
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
				var property = TypeOf<T>.Properties.Values.First(property =>
					property.Attributes.First<GraphNameAttribute>()?.Name.Is(column) is true) ?? TypeOf<T>.Properties.Get(column)!;
				request.Set[property.Name] = property.GetValue(set);
			});

			var sqlResponse = new SqlResponse<T>();
			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;
			if (selections.Any(selection => selection.Left(nameof(SqlResponse<T>.SQL))))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<UpdateRequest, string>(request);
			if (request.Output.Any())
			{
				var data = await this._Mediator.ApplyRulesAsync<(ISqlApi, UpdateRequest), RowSet>((this._SqlApi, request));
				sqlResponse.Data = data?.Rows is not null ? data.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}

		[GraphName("UpdateBatch{0}")]
		[GraphDescription("UPDATE {0} SET ...")]
		public async Task<SqlResponse<T>> UpdateBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var selections = context.GetQuerySelections().ToArray();
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array<string>.Empty;
			var request = new BatchRequest
			{
				Input = batch.MapRowSet(columns),
				Table = this.Table,
				Update = columns
			};
			selections
				.If(selection => selection.Left(nameof(SqlResponse<T>.Data)))
				.To(selection => selection.TrimStart($"{nameof(SqlResponse<T>.Data)}.")!)
				.Do(selection => request.Output[selection] = "INSERTED");

			var sqlResponse = new SqlResponse<T>();
			if (selections.Has(nameof(SqlResponse<T>.Table)))
				sqlResponse.Table = this.Table;
			if (selections.Any(selection => selection.Left(nameof(SqlResponse<T>.SQL))))
				sqlResponse.SQL = await this._Mediator.ApplyRulesAsync<BatchRequest, string>(request);
			if (request.Output.Any())
			{
				var data = await this._Mediator.ApplyRulesAsync<(ISqlApi, BatchRequest), RowSet>((this._SqlApi, request));
				sqlResponse.Data = data?.Rows is not null ? data.MapModels<T>() : Array<T>.Empty;
			}
			return sqlResponse;
		}
	}
}
