// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GraphQL;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL
{
	public class SqlApi<T>
		where T : class, new()
	{
		private readonly IMediator _Mediator;
		private readonly ISqlApi _SqlApi;

		public string TableName { get; }

		public SqlApi(IMediator mediator, ISqlApi sqlApi, string tableName)
		{
			this._Mediator = mediator;
			this._SqlApi = sqlApi;

			this.TableName = tableName;
		}

		[Graph(name: "Delete{0}", description: "DELETE FROM {0} WHERE ...")]
		public async Task<T[]> Delete(string where, [AllowNull] Parameter[] parameters, IResolveFieldContext context)
		{
			var request = new DeleteRequest
			{
				From = this.TableName,
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true
						|| property.Name.Is(selection))!;
					return new OutputExpression($"DELETED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Where = where
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, DeleteRequest, RowSet>(this._SqlApi, request);
			return response.Result?.Rows is not null ? response.Result.MapModels<T>() : Array.Empty<T>();
		}

		[Graph(name: "Delete{0}_SQL")]
		public async Task<string> DeleteSQL(string where, string[] output, [AllowNull] Parameter[] parameters, IResolveFieldContext context)
		{
			var request = new DeleteRequest
			{
				From = this.TableName,
				Output = output?.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"DELETED.[{property.Name}]", selection);
				}).ToArray(output.Length),
				Parameters = parameters,
				Where = where
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, DeleteRequest, string>(this._SqlApi, request);
			return response.Result!;
		}

		[Graph(name: "DeleteBatch{0}", description: "DELETE FROM {0} ...")]
		public async Task<T[]> DeleteBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var request = new BatchRequest
			{
				Delete = true,
				Table = this.TableName,
				Input = batch.MapRowSet(TypeOf<T>.Properties.Keys.ToArray()),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"DELETED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count)
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, BatchRequest, RowSet>(this._SqlApi, request);
			return response.Result?.Rows is not null ? response.Result.MapModels<T>() : Array.Empty<T>();
		}

		[Graph(name: "DeleteBatch{0}_SQL")]
		public async Task<string> DeleteBatchSQL([NotNull] T[] batch, IResolveFieldContext context)
		{
			var request = new BatchRequest
			{
				Delete = true,
				Table = this.TableName,
				Input = batch.MapRowSet(TypeOf<T>.Properties.Keys.ToArray()),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"DELETED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count)
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, BatchRequest, string>(this._SqlApi, request);
			return response.Result!;
		}

		[Graph(name: "InsertBatch{0}", description: "INSERT INTO {0} ...")]
		public async Task<T[]> InsertBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Table = this.TableName,
				Input = batch.MapRowSet(columns),
				Insert = columns,
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"INSERTED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count)
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, BatchRequest, RowSet>(this._SqlApi, request);
			return response.Result?.Rows is not null ? response.Result.MapModels<T>() : Array.Empty<T>();
		}

		[Graph(name: "InsertBatch{0}_SQL")]
		public async Task<string> InsertBatchSQL([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Table = this.TableName,
				Input = batch.MapRowSet(columns),
				Insert = columns,
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"INSERTED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count)
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, BatchRequest, string>(this._SqlApi, request);
			return response.Result!;
		}

		[Graph(name: "Select{0}", description: "SELLECT ... FROM {0} HAVING ... WHERE ... ORDER BY ...")]
		public async Task<T[]> Select(string having, ColumnSort[] orderBy, string where, [AllowNull] Parameter[] parameters, IResolveFieldContext context)
		{
			var request = new SelectRequest
			{
				From = this.TableName,
				Having = having,
				OrderBy = orderBy,
				Select = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Where = where
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, SelectRequest, RowSet>(this._SqlApi, request);
			return response.Result?.Rows is not null ? response.Result.MapModels<T>() : Array.Empty<T>();
		}

		[Graph(name: "Select{0}_SQL")]
		public async Task<string> SelectSQL(string having, ColumnSort[] orderBy, string where, [AllowNull] Parameter[] parameters, IResolveFieldContext context)
		{
			var request = new SelectRequest
			{
				From = this.TableName,
				Having = having,
				OrderBy = orderBy,
				Select = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Where = where
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, SelectRequest, string>(this._SqlApi, request);
			return response.Result!;
		}

		[Graph(name: "Update{0}", description: "UPDATE {0} SET ... WHERE ...")]
		public async Task<T[]> Update([AllowNull] Parameter[] parameters, T set, string where, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>>(nameof(set)).Keys.ToArray();
			var request = new UpdateRequest
			{
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"INSERTED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Set = columns.To(column =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(column) == true)
						?? TypeOf<T>.Properties.Get(column)!;
					return new ColumnSet(column, property.GetValue!(set));
				}).ToArray(),
				Table = this.TableName,
				Where = where
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, UpdateRequest, RowSet>(this._SqlApi, request);
			return response.Result?.Rows is not null ? response.Result.MapModels<T>() : Array.Empty<T>();
		}

		[Graph(name: "Update{0}_SQL")]
		public async Task<string> UpdateSQL([AllowNull] Parameter[] parameters, T set, string where, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>>(nameof(set)).Keys.ToArray();
			var request = new UpdateRequest
			{
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"INSERTED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Set = columns.To(column =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(column) == true)
						?? TypeOf<T>.Properties.Get(column)!;
					return new ColumnSet(column, property.GetValue!(set));
				}).ToArray(),
				Table = this.TableName,
				Where = where
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, UpdateRequest, string>(this._SqlApi, request);
			return response.Result!;
		}

		[Graph(name: "UpdateBatch{0}", description: "UPDATE {0} SET ...")]
		public async Task<T[]> UpdateBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Input = batch.MapRowSet(columns),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"INSERTED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count),
				Table = this.TableName,
				Update = columns
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, BatchRequest, RowSet>(this._SqlApi, request);
			return response.Result?.Rows is not null ? response.Result.MapModels<T>() : Array.Empty<T>();
		}

		[Graph(name: "UpdateBatch{0}_SQL")]
		public async Task<string> UpdateBatchSQL([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Input = batch.MapRowSet(columns),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection)!;
					return new OutputExpression($"INSERTED.[{property.Name}]", selection);
				}).ToArray(context.SubFields.Count),
				Table = this.TableName,
				Update = columns
			};

			var response = await this._Mediator.ApplyRuleAsync<ISqlApi, BatchRequest, string>(this._SqlApi, request);
			return response.Result!;
		}
	}
}
