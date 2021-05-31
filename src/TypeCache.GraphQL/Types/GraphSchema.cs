// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types
{
	public class GraphSchema : Schema
	{
		private readonly IDataLoaderContextAccessor _DataLoader;
		private readonly IMediator _Mediator;
		private readonly IServiceProvider _ServiceProvider;
		private readonly ISqlApi _SqlApi;

		public GraphSchema(IServiceProvider provider, IMediator mediator, ISqlApi? sqlApi, IDataLoaderContextAccessor dataLoader, Action<GraphSchema> addEndpoints) : base(provider)
		{
			this.Query = new ObjectGraphType { Name = nameof(Query) };
			this.Mutation = new ObjectGraphType { Name = nameof(Mutation) };

			this._DataLoader = dataLoader;
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
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <param name="method">Graph endpoint implementation</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddMutation(InstanceMethodMember method)
		{
			var handler = this._ServiceProvider.GetRequiredService(method.Type);
			var resolver = new InstanceMethodFieldResolver(method, handler);
			return this.Mutation.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="method">Graph endpoint implementation</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddMutation(StaticMethodMember method)
		{
			var resolver = new StaticMethodFieldResolver(method);
			return this.Mutation.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="method">Graph endpoint implementation</param>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddQuery(InstanceMethodMember method)
		{
			var handler = this._ServiceProvider.GetRequiredService(method.Type);
			var resolver = new InstanceMethodFieldResolver(method, handler);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="method">Graph endpoint implementation</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddQuery(StaticMethodMember method)
		{
			var resolver = new StaticMethodFieldResolver(method);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a single item.<br />
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item>T</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <param name="method">Graph endpoint implementation</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddSubquery<T>(InstanceMethodMember method)
			where T : class
		{
			var handler = this._ServiceProvider.GetRequiredService(method.Type);
			var resolver = new ItemLoaderFieldResolver<T>(method, handler, this._DataLoader);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a single item mapped to the parent type by a key property.<br />
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item>PARENT</item>
		/// <item>IEnumerable&lt;KEY&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <typeparam name="PARENT">The parent type to add the endpount to</typeparam>
		/// <typeparam name="CHILD">The mapped child type to be returned</typeparam>
		/// <typeparam name="KEY">The type of the key mapping between the parent and child types.</typeparam>
		/// <param name="method">Graph endpoint implementation</param>
		/// <param name="getParentKey">Gets the key value from the parent instance</param>
		/// <param name="getChildKey">Gets the key value from the child instance</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddSubquery<PARENT, CHILD, KEY>(InstanceMethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
			where PARENT : class
		{
			var handler = this._ServiceProvider.GetRequiredService(method.Type);
			var resolver = new BatchLoaderFieldResolver<PARENT, CHILD, KEY>(method, handler, this._DataLoader, getParentKey, getChildKey);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a single item mapped to the parent type by a key property.<br />
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item>PARENT</item>
		/// <item>IEnumerable&lt;KEY&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <param name="method">Graph endpoint implementation</param>
		/// <param name="parentPropertyKey">Parent property containing the key value</param>
		/// <param name="childPropertyKey">Child property containing the key value</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddSubquery(InstanceMethodMember method, InstancePropertyMember parentPropertyKey, InstancePropertyMember childPropertyKey)
		{
			var handler = this._ServiceProvider.GetRequiredService(method.Type);

			if (parentPropertyKey.PropertyType != childPropertyKey.PropertyType)
				throw new ArgumentException($"{nameof(AddSubquery)}: Expected properties [{parentPropertyKey.Name}] and [{childPropertyKey.Name}] to have the same type; instead of [{parentPropertyKey.PropertyType.Name}] and [{childPropertyKey.PropertyType.Name}].");

			var resolverType = typeof(BatchLoaderFieldResolver<,,>).MakeGenericType(parentPropertyKey.Type, childPropertyKey.Type, childPropertyKey.PropertyType);

			var resolver = (IFieldResolver)resolverType.GetConstructorCache().First()!.Create!(method, handler, this._DataLoader, parentPropertyKey, childPropertyKey);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a collection of items mapped to the parent type by a key property.
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item>PARENT</item>
		/// <item>IEnumerable&lt;KEY&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <typeparam name="PARENT">The parent type to add the endpount to</typeparam>
		/// <typeparam name="CHILD">The mapped child type to be returned</typeparam>
		/// <typeparam name="KEY">The type of the key mapping between the parent and child types.</typeparam>
		/// <param name="method">Graph endpoint implementation</param>
		/// <param name="getParentKey">Gets the key value from the parent instance</param>
		/// <param name="getChildKey">Gets the key value from the child instance</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddSubqueryCollection<PARENT, CHILD, KEY>(InstanceMethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
			where PARENT : class
		{
			if (!method.Return.Type.Implements<IEnumerable<CHILD>>())
				throw new ArgumentException($"{nameof(AddSubquery)}: Expected method [{method.Name}] to have a return type of [{TypeOf<IEnumerable<CHILD>>.Name}] instead of [{method.Return.Type.Name}].");

			var handler = this._ServiceProvider.GetRequiredService(method.Type);
			var resolver = new CollectionBatchLoaderFieldResolver<PARENT, CHILD, KEY>(method, handler, this._DataLoader, getParentKey, getChildKey);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a collection of items mapped to the parent type by a key property.
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item>PARENT</item>
		/// <item>IEnumerable&lt;KEY&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
		/// <param name="method">Graph endpoint implementation</param>
		/// <param name="parentPropertyKey">Parent property containing the key value</param>
		/// <param name="childPropertyKey">Child property containing the key value</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddSubqueryCollection(InstanceMethodMember method, InstancePropertyMember parentPropertyKey, InstancePropertyMember childPropertyKey)
		{
			var handler = this._ServiceProvider.GetRequiredService(method.Type);

			if (parentPropertyKey.PropertyType != childPropertyKey.PropertyType)
				throw new ArgumentException($"{nameof(AddSubquery)}: Expected properties [{parentPropertyKey.Name}] and [{childPropertyKey.Name}] to have the same type; instead of [{parentPropertyKey.PropertyType.Name}] and [{childPropertyKey.PropertyType.Name}].");

			var resolverType = typeof(CollectionBatchLoaderFieldResolver<,,>).MakeGenericType(parentPropertyKey.Type, childPropertyKey.Type, childPropertyKey.PropertyType);
			var resolver = (IFieldResolver)resolverType.GetTypeMember().Create(method, handler, this._DataLoader, parentPropertyKey, childPropertyKey);
			return this.Query.AddField(method.ToFieldType(resolver));
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
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["Delete"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["DeleteBatch"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["InsertBatch"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["Update"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(TypeOf<SqlApi<T>>.Methods["UpdateBatch"][0].ToFieldType(sqlApi));
			}

			this.Query.AddField(TypeOf<SqlApi<T>>.Methods["Select"][0].ToFieldType(sqlApi));
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
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["DeleteSQL"][0].ToFieldType(sqlApi));
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["DeleteBatchSQL"][0].ToFieldType(sqlApi));
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["InsertBatchSQL"][0].ToFieldType(sqlApi));
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["UpdateSQL"][0].ToFieldType(sqlApi));
				this.Query.AddField(TypeOf<SqlApi<T>>.Methods["UpdateBatchSQL"][0].ToFieldType(sqlApi));
			}

			this.Query.AddField(TypeOf<SqlApi<T>>.Methods["SelectSQL"][0].ToFieldType(sqlApi));
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

			TypeOf<SqlApi<T>>.Methods["Delete"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["DeleteBatch"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
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

			TypeOf<SqlApi<T>>.Methods["DeleteSQL"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["DeleteBatchSQL"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
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

			TypeOf<SqlApi<T>>.Methods["InsertBatch"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
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

			TypeOf<SqlApi<T>>.Methods["InsertBatchSQL"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
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

			TypeOf<SqlApi<T>>.Methods["Select"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
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

			TypeOf<SqlApi<T>>.Methods["SelectSQL"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
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

			TypeOf<SqlApi<T>>.Methods["Update"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["UpdateBatch"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
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

			TypeOf<SqlApi<T>>.Methods["UpdateSQL"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["UpdateBatchSQL"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
		}
	}
}
