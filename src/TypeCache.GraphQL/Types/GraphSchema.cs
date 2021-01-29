// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading.Tasks;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types
{
	public class GraphSchema : Schema
	{
		private readonly IMediator _Mediator;

		public GraphSchema(IServiceProvider provider, Action<GraphSchema> addEndpoints) : base(provider)
		{
			this.Query = new ObjectGraphType { Name = nameof(this.Query) };
			this.Mutation = new ObjectGraphType { Name = nameof(this.Mutation) };

			this._Mediator = provider.GetRequiredService<IMediator>();

			addEndpoints(this);
		}

		/// <summary>
		/// Use this to create GraphQL endpoints based on methods defined in the specified class tagged with either <see cref="GraphMutationAttribute"/> or <see cref="GraphQueryAttribute"/>
		/// </summary>
		public void AddHandlerEndpoints<T>()
			where T : class, new()
		{
			var handler = TypeOf<T>.Create();
			TypeOf<T>.Methods.Values.Gather().If(method => method!.Attributes.Any<Attribute, GraphMutationAttribute>()).Do(method =>
			{
				if (method!.IsVoid)
					throw new NotSupportedException("Queries cannot have a return type that is void, Task or ValueTask.");

				this.Mutation.AddField(method.CreateHandlerFieldType(handler));
			});

			TypeOf<T>.Methods.Values.Gather().If(method => method!.Attributes.Any<Attribute, GraphQueryAttribute>()).Do(method =>
			{
				if (method!.IsVoid)
					throw new NotSupportedException("Mutations cannot have a return type that is void, Task or ValueTask.");

				this.Query.AddField(method.CreateHandlerFieldType(handler));
			});
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Item}</term> <description>Deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Item}</term> <description>Deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// <item><term>Mutation: Insert-Batch-{Item}</term> <description>Inserts a batch of records.</description></item>
		/// <item><term>Query: Select-{Item}</term> <description>Selects records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-{Item}</term> <description>Updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-Batch-{Item}</term> <description>Updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApi"/></code>
		/// </summary>
		public void AddSqlApiEndpoints<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = this.GetObjectSchema(databaseProvider, connectionString, table).Result;
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, table);

			if (objectSchema.Type == ObjectType.Table)
			{
				var mutation = (ObjectGraphType)this.Mutation;
				mutation.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["Delete"][0], sqlApi);
				mutation.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["DeleteBatch"][0], sqlApi);
				mutation.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["InsertBatch"][0], sqlApi);
				mutation.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["Update"][0], sqlApi);
				mutation.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["UpdateBatch"][0], sqlApi);
			}

			var query = (ObjectGraphType)this.Query;
			query.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["Select"][0], sqlApi);
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Item}-SQL</term> <description>Returns SQL that deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Item}-SQL</term> <description>Returns SQL that deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// <item><term>Mutation: Insert-Batch-{Item}-SQL</term> <description>Returns SQL that inserts a batch of records.</description></item>
		/// <item><term>Query: Select-{Item}-SQL</term> <description>Returns SQL that selects records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-{Item}-SQL</term> <description>Returns SQL that updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-Batch-{Item}-SQL</term> <description>Returns SQL that updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApi"/></code>
		/// </summary>
		public void AddSqlOnlyEndpoints<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = this.GetObjectSchema(databaseProvider, connectionString, table).Result;
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			var query = (ObjectGraphType)this.Query;
			if (objectSchema.Type == ObjectType.Table)
			{
				query.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["DeleteSQL"][0], sqlApi);
				query.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["DeleteBatchSQL"][0], sqlApi);
				query.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["InsertBatchSQL"][0], sqlApi);
				query.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["UpdateSQL"][0], sqlApi);
				query.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["UpdateBatchSQL"][0], sqlApi);
			}

			query.AddSqlApiFieldType(TypeOf<SqlApi<T>>.Methods["SelectSQL"][0], sqlApi);
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Item}</term> <description>Deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Item}</term> <description>Deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiDelete"/></code>
		/// </summary>
		public async ValueTask AddDeleteEndpoints<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Delete"].Do(method => this.Mutation.AddField(method.CreateSqlApiFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["DeleteBatch"].Do(method => this.Mutation.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Item}-SQL</term> <description>Returns SQL that deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Item}-SQL</term> <description>Returns SQL that deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiDelete"/></code>
		/// </summary>
		public async ValueTask AddDeleteSqlEndpoints<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["DeleteSQL"].Do(method => this.Query.AddField(method.CreateSqlApiFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["DeleteBatchSQL"].Do(method => this.Query.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Insert-Batch-{Item}</term> <description>Inserts a batch of records.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiInsert"/></code>
		/// </summary>
		public async ValueTask AddInsertEndpoint<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["InsertBatch"].Do(method => this.Mutation.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Mutation: Insert-Batch-{Item}-SQL</term> <description>Returns SQL that inserts a batch of records.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiInsert"/></code>
		/// </summary>
		public async ValueTask AddInsertSqlEndpoint<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["InsertBatchSQL"].Do(method => this.Query.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Query: Select-{Item}</term> <description>Selects records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelect"/></code>
		/// </summary>
		public async ValueTask AddSelectEndpoint<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Select"].Do(method => this.Query.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Query: Select-{Item}-SQL</term> <description>Returns SQL that selects records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelect"/></code>
		/// </summary>
		public async ValueTask AddSelectSqlEndpoint<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["SelectSQL"].Do(method => this.Query.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Update-{Item}</term> <description>Updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-Batch-{Item}</term> <description>Updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiUpdate"/></code>
		/// </summary>
		public async ValueTask AddUpdateEndpoints<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Update"].Do(method => this.Mutation.AddField(method.CreateSqlApiFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["UpdateBatch"].Do(method => this.Mutation.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Mutation: Update-{Item}-SQL</term> <description>Returns SQL that updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-Batch-{Item}-SQL</term> <description>Returns SQL that updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires calls to:</i>
		/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
		/// <code><see cref="TypeCache.Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiUpdate"/></code>
		/// </summary>
		public async ValueTask AddUpdateSqlEndpoints<T>(string databaseProvider, string connectionString, string table)
			where T : class, new()
		{
			var objectSchema = await this.GetObjectSchema(databaseProvider, connectionString, table);
			var sqlApi = new SqlApi<T>(databaseProvider, connectionString, this._Mediator, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["UpdateSQL"].Do(method => this.Query.AddField(method.CreateSqlApiFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["UpdateBatchSQL"].Do(method => this.Query.AddField(method.CreateSqlApiFieldType(sqlApi)));
		}

		private async ValueTask<ObjectSchema> GetObjectSchema(string databaseProvider, string connectionString, string table)
		{
			var dbProviderFactory = DbProviderFactories.GetFactory(databaseProvider);
			await using var dbConnection = dbProviderFactory.CreateConnection(connectionString);

			await dbConnection.OpenAsync();
			var objectSchema = dbConnection.GetObjectSchema(table);
			await dbConnection.CloseAsync();

			return objectSchema;
		}
	}
}
