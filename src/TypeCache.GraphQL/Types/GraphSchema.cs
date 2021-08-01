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
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SQL;
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
		private SqlApi<T> CreateSqlApi<T>(string dataSource, string table)
			where T : class, new()
			=> new SqlApi<T>(this._Mediator, dataSource, table);

		/// <summary>
		/// Adds GraphQL endpoints based on class methods decorated with the following attributes:
		/// <list type="bullet">
		/// <item><see cref="GraphQueryAttribute"/></item>
		/// <item><see cref="GraphMutationAttribute"/></item>
		/// <item><see cref="GraphSubqueryAttribute"/></item>
		/// <item><see cref="GraphSubqueryBatchAttribute"/></item>
		/// <item><see cref="GraphSubqueryCollectionAttribute"/></item>
		/// </list>
		/// </summary>
		/// <typeparam name="T">The class containing the decorated methods that will be converted into GraphQL endpoints.</typeparam>
		/// <returns>The added <see cref="FieldType"/>(s).</returns>
		public FieldType[] AddEndpoints<T>()
		{
			var fieldTypes = new List<FieldType>();

			var methods = TypeOf<T>.Methods.Values.Gather().ToArray();
			fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQueryAttribute>()).To(this.AddQuery));
			fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphMutationAttribute>()).To(this.AddMutation));
			fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphSubqueryAttribute>()).To(method =>
			{
				var parentType = method.Attributes.First<GraphSubqueryAttribute>()!.ParentType;
				var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
				var resolver = (IFieldResolver)typeof(ItemLoaderFieldResolver<>).MakeGenericType(parentType).GetTypeMember().Create(method, handler, this._DataLoader);
				return this.Query.AddField(method.ToFieldType(resolver));
			}));
			fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphSubqueryBatchAttribute>()).To(method =>
			{
				var (parentProperty, childProperty) = getParentChildProperties(method);
				return this.AddSubqueryBatch(method, parentProperty, childProperty);
			}));
			fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphSubqueryCollectionAttribute>()).To(method =>
			{
				var (parentProperty, childProperty) = getParentChildProperties(method);
				return this.AddSubqueryCollection(method, parentProperty, childProperty);
			}));

			return fieldTypes.ToArray();

			(PropertyMember, PropertyMember) getParentChildProperties(MethodMember method)
			{
				if (method.Return.Type.Kind is not Kind.Collection)
					throw new ArgumentException($"{nameof(AddEndpoints)}: [{nameof(method)}] must return a collection instead of [{method.Return.Type.Name}].");

				var attribute = method.Attributes.First<GraphSubqueryCollectionAttribute>()!;
				var parentType = attribute.ParentType.GetTypeMember();
				var key = attribute.Key;
				var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;

				var parentKeyProperty = parentType.Properties.Values.FirstValue(property => property.Attributes.First<GraphKeyAttribute>()?.Name.Is(key) is true);
				if (parentKeyProperty is null)
					throw new ArgumentException($"AddEndpoints<{TypeOf<T>.Name}>: The parent model [{parentType.Name}] requires a [{nameof(GraphKeyAttribute)}] with a {nameof(key)} of \"{key}\".");

				var childType = method.Return.Type.EnclosedType!.Value;
				var childKeyProperty = childType.Properties.Values.FirstValue(property => property.Attributes.First<GraphKeyAttribute>()?.Name.Is(key) is true);
				if (childKeyProperty is null)
					throw new ArgumentException($"AddEndpoints<{TypeOf<T>.Name}>: The child model [{childType.Name}] requires a [{nameof(GraphKeyAttribute)}] with a {nameof(key)} of \"{key}\".");

				return (parentKeyProperty.Value, childKeyProperty.Value);
			}
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <typeparam name="T">The class that holds the instance or static method to create a mutation endpoint from.</typeparam>
		/// <param name="method">The name of the method or set of methods to use (each method must have a unique GraphName).</param>
		/// <returns>The added <see cref="FieldType"/>(s).</returns>
		public FieldType[] AddMutation<T>(string method)
			where T : class
		{
			var methods = TypeOf<T>.Methods[method];
			if (methods.Any())
			{
				var handler = methods.Any(method => !method.Static) ? this._ServiceProvider.GetRequiredService<T>() : null;
				return methods.To(method =>
				{
					var resolver = new MethodFieldResolver(method, handler);
					return this.Mutation.AddField(method.ToFieldType(resolver));
				}).ToArray();
			}
			return Array<FieldType>.Empty;
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <param name="method">Graph endpoint implementation</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		public FieldType AddMutation(MethodMember method)
		{
			var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
			var resolver = new MethodFieldResolver(method, handler);
			return this.Mutation.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <typeparam name="T">The class that holds the instance or static method to create a query endpoint from.</typeparam>
		/// <param name="method">The name of the method or set of methods to use (each method must have a unique GraphName).</param>
		/// <returns>The added <see cref="FieldType"/>(s).</returns>
		public FieldType[] AddQuery<T>(string method)
			where T : class
		{
			var methods = TypeOf<T>.Methods[method];
			if (methods.Any())
			{
				var handler = methods.Any(method => !method.Static) ? this._ServiceProvider.GetRequiredService<T>() : null;
				return methods.To(instanceMethod =>
				{
					var resolver = new MethodFieldResolver(instanceMethod, handler);
					return this.Query.AddField(instanceMethod.ToFieldType(resolver));
				}).ToArray();
			}
			return Array<FieldType>.Empty;
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
		public FieldType AddQuery(MethodMember method)
		{
			var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
			var resolver = new MethodFieldResolver(method, handler);
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
		public FieldType AddSubquery<T>(MethodMember method)
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
		public FieldType AddSubqueryBatch<PARENT, CHILD, KEY>(MethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
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
		public FieldType AddSubqueryBatch(MethodMember method, PropertyMember parentPropertyKey, PropertyMember childPropertyKey)
		{
			var handler = this._ServiceProvider.GetRequiredService(method.Type);

			if (parentPropertyKey.PropertyType != childPropertyKey.PropertyType)
				throw new ArgumentException($"{nameof(AddSubquery)}: Expected properties [{parentPropertyKey.Name}] and [{childPropertyKey.Name}] to have the same type; instead of [{parentPropertyKey.PropertyType.Name}] and [{childPropertyKey.PropertyType.Name}].");

			var resolverType = typeof(BatchLoaderFieldResolver<,,>).MakeGenericType(parentPropertyKey.Type, childPropertyKey.Type, childPropertyKey.PropertyType);

			var resolver = (IFieldResolver)resolverType.GetTypeMember().Create(method, handler, this._DataLoader, parentPropertyKey, childPropertyKey);
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
		public FieldType AddSubqueryCollection<PARENT, CHILD, KEY>(MethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
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
		public FieldType AddSubqueryCollection(MethodMember method, PropertyMember parentPropertyKey, PropertyMember childPropertyKey)
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
		public void AddSqlApiEndpoints<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));

			var objectSchema = this._SqlApi.GetObjectSchema(dataSource, table);
			var sqlApi = this.CreateSqlApi<T>(dataSource, objectSchema.Name);

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
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Item}</term> <description>Deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Item}</term> <description>Deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiDeleteRules"/></code>
		/// </summary>
		public void AddDeleteEndpoints<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));

			var objectSchema = this._SqlApi.GetObjectSchema(dataSource, table);
			var sqlApi = this.CreateSqlApi<T>(dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Delete"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["DeleteBatch"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Insert-Batch-{Item}</term> <description>Inserts a batch of records.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiInsertRules"/></code>
		/// </summary>
		public void AddInsertEndpoint<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));

			var objectSchema = this._SqlApi.GetObjectSchema(dataSource, table);
			var sqlApi = this.CreateSqlApi<T>(dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["InsertBatch"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Query: Select-{Item}</term> <description>Selects records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		public void AddSelectEndpoint<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));

			var objectSchema = this._SqlApi.GetObjectSchema(dataSource, table);
			var sqlApi = this.CreateSqlApi<T>(dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Select"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
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
		public void AddUpdateEndpoints<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));

			var objectSchema = this._SqlApi.GetObjectSchema(dataSource, table);
			var sqlApi = this.CreateSqlApi<T>(dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Update"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
			TypeOf<SqlApi<T>>.Methods["UpdateBatch"].Do(method => this.Mutation.AddField(method.ToFieldType(sqlApi)));
		}
	}
}
