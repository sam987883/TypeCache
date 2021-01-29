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

namespace TypeCache.GraphQL
{
	public class SqlApi<T>
		where T : class, new()
	{
		private readonly DbProviderFactory _DbProviderFactory;
		private readonly string _ConnectionString;
		private readonly IMediator _Mediator;

		public string TableName { get; }

		public SqlApi(string databaseProvider, string connectionString, IMediator mediator, string tableName)
		{
			this._DbProviderFactory = DbProviderFactories.GetFactory(databaseProvider);
			this._ConnectionString = connectionString;
			this._Mediator = mediator;

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
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true
						|| property.Name.Is(selection));
					return new OutputExpression
					{
						Expression = $"DELETED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Where = where
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, DeleteRequest, RowSet>(dbConnection, request);
			return response.Result.Rows != null ? response.Result.Map<T>() : Array.Empty<T>();
		}

		[Graph(name: "Delete{0}_SQL")]
		public async Task<string> DeleteSQL(string where, string[] output, [AllowNull] Parameter[] parameters, IResolveFieldContext context)
		{
			var request = new DeleteRequest
			{
				From = this.TableName,
				Output = output?.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"DELETED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(output.Length),
				Parameters = parameters,
				Where = where
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, DeleteRequest, string>(dbConnection, request);
			return response.Result!;
		}

		[Graph(name: "DeleteBatch{0}", description: "DELETE FROM {0} ...")]
		public async Task<T[]> DeleteBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var request = new BatchRequest
			{
				Delete = true,
				Table = this.TableName,
				Input = batch.Map(TypeOf<T>.Properties.Keys.ToArray()),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"DELETED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count)
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, BatchRequest, RowSet>(dbConnection, request);
			return response.Result.Rows != null ? response.Result.Map<T>() : Array.Empty<T>();
		}

		[Graph(name: "DeleteBatch{0}_SQL")]
		public async Task<string> DeleteBatchSQL([NotNull] T[] batch, IResolveFieldContext context)
		{
			var request = new BatchRequest
			{
				Delete = true,
				Table = this.TableName,
				Input = batch.Map(TypeOf<T>.Properties.Keys.ToArray()),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"DELETED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count)
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, BatchRequest, string>(dbConnection, request);
			return response.Result!;
		}

		[Graph(name: "InsertBatch{0}", description: "INSERT INTO {0} ...")]
		public async Task<T[]> InsertBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Table = this.TableName,
				Input = batch.Map(columns),
				Insert = columns,
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"INSERTED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count)
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, BatchRequest, RowSet>(dbConnection, request);
			return response.Result.Rows != null ? response.Result.Map<T>() : Array.Empty<T>();
		}

		[Graph(name: "InsertBatch{0}_SQL")]
		public async Task<string> InsertBatchSQL([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Table = this.TableName,
				Input = batch.Map(columns),
				Insert = columns,
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"INSERTED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count)
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, BatchRequest, string>(dbConnection, request);
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
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Where = where
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, SelectRequest, RowSet>(dbConnection, request);
			return response.Result.Rows != null ? response.Result.Map<T>() : Array.Empty<T>();
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
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Where = where
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, SelectRequest, string>(dbConnection, request);
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
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"INSERTED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Set = columns.To(column =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(column) == true)
						?? TypeOf<T>.Properties.Get(column);
					return new ColumnSet
					{
						Column = column,
						Expression = property![set]
					};
				}).ToArray(),
				Table = this.TableName,
				Where = where
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, UpdateRequest, RowSet>(dbConnection, request);
			return response.Result.Rows != null ? response.Result.Map<T>() : Array.Empty<T>();
		}

		[Graph(name: "Update{0}_SQL")]
		public async Task<string> UpdateSQL([AllowNull] Parameter[] parameters, T set, string where, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>>(nameof(set)).Keys.ToArray();
			var request = new UpdateRequest
			{
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"INSERTED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count),
				Parameters = parameters,
				Set = columns.To(column =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(column) == true)
						?? TypeOf<T>.Properties.Get(column);
					return new ColumnSet
					{
						Column = column,
						Expression = property![set]
					};
				}).ToArray(),
				Table = this.TableName,
				Where = where
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, UpdateRequest, string>(dbConnection, request);
			return response.Result!;
		}

		[Graph(name: "UpdateBatch{0}", description: "UPDATE {0} SET ...")]
		public async Task<T[]> UpdateBatch([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Input = batch.Map(columns),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"INSERTED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count),
				Table = this.TableName,
				Update = columns
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, BatchRequest, RowSet>(dbConnection, request);
			return response.Result.Rows != null ? response.Result.Map<T>() : Array.Empty<T>();
		}

		[Graph(name: "UpdateBatch{0}_SQL")]
		public async Task<string> UpdateBatchSQL([NotNull] T[] batch, IResolveFieldContext context)
		{
			var columns = context.GetArgument<IDictionary<string, object>[]>(nameof(batch)).First()?.Keys.ToArray() ?? Array.Empty<string>();
			var request = new BatchRequest
			{
				Input = batch.Map(columns),
				Output = context.SubFields.Keys.To(selection =>
				{
					var property = TypeOf<T>.Properties.Values.First(property => property!.Attributes.First<Attribute, GraphAttribute>()?.Name.Is(selection) == true)
						?? TypeOf<T>.Properties.Get(selection);
					return new OutputExpression
					{
						Expression = $"INSERTED.[{property!.Name}]",
						As = selection
					};
				}).ToArray(context.SubFields.Count),
				Table = this.TableName,
				Update = columns
			};

			await using var dbConnection = this._DbProviderFactory.CreateConnection(this._ConnectionString);
			var response = await this._Mediator.ApplyRuleAsync<DbConnection, BatchRequest, string>(dbConnection, request);
			return response.Result!;
		}
	}
}
