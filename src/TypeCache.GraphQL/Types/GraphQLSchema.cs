// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
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

namespace TypeCache.GraphQL.Types;

public abstract class GraphQLSchema : Schema
{
	protected readonly IDataLoaderContextAccessor _DataLoader;
	protected readonly IMediator _Mediator;
	protected readonly ISqlApi? _SqlApi;

	public GraphQLSchema(IServiceProvider provider) : base(provider)
	{
		this.Description = this.GetType().Name;

		this._DataLoader = this.GetRequiredService<IDataLoaderContextAccessor>();
		this._Mediator = this.GetRequiredService<IMediator>();
		this._SqlApi = this.GetService<ISqlApi>();
	}

	/// <summary>
	/// Adds a GraphQL endpoint that returns the version of the GraphQL schema.
	/// </summary>
	/// <param name="version">The version of this GraphQL schema</param>
	public FieldType AddVersion(string version)
		=> this.Query.AddField(new()
		{
			Name = "Version",
			DefaultValue = "0",
			Description = $"The version number of this {this.GetType().Name}.",
			Resolver = new FuncFieldResolver<string>(context => version),
			Type = typeof(NonNullGraphType<StringGraphType>)
		});

	/// <summary>
	/// Adds GraphQL endpoints based on class methods decorated with the following attributes:
	/// <list type="bullet">
	/// <item><see cref="GraphQLQueryAttribute"/></item>
	/// <item><see cref="GraphQLMutationAttribute"/></item>
	/// <item><see cref="GraphQLSubqueryAttribute"/></item>
	/// <item><see cref="GraphQLSubqueryBatchAttribute"/></item>
	/// <item><see cref="GraphQLSubqueryCollectionAttribute"/></item>
	/// </list>
	/// </summary>
	/// <typeparam name="T">The class containing the decorated methods that will be converted into GraphQL endpoints.</typeparam>
	/// <returns>The added <see cref="FieldType"/>(s).</returns>
	public FieldType[] AddEndpoints<T>()
	{
		var fieldTypes = new List<FieldType>();

		var methods = TypeOf<T>.Methods.Values.Gather().ToArray();
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLQueryAttribute>()).Map(this.AddQuery));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLMutationAttribute>()).Map(this.AddMutation));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLSubqueryAttribute>()).Map(method =>
		{
			var parentType = method.Attributes.First<GraphQLSubqueryAttribute>()!.ParentType;
			var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
			var resolver = (IFieldResolver)typeof(ItemLoaderFieldResolver<>).MakeGenericType(parentType).GetTypeMember().Create(method, controller, this._DataLoader);
			return this.Query.AddField(method.ToFieldType(resolver));
		}));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLSubqueryBatchAttribute>()).Map(method =>
		{
			var attribute = method.Attributes.First<GraphQLSubqueryCollectionAttribute>()!;
			return this.AddSubqueryBatch(method, attribute.ParentType, attribute.Key);
		}));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLSubqueryCollectionAttribute>()).Map(method =>
		{
			var attribute = method.Attributes.First<GraphQLSubqueryCollectionAttribute>()!;
			return this.AddSubqueryCollection(method, attribute.ParentType, attribute.Key);
		}));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLSubscriptionAttribute>()).Map(this.AddSubscription));

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
		=> TypeOf<T>.Methods[method].Map(this.AddMutation).ToArray();

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
		if (method.Return.Void)
			throw new ArgumentException($"{nameof(AddMutation)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
		return this.Mutation!.AddField(method.ToFieldType(controller));
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
		=> TypeOf<T>.Methods[method].Map(this.AddQuery).ToArray();

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
		if (method.Return.Void)
			throw new ArgumentException($"{nameof(AddQuery)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
		return this.Query.AddField(method.ToFieldType(controller));
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
	public FieldType[] AddSubscription<T>(string method)
		=> TypeOf<T>.Methods[method].Map(this.AddSubscription).ToArray();

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
	public FieldType AddSubscription(MethodMember method)
	{
		if (!method.Return.Type.Is(typeof(IObservable<>)))
			throw new ArgumentException($"{nameof(AddSubscription)}: GraphQL subscription endpoints must have a return type of IObservable<...>.");

		var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
		return this.Subscription!.AddField(method.ToEventStreamFieldType(controller));
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
		var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
		var resolver = new ItemLoaderFieldResolver<T>(method, controller, this._DataLoader);
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
	/// <param name="key">The <see cref="GraphQLKeyAttribute"/> used to match parent and child type properties.</param>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public FieldType AddSubqueryBatch(MethodMember method, Type parentType, string key)
	{
		if (!method.Return.Type.SystemType.IsCollection())
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: [{nameof(method)}] must return a collection instead of [{method.Return.Type.Name}].");

		if (!parentType.GetTypeMember().Properties.Values.TryFirst(property => property.GraphQLKey()?.Is(key) is true, out var parentKeyProperty)
			|| parentKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var childType = method.Return.Type.ElementType ?? method.Return.Type.GenericTypes.First()!;
		if (!childType.Properties.Values.TryFirst(property => property.GraphQLKey().Is(key), out var childKeyProperty)
			|| childKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var getParentKey = parentKeyProperty.Getter.Method;
		var getChildKey = childKeyProperty.Getter.Method;

		var addSubQueryBatchMethod = TypeOf<GraphQLSchema>.Methods[nameof(AddSubqueryBatch)].If(method => method.GenericTypes == 3).First()!;
		return (FieldType)addSubQueryBatchMethod.InvokeGeneric(this, new[] { (Type)parentType, (Type)childType, (Type)childKeyProperty.PropertyType }, method, getParentKey, getChildKey)!;
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
		var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
		var resolver = new BatchLoaderFieldResolver<PARENT, CHILD, KEY>(method, controller, this._DataLoader, getParentKey, getChildKey);
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
	/// <param name="key">The <see cref="GraphQLKeyAttribute"/> used to match parent and child type properties.</param>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public FieldType AddSubqueryCollection(MethodMember method, Type parentType, string key)
	{
		if (!method.Return.Type.SystemType.IsCollection())
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: [{nameof(method)}] must return a collection instead of [{method.Return.Type.Name}].");

		if (!parentType.GetTypeMember().Properties.Values.TryFirst(property => property.GraphQLKey()?.Is(key) is true, out var parentKeyProperty)
			|| parentKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var childType = method.Return.Type.ElementType ?? method.Return.Type.GenericTypes.First()!;
		if (!childType.Properties.Values.TryFirst(property => property.GraphQLKey().Is(key), out var childKeyProperty)
			|| childKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var getParentKey = parentKeyProperty.Getter.Method;
		var getChildKey = childKeyProperty.Getter.Method;

		var addSubqueryCollectionMethod = TypeOf<GraphQLSchema>.Methods[nameof(AddSubqueryCollection)].If(method => method.GenericTypes == 3).First();
		return (FieldType)addSubqueryCollectionMethod!.InvokeGeneric(this, new[] { (Type)parentType, (Type)childType, (Type)childKeyProperty.PropertyType }, method, getParentKey, getChildKey)!;
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
		var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
		var resolver = new CollectionLoaderFieldResolver<PARENT, CHILD, KEY>(method, controller, this._DataLoader, getParentKey, getChildKey);
		return this.Query.AddField(method.ToFieldType(resolver));
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Delete{Table}</term> Deletes records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: Delete{Table}Data</term> Deletes a batch of records based on a table's <c>Primary Key</c>.</item>
	/// <item><term>Mutation: Insert{Table}Data</term> Inserts a batch of records.</item>
	/// <item><term>Query: Page{Table}</term> Pages records based on a <c>WHERE</c> clause and <c>pager</c>.</item>
	/// <item><term>Query: Select{Table}</term> Selects records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: Update{Table}</term> Updates records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: Update{Table}Data</term> Updates a batch of records based on a table's <c>Primary Key</c>.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiEndpoints<T>(string dataSource, string table)
		where T : class, new()
	{
		table.AssertNotBlank();
		this._SqlApi.AssertNotNull();

		var schema = this._SqlApi!.GetObjectSchema(dataSource, table);
		var controller = new SqlApiController<T>(this._Mediator, dataSource, schema.Name);
		var methods = TypeOf<SqlApiController<T>>.Methods;

		if (schema.Type == ObjectType.Table && this.Mutation is not null)
		{
			this.Mutation!.AddField(methods["Delete"][0].ToFieldType(schema.ObjectName, controller));
			this.Mutation.AddField(methods["DeleteData"][0].ToFieldType(schema.ObjectName, controller));
			this.Mutation.AddField(methods["InsertData"][0].ToFieldType(schema.ObjectName, controller));
			this.Mutation.AddField(methods["Update"][0].ToFieldType(schema.ObjectName, controller));
			this.Mutation.AddField(methods["UpdateData"][0].ToFieldType(schema.ObjectName, controller));
		}
		if (this.Query is not null)
		{
			this.Query.AddField(methods["Page"][0].ToFieldType(schema.ObjectName, controller));
			this.Query.AddField(methods["Select"][0].ToFieldType(schema.ObjectName, controller));
		}
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Call{Procedure}</term> Calls the stored procedure and returns its results.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddCallProcedureEndpoint<T>(string dataSource, string procedure)
		where T : class, new()
	{
		procedure.AssertNotBlank();
		this._SqlApi.AssertNotNull();

		var schema = this._SqlApi!.GetObjectSchema(dataSource, procedure);
		var arguments = schema.Parameters.ToDictionary(parameter => parameter.Name, parameter =>
		{
			var type = parameter.Type.ToType();
			return type.IsValueType ? typeof(Nullable<>).MakeGenericType(type).TypeHandle : type.TypeHandle;
		}, StringComparer.OrdinalIgnoreCase);
		this.Query.AddField(new()
		{
			Arguments = new QueryArguments(arguments.Map(_ => new QueryArgument(_.Value.GetTypeMember().GraphQLType(true)) { Name = _.Key })),
			Name = $"Call{schema.ObjectName}",
			Description = $"Calls stored procedure: {schema.Name}.",
			Resolver = new ProcedureFieldResolver(dataSource, procedure, arguments, this._Mediator),
			Type = TypeOf<StoredProcedureResponse>.Member.GraphQLType(false).GraphQLNonNull()
		});
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Delete{Table}</term> Deletes records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: Delete{Table}Data</term> Deletes a batch of records based on a table's <c>Primary Key</c>.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddDeleteEndpoints<T>(string dataSource, string table)
		where T : class, new()
	{
		table.AssertNotBlank();
		this._SqlApi.AssertNotNull();

		var schema = this._SqlApi!.GetObjectSchema(dataSource, table);
		var controller = new SqlApiController<T>(this._Mediator, dataSource, schema.Name);
		var methods = TypeOf<SqlApiController<T>>.Methods;

		methods["Delete"].Do(method => this.Mutation!.AddField(method.ToFieldType(schema.ObjectName, controller)));
		methods["DeleteData"].Do(method => this.Mutation!.AddField(method.ToFieldType(schema.ObjectName, controller)));
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Insert{Table}Data</term> Inserts a batch of records.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddInsertEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		table.AssertNotBlank();
		this._SqlApi.AssertNotNull();

		var schema = this._SqlApi!.GetObjectSchema(dataSource, table);
		var controller = new SqlApiController<T>(this._Mediator, dataSource, schema.Name);

		TypeOf<SqlApiController<T>>.Methods["InsertData"].Do(method => this.Mutation!.AddField(method.ToFieldType(controller)));
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Query: Page{Table}</term> Pages records based on a <c>WHERE</c> clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddPageEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		table.AssertNotBlank();
		this._SqlApi.AssertNotNull();

		var schema = this._SqlApi!.GetObjectSchema(dataSource, table);
		var controller = new SqlApiController<T>(this._Mediator, dataSource, schema.Name);

		TypeOf<SqlApiController<T>>.Methods["Page"].Do(method => this.Query.AddField(method.ToFieldType(schema.ObjectName, controller)));
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Query: Select{Table}</term> Selects records based on a <c>WHERE</c> clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSelectEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		table.AssertNotBlank();
		this._SqlApi.AssertNotNull();

		var schema = this._SqlApi!.GetObjectSchema(dataSource, table);
		var controller = new SqlApiController<T>(this._Mediator, dataSource, schema.Name);

		TypeOf<SqlApiController<T>>.Methods["Select"].Do(method => this.Query.AddField(method.ToFieldType(schema.ObjectName, controller)));
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Update{Table}</term> Updates records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: Update{Table}Data</term> Updates a batch records based on a table's <c>Primary Key</c>.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddUpdateEndpoints<T>(string dataSource, string table)
		where T : class, new()
	{
		table.AssertNotBlank();
		this._SqlApi.AssertNotNull();

		var schema = this._SqlApi!.GetObjectSchema(dataSource, table);
		var controller = new SqlApiController<T>(this._Mediator, dataSource, schema.Name);
		var methods = TypeOf<SqlApiController<T>>.Methods;

		methods["Update"].Do(method => this.Mutation!.AddField(method.ToFieldType(schema.ObjectName, controller)));
		methods["UpdateData"].Do(method => this.Mutation!.AddField(method.ToFieldType(schema.ObjectName, controller)));
	}
}
