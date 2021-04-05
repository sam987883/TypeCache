// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types
{
	public class GraphSchema : Schema
	{
		private readonly IMediator _Mediator;
		private readonly IServiceProvider _ServiceProvider;
		private readonly ISqlApi _SqlApi;

		public GraphSchema(IServiceProvider provider, IMediator mediator, ISqlApi? sqlApi, Action<GraphSchema> addEndpoints) : base(provider)
		{
			this.Query = new ObjectGraphType { Name = nameof(this.Query) };
			this.Mutation = new ObjectGraphType { Name = nameof(this.Mutation) };

			this._Mediator = mediator;
			this._ServiceProvider = provider;
			this._SqlApi = sqlApi!;

			addEndpoints(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SqlApi<T> CreateSqlApi<T>(ObjectSchema objectSchema)
			where T : class, new()
			=> new SqlApi<T>(this._Mediator, this._SqlApi, objectSchema.Name);

		/// <summary>
		/// Use this to create GraphQL endpoints based on methods defined in the specified class tagged with either <see cref="GraphMutationAttribute"/> or <see cref="GraphQueryAttribute"/>
		/// </summary>
		public void AddHandlerEndpoints<T>()
			where T : notnull
		{
			var handler = this._ServiceProvider.GetRequiredService<T>();
			TypeOf<T>.Methods.Values.Gather().If(method => method!.Attributes.Any<GraphMutationAttribute>()).Do(method =>
			{
				if (method!.Return.IsVoid)
					throw new NotSupportedException("Queries cannot have a return type that is void, Task or ValueTask.");

				this.Mutation.AddField(method, handler);
			});

			TypeOf<T>.Methods.Values.Gather().If(method => method!.Attributes.Any<GraphQueryAttribute>()).Do(method =>
			{
				if (method!.Return.IsVoid)
					throw new NotSupportedException("Mutations cannot have a return type that is void, Task or ValueTask.");

				this.Query.AddField(method, handler);
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
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiRules"/></code>
		/// </summary>
		public void AddSqlApiEndpoints<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			if (objectSchema.Type == ObjectType.Table)
			{
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["Delete"][0], sqlApi);
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["DeleteBatch"][0], sqlApi);
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["InsertBatch"][0], sqlApi);
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["Update"][0], sqlApi);
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["UpdateBatch"][0], sqlApi);
			}

			this.Query.AddField(TypeOf<SqlApi<T>>.Methods["Select"][0], sqlApi);
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
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApi"/></code>
		/// </summary>
		public void AddSqlOnlyEndpoints<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			if (objectSchema.Type == ObjectType.Table)
			{
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["DeleteSQL"][0], sqlApi);
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["DeleteBatchSQL"][0], sqlApi);
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["InsertBatchSQL"][0], sqlApi);
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["UpdateSQL"][0], sqlApi);
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["UpdateBatchSQL"][0], sqlApi);
			}

			this.Query.AddField(TypeOf<SqlApi<T>>.Methods["SelectSQL"][0], sqlApi);
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Item}</term> <description>Deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Item}</term> <description>Deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiDeleteRules"/></code>
		/// </summary>
		public void AddDeleteEndpoints<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["Delete"].Do(method => this.Mutation.AddField(method, sqlApi));
			TypeOf<SqlApi<T>>.Methods["DeleteBatch"].Do(method => this.Mutation.AddField(method, sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Item}-SQL</term> <description>Returns SQL that deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Item}-SQL</term> <description>Returns SQL that deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiDeleteRules"/></code>
		/// </summary>
		public void AddDeleteSqlEndpoints<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["DeleteSQL"].Do(method => this.Query.AddField(method, sqlApi));
			TypeOf<SqlApi<T>>.Methods["DeleteBatchSQL"].Do(method => this.Query.AddField(method, sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Insert-Batch-{Item}</term> <description>Inserts a batch of records.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiInsertRules"/></code>
		/// </summary>
		public void AddInsertEndpoint<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["InsertBatch"].Do(method => this.Mutation.AddField(method, sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Mutation: Insert-Batch-{Item}-SQL</term> <description>Returns SQL that inserts a batch of records.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiInsertRules"/></code>
		/// </summary>
		public void AddInsertSqlEndpoint<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["InsertBatchSQL"].Do(method => this.Query.AddField(method, sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Query: Select-{Item}</term> <description>Selects records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		public void AddSelectEndpoint<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["Select"].Do(method => this.Query.AddField(method, sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Query: Select-{Item}-SQL</term> <description>Returns SQL that selects records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		public void AddSelectSqlEndpoint<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["SelectSQL"].Do(method => this.Query.AddField(method, sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Update-{Item}</term> <description>Updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-Batch-{Item}</term> <description>Updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiUpdateRules"/></code>
		/// </summary>
		public void AddUpdateEndpoints<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["Update"].Do(method => this.Mutation.AddField(method, sqlApi));
			TypeOf<SqlApi<T>>.Methods["UpdateBatch"].Do(method => this.Mutation.AddField(method, sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints that return SQL for testing purposes:
		/// <list type="table">
		/// <item><term>Mutation: Update-{Item}-SQL</term> <description>Returns SQL that updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-Batch-{Item}-SQL</term> <description>Returns SQL that updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiUpdateRules"/></code>
		/// </summary>
		public void AddUpdateSqlEndpoints<T>(string table)
			where T : class, new()
		{
			var objectSchema = this._SqlApi.GetObjectSchema(table);
			var sqlApi = this.CreateSqlApi<T>(objectSchema);

			TypeOf<SqlApi<T>>.Methods["UpdateSQL"].Do(method => this.Query.AddField(method, sqlApi));
			TypeOf<SqlApi<T>>.Methods["UpdateBatchSQL"].Do(method => this.Query.AddField(method, sqlApi));
		}
	}
}
