// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Data.Responses;
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
		private readonly ISqlApi? _SqlApi;
		private string _Version = "0";

		public GraphSchema(IServiceProvider provider, IMediator mediator, ISqlApi? sqlApi, IDataLoaderContextAccessor dataLoader, Action<GraphSchema> addEndpoints) : base(provider)
		{
			this.Query = new ObjectGraphType { Name = nameof(this.Query) };
			this.Mutation = new ObjectGraphType { Name = nameof(this.Mutation) };

			this._DataLoader = dataLoader;
			this._Mediator = mediator;
			this._ServiceProvider = provider;
			this._SqlApi = sqlApi;

			this.Query.AddField(new()
			{
				Name = nameof(this.Version),
				DefaultValue = "0",
				Description = $"The version number of this {nameof(GraphSchema)}.",
				Resolver = new FuncFieldResolver<string>(context => ((GraphSchema)context.Schema).Version),
				Type = typeof(NonNullGraphType<StringGraphType>)
			});

			addEndpoints(this);
		}

		public string Version
		{
			get => this._Version;
			set
			{
				this._Version = value;
				this.Description = $"{nameof(GraphSchema)} v{value}";
			}
		}

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
				var attribute = method.Attributes.First<GraphSubqueryCollectionAttribute>()!;
				return this.AddSubqueryBatch(method, attribute.ParentType, attribute.Key);
			}));
			fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphSubqueryCollectionAttribute>()).To(method =>
			{
				var attribute = method.Attributes.First<GraphSubqueryCollectionAttribute>()!;
				return this.AddSubqueryCollection(method, attribute.ParentType, attribute.Key);
			}));

			return fieldTypes.ToArray();
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
		/// <exception cref="ArgumentException"/>
		public FieldType[] AddMutation<T>(string method)
			=> TypeOf<T>.Methods[method].To(this.AddMutation).ToArray();

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType AddMutation(MethodMember method)
		{
			if (method.Return.IsVoid)
				throw new ArgumentException($"{nameof(AddMutation)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

			var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
			return this.Mutation!.AddField(method.ToFieldType(handler));
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
		/// <exception cref="ArgumentException"/>
		public FieldType[] AddQuery<T>(string method)
			=> TypeOf<T>.Methods[method].To(this.AddQuery).ToArray();

		/// <summary>
		/// Method parameters with the following type are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// </list>
		/// </summary>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		public FieldType AddQuery(MethodMember method)
		{
			if (method.Return.IsVoid)
				throw new ArgumentException($"{nameof(AddQuery)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

			var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
			return this.Query.AddField(method.ToFieldType(handler));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a single item.<br />
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item><typeparamref name="T"/></item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public FieldType AddSubquery<T>(MethodMember method)
		{
			var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
			var resolver = new ItemLoaderFieldResolver<T>(method, handler, this._DataLoader);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a single child item mapped by key properties.<br />
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item><paramref name="parentType"/></item>
		/// <item>IEnumerable&lt;keyType&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <param name="parentType">Gets the parent instance type.</param>
		/// <param name="key">The <see cref="GraphKeyAttribute"/> used to match parent and child type properties.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public FieldType AddSubqueryBatch(MethodMember method, Type parentType, string key)
		{
			if (method.Return.Type.Kind is not Kind.Collection)
				throw new ArgumentException($"{nameof(AddSubqueryBatch)}: [{nameof(method)}] must return a collection instead of [{method.Return.Type.Name}].");

			var parentKeyProperty = parentType.GetTypeMember().Properties.Values.FirstValue(property => property.GraphKey()?.Is(key) is true);
			if (parentKeyProperty?.Getter is null)
				throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

			var childType = method.Return.Type.EnclosedType!.Value;
			var childKeyProperty = childType.Properties.Values.FirstValue(property => property.GraphKey()?.Is(key) is true);
			if (childKeyProperty?.Getter is null)
				throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

			var getParentKey = parentKeyProperty.Value.Getter.Value.Method;
			var getChildKey = childKeyProperty.Value.Getter.Value.Method;

			var addSubQueryBatchMethod = TypeOf<GraphSchema>.Methods[nameof(AddSubqueryBatch)].If(method => method.GenericTypes == 3).FirstValue()!.Value;
			return (FieldType)addSubQueryBatchMethod.InvokeGeneric(this, new[] { (Type)parentType, (Type)childType, (Type)childKeyProperty.Value.PropertyType }, method, getParentKey, getChildKey)!;
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a single child item mapped by key properties.<br />
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item><typeparamref name="PARENT"/></item>
		/// <item>IEnumerable&lt;<typeparamref name="KEY"/>&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <typeparam name="PARENT">The parent type to add the endpount to.</typeparam>
		/// <typeparam name="CHILD">The mapped child type to be returned.</typeparam>
		/// <typeparam name="KEY">The type of the key mapping between the parent and child types.</typeparam>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <param name="getParentKey">Gets the key value from the parent instance.</param>
		/// <param name="getChildKey">Gets the key value from the child instance.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public FieldType AddSubqueryBatch<PARENT, CHILD, KEY>(MethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
			where PARENT : class
		{
			var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
			var resolver = new BatchLoaderFieldResolver<PARENT, CHILD, KEY>(method, handler, this._DataLoader, getParentKey, getChildKey);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a collection of child items mapped by key properties.<br/>
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item><paramref name="parentType"/></item>
		/// <item>IEnumerable&lt;keyType&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <param name="parentType">Gets the parent instance type.</param>
		/// <param name="key">The <see cref="GraphKeyAttribute"/> used to match parent and child type properties.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public FieldType AddSubqueryCollection(MethodMember method, Type parentType, string key)
		{
			if (method.Return.Type.Kind is not Kind.Collection)
				throw new ArgumentException($"{nameof(AddSubqueryBatch)}: [{nameof(method)}] must return a collection instead of [{method.Return.Type.Name}].");

			var parentKeyProperty = parentType.GetTypeMember().Properties.Values.FirstValue(property => property.GraphKey()?.Is(key) is true);
			if (parentKeyProperty?.Getter is null)
				throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

			var childType = method.Return.Type.EnclosedType!.Value;
			var childKeyProperty = childType.Properties.Values.FirstValue(property => property.GraphKey()?.Is(key) is true);
			if (childKeyProperty?.Getter is null)
				throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

			var getParentKey = parentKeyProperty.Value.Getter.Value.Method;
			var getChildKey = childKeyProperty.Value.Getter.Value.Method;

			var addSubqueryCollectionMethod = TypeOf<GraphSchema>.Methods[nameof(AddSubqueryCollection)].If(method => method.GenericTypes == 3).FirstValue()!.Value;
			return (FieldType)addSubqueryCollectionMethod.InvokeGeneric(this, new[] { (Type)parentType, (Type)childType, (Type)childKeyProperty.Value.PropertyType }, method, getParentKey, getChildKey)!;
		}

		/// <summary>
		/// Adds a subquery to an existing parent type that returns a collection of child items mapped by key properties.<br/>
		/// Method parameters with the following types are ignored in the schema and will have their value injected:
		/// <list type="bullet">
		/// <item><see cref="IResolveFieldContext"/></item>
		/// <item><typeparamref name="PARENT"/></item>
		/// <item>IEnumerable&lt;<typeparamref name="KEY"/>&gt;</item>
		/// </list>
		/// </summary>
		/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
		/// <typeparam name="PARENT">The parent type to add the endpount to.</typeparam>
		/// <typeparam name="CHILD">The mapped child type to be returned.</typeparam>
		/// <typeparam name="KEY">The type of the key mapping between the parent and child types.</typeparam>
		/// <param name="method">Graph endpoint implementation.</param>
		/// <param name="getParentKey">Gets the key value from the parent instance.</param>
		/// <param name="getChildKey">Gets the key value from the child instance.</param>
		/// <returns>The added <see cref="FieldType"/>.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public FieldType AddSubqueryCollection<PARENT, CHILD, KEY>(MethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
			where PARENT : class
		{
			var handler = !method.Static ? this._ServiceProvider.GetRequiredService(method.Type) : null;
			var resolver = new CollectionLoaderFieldResolver<PARENT, CHILD, KEY>(method, handler, this._DataLoader, getParentKey, getChildKey);
			return this.Query.AddField(method.ToFieldType(resolver));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Delete-{Table}</term> <description>Deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete-Batch-{Table}</term> <description>Deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// <item><term>Mutation: Insert-Batch-{Table}</term> <description>Inserts a batch of records.</description></item>
		/// <item><term>Query: Select-{Table}</term> <description>Selects records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-{Table}</term> <description>Updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update-Batch-{Table}</term> <description>Updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddSqlApiEndpoints<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var objectSchema = this._SqlApi!.GetObjectSchema(dataSource, table);
			var sqlApi = new SqlApi<T>(this._Mediator, dataSource, objectSchema.Name);
			var sqlApiMethods = TypeOf<SqlApi<T>>.Methods;

			if (objectSchema.Type == ObjectType.Table)
			{
				this.Mutation!.AddField(sqlApiMethods["Delete"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(sqlApiMethods["DeleteData"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(sqlApiMethods["InsertData"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(sqlApiMethods["Update"][0].ToFieldType(sqlApi));
				this.Mutation.AddField(sqlApiMethods["UpdateData"][0].ToFieldType(sqlApi));
			}

			this.Query.AddField(sqlApiMethods["Count"][0].ToFieldType(sqlApi));
			this.Query.AddField(sqlApiMethods["Page"][0].ToFieldType(sqlApi));
			this.Query.AddField(sqlApiMethods["Select"][0].ToFieldType(sqlApi));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Call{Procedure}</term> <description>Calls the stored procedure and returns its results.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddCallProcedureEndpoint<T>(string dataSource, string procedure)
			where T : class, new()
		{
			procedure.AssertNotBlank(nameof(procedure));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var schema = this._SqlApi!.GetObjectSchema(dataSource, procedure);
			var arguments = schema.Parameters.ToDictionary(parameter => parameter.Name, parameter =>
			{
				var type = parameter.Type.ToType();
				return type.IsValueType ? typeof(Nullable<>).MakeGenericType(type).TypeHandle : type.TypeHandle;
			}, StringComparer.OrdinalIgnoreCase);
			this.Query.AddField(new()
			{
				Arguments = new QueryArguments(arguments.To(_ => new QueryArgument(_.Value.GetTypeMember().GraphType(true)) { Name = _.Key })),
				Name = $"Call{schema.ObjectName}",
				Description = $"Calls stored procedure: {schema.Name}.",
				Resolver = new ProcedureFieldResolver(dataSource, procedure, arguments, this._Mediator),
				Type = TypeOf<StoredProcedureResponse>.Member.GraphType(false)
			});
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Query: Count{Table}</term> <description>Counts records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddCountEndpoint<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var objectSchema = this._SqlApi!.GetObjectSchema(dataSource, table);
			var sqlApi = new SqlApi<T>(this._Mediator, dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Count"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Delete{Table}</term> <description>Deletes records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Delete{Table}Data</term> <description>Deletes a batch of records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiDeleteRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddDeleteEndpoints<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var objectSchema = this._SqlApi!.GetObjectSchema(dataSource, table);
			var sqlApi = new SqlApi<T>(this._Mediator, dataSource, objectSchema.Name);
			var sqlApiMethods = TypeOf<SqlApi<T>>.Methods;

			sqlApiMethods["Delete"].Do(method => this.Mutation!.AddField(method.ToFieldType(sqlApi)));
			sqlApiMethods["DeleteData"].Do(method => this.Mutation!.AddField(method.ToFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Insert{Table}Data</term> <description>Inserts a batch of records.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiInsertRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddInsertEndpoint<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var objectSchema = this._SqlApi!.GetObjectSchema(dataSource, table);
			var sqlApi = new SqlApi<T>(this._Mediator, dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["InsertData"].Do(method => this.Mutation!.AddField(method.ToFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Query: Page{Table}</term> <description>Pages records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddPageEndpoint<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var objectSchema = this._SqlApi!.GetObjectSchema(dataSource, table);
			var sqlApi = new SqlApi<T>(this._Mediator, dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Page"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Query: Select{Table}</term> <description>Selects records based on a <c>WHERE</c> clause.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiSelectRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddSelectEndpoint<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var objectSchema = this._SqlApi!.GetObjectSchema(dataSource, table);
			var sqlApi = new SqlApi<T>(this._Mediator, dataSource, objectSchema.Name);

			TypeOf<SqlApi<T>>.Methods["Select"].Do(method => this.Query.AddField(method.ToFieldType(sqlApi)));
		}

		/// <summary>
		/// Creates the following GraphQL endpoints:
		/// <list type="table">
		/// <item><term>Mutation: Update{Table}</term> <description>Updates records based on a <c>WHERE</c> clause.</description></item>
		/// <item><term>Mutation: Update{Table}Data</term> <description>Updates a batch records based on a table's <c>Primary Key</c>.</description></item>
		/// </list>
		/// <i>Requires call to:</i>
		/// <code><see cref="Data.Extensions.IServiceCollectionExtensions.RegisterSqlApiUpdateRules"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddUpdateEndpoints<T>(string dataSource, string table)
			where T : class, new()
		{
			table.AssertNotBlank(nameof(table));
			this._SqlApi.AssertNotNull(nameof(this._SqlApi));

			var objectSchema = this._SqlApi!.GetObjectSchema(dataSource, table);
			var sqlApi = new SqlApi<T>(this._Mediator, dataSource, objectSchema.Name);
			var sqlApiMethods = TypeOf<SqlApi<T>>.Methods;

			sqlApiMethods["Update"].Do(method => this.Mutation!.AddField(method.ToFieldType(sqlApi)));
			sqlApiMethods["UpdateData"].Do(method => this.Mutation!.AddField(method.ToFieldType(sqlApi)));
		}
	}
}
