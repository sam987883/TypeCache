// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using GraphQLParser;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Attributes;
using TypeCache.Business;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SqlApi;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public abstract class GraphQLSchema : Schema
{
	protected IDataLoaderContextAccessor DataLoader { get; }

	protected IMediator Mediator { get; }

	public GraphQLSchema(IServiceProvider provider) : base(provider)
	{
		this.Description = this.GetType().Name;

		this.DataLoader = this.GetRequiredService<IDataLoaderContextAccessor>();
		this.Mediator = this.GetRequiredService<IMediator>();
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
			Description = Invariant($"The version number of this {this.Description}."),
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

		var methods = TypeOf<T>.Methods.ToArray();
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLQueryAttribute>()).Map(this.AddQuery));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLMutationAttribute>()).Map(this.AddMutation));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLSubqueryAttribute>()).Map(method =>
		{
			var parentType = method.Attributes.First<GraphQLSubqueryAttribute>()!.ParentType;
			var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
			var resolver = (IFieldResolver)typeof(ItemLoaderFieldResolver<>).MakeGenericType(parentType).GetTypeMember().Create(method, controller, this.DataLoader)!;
			return this.Query.AddField(method.ToFieldType(resolver));
		}));
		fieldTypes.AddRange(methods.If(method => method.Attributes.Any<GraphQLSubqueryBatchAttribute>()).Map(method =>
		{
			var attribute = method.Attributes.First<GraphQLSubqueryBatchAttribute>()!;
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
		=> TypeOf<T>.Methods.If(_ => _.Name.Is(method)).Map(this.AddMutation).ToArray();

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
		=> TypeOf<T>.Methods.If(_ => _.Name.Is(method)).Map(this.AddQuery).ToArray();

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
		=> TypeOf<T>.Methods.If(_ => _.Name.Is(method)).Map(this.AddSubscription).ToArray();

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
		var controller = !method.Static ? this.GetRequiredService(method.Type) : null;
		return this.Subscription!.AddField(method.ToFieldType(controller));
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
		var resolver = new ItemLoaderFieldResolver<T>(method, controller, this.DataLoader);
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

		if (!parentType.GetTypeMember().Properties.IfFirst(property => property.GraphQLKey()?.Is(key) is true, out var parentKeyProperty)
			|| parentKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var childType = method.Return.Type.ElementType ?? method.Return.Type.GenericTypes.First()!;
		if (!childType.Properties.IfFirst(property => property.GraphQLKey().Is(key), out var childKeyProperty)
			|| childKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var getParentKey = parentKeyProperty.Getter.Method;
		var getChildKey = childKeyProperty.Getter.Method;

		var addSubQueryBatchMethod = TypeOf<GraphQLSchema>.Methods.If(_ => _.Name.Is(nameof(AddSubqueryBatch)) && method.GenericTypeCount == 3).First()!;
		return (FieldType)addSubQueryBatchMethod.InvokeGeneric(new[] { parentType, (Type)childType, (Type)childKeyProperty.PropertyType }, this, method, getParentKey, getChildKey)!;
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
		var resolver = new BatchLoaderFieldResolver<PARENT, CHILD, KEY>(method, controller, this.DataLoader, getParentKey, getChildKey);
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

		if (!parentType.GetTypeMember().Properties.IfFirst(property => property.GraphQLKey()?.Is(key) is true, out var parentKeyProperty)
			|| parentKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var childType = method.Return.Type.ElementType ?? method.Return.Type.GenericTypes.First()!;
		if (!childType.Properties.IfFirst(property => property.GraphQLKey().Is(key), out var childKeyProperty)
			|| childKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var getParentKey = parentKeyProperty.Getter.Method;
		var getChildKey = childKeyProperty.Getter.Method;

		var addSubqueryCollectionMethod = TypeOf<GraphQLSchema>.Methods.If(_ => _.Name.Is(nameof(AddSubqueryCollection)) && _.GenericTypeCount == 3).First();
		return (FieldType)addSubqueryCollectionMethod!.InvokeGeneric(new[] { (Type)parentType, (Type)childType, (Type)childKeyProperty.PropertyType }, this, method, getParentKey, getChildKey)!;
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
		var resolver = new CollectionLoaderFieldResolver<PARENT, CHILD, KEY>(method, controller, this.DataLoader, getParentKey, getChildKey);
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
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var action = TypeOf<T>.Attributes.First<SqlApiAttribute>()?.Actions ?? SqlApiAction.All;
		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).Result;

		if (this.Mutation is not null && (action is SqlApiAction.All || schema.Type is ObjectType.Table))
		{
			if (action.HasFlag(SqlApiAction.DeleteData))
				this.AddSqlApiDeleteDataEndpoint<T>(dataSource, table);

			if (action.HasFlag(SqlApiAction.Delete))
				this.AddSqlApiDeleteEndpoint<T>(dataSource, table);

			if (action.HasFlag(SqlApiAction.InsertData))
				this.AddSqlApiInsertDataEndpoint<T>(dataSource, table);

			if (action.HasFlag(SqlApiAction.Insert))
				this.AddSqlApiInsertEndpoint<T>(dataSource, table);

			if (action.HasFlag(SqlApiAction.UpdateData))
				this.AddSqlApiUpdateDataEndpoint<T>(dataSource, table);

			if (action.HasFlag(SqlApiAction.Update))
				this.AddSqlApiUpdateEndpoint<T>(dataSource, table);
		}

		if (this.Query is not null)
		{
			if (action.HasFlag(SqlApiAction.Page))
				this.AddSqlApiPageEndpoint<T>(dataSource, table);

			if (action.HasFlag(SqlApiAction.Select))
				this.AddSqlApiSelectEndpoint<T>(dataSource, table);
		}
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: call{Procedure}</term> Calls the stored procedure and returns its results.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiCallProcedureEndpoint<T>(string dataSource, string procedure, Func<DbDataReader, CancellationToken, ValueTask<object>> readData)
	{
		dataSource.AssertNotBlank();
		procedure.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, procedure)).Result;
		var arguments = schema.Parameters.ToDictionary(parameter => parameter.Name, parameter =>
		{
			var type = parameter.Type.ToType();
			return type.IsValueType ? typeof(Nullable<>).MakeGenericType(type).TypeHandle : type.TypeHandle;
		}, StringComparer.OrdinalIgnoreCase);
		this.Query.AddField(new()
		{
			Arguments = new QueryArguments(arguments.Map(_ => new QueryArgument(_.Value.GetTypeMember().GraphQLType(true)) { Name = _.Key })),
			Name = Invariant($"Call{schema.ObjectName}"),
			Description = Invariant($"Calls stored procedure: {schema.Name}."),
			Resolver = new StoredProcedureFieldResolver(schema, readData, this.Mediator),
			Type = TypeOf<T>.Member.GraphQLType(false).ToNonNullGraphType()
		});
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Delete{Table}Data</term> Deletes records passed in based on primary key value(s).</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiDeleteDataEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).Result;
		var fieldType = new FieldType
		{
			Arguments = new(new QueryArgument(typeof(ListGraphType<GraphQLInputType<T>>)) { Name = "Data", Description = "The data to be deleted." }),
			Name = string.Format("delete{0}Data", schema.ObjectName),
			Description = string.Format("DELETE ... OUTPUT ... FROM {0} ... VALUES ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiDeleteData<T>),
			Type = typeof(GraphQLObjectType<DeleteResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Mutation!.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Delete{Table}</term> Deletes records based on a <c>WHERE</c> clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiDeleteEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).Result;
		var fieldType = new FieldType
		{
			Arguments = new(
				new QueryArgument<ListGraphType<GraphQLInputType<Parameter>>> { Name = "Parameters", DefaultValue = Array<Parameter>.Empty },
				new QueryArgument<StringGraphType> { Name = nameof(DeleteCommand.Where), DefaultValue = string.Empty, Description = "If `where` is omitted, all records will be deleted!" }
			),
			Name = string.Format("delete{0}", schema.ObjectName),
			Description = string.Format("DELETE ... OUTPUT ... FROM {0} WHERE ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiDelete<T>),
			Type = typeof(GraphQLObjectType<DeleteResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Mutation!.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: insert{Table}Data</term> Inserts a batch of records.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiInsertDataEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).GetAwaiter().GetResult();
		var fieldType = new FieldType
		{
			Arguments = new(
				new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>
				{
					Name = nameof(InsertDataCommand<T>.Columns),
					Description = "The columns to be inserted into."
				},
				new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<GraphQLInputType<T>>>>>
				{
					Name = "Data",
					Description = "The data to be inserted."
				}
			),
			Name = string.Format("insert{0}Data", schema.ObjectName),
			Description = string.Format("INSERT INTO {0} ... VALUES ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiInsertData<T>),
			Type = typeof(GraphQLObjectType<InsertResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Mutation!.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: insert{Table}Data</term> Inserts a batch of records.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiInsertEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).GetAwaiter().GetResult();
		var fieldType = new FieldType
		{
			Arguments = new(
				new QueryArgument<BooleanGraphType> { Name = nameof(InsertCommand.Distinct), DefaultValue = false },
				new QueryArgument<ListGraphType<GraphQLOrderByType<T>>> { Name = nameof(InsertCommand.OrderBy), DefaultValue = Array<OrderBy<T>>.Empty },
				new QueryArgument<ListGraphType<GraphQLInputType<Parameter>>> { Name = "Parameters", DefaultValue = Array<Parameter>.Empty },
				new QueryArgument<BooleanGraphType> { Name = nameof(InsertCommand.Percent), DefaultValue = false },
				new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>> { Name = "SourceColumns" },
				new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "SourceTable" },
				new QueryArgument<StringGraphType> { Name = nameof(UpdateCommand.TableHints), DefaultValue = string.Empty },
				new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>> { Name = "TargetColumns" },
				new QueryArgument<UIntGraphType> { Name = nameof(InsertCommand.Top), DefaultValue = 0U },
				new QueryArgument<StringGraphType> { Name = nameof(InsertCommand.Where), DefaultValue = string.Empty },
				new QueryArgument<BooleanGraphType> { Name = nameof(InsertCommand.WithTies), DefaultValue = false }
			),
			Name = string.Format("insert{0}", schema.ObjectName),
			Description = string.Format("INSERT INTO {0} SELECT ... FROM ... WHERE ... ORDER BY ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiInsert<T>),
			Type = typeof(GraphQLObjectType<SelectResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Mutation!.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Query: page{Table}</term> Selects records in pages (batches) based on a <c>WHERE</c> clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiPageEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).GetAwaiter().GetResult();
		schema.AssertNotNull();

		var fieldType = new FieldType
		{
			Arguments = new(
				new QueryArgument<UIntGraphType> { Name = nameof(Pager.After), DefaultValue = 0U },
				new QueryArgument<BooleanGraphType> { Name = nameof(SelectCommand.Distinct), DefaultValue = false },
				new QueryArgument<UIntGraphType> { Name = nameof(Pager.First), DefaultValue = 0U },
				new QueryArgument<ListGraphType<GraphQLOrderByType<T>>> { Name = nameof(SelectCommand.OrderBy), DefaultValue = Array<OrderBy<T>>.Empty },
				new QueryArgument<ListGraphType<GraphQLInputType<Parameter>>> { Name = "Parameters", DefaultValue = Array<Parameter>.Empty },
				new QueryArgument<StringGraphType> { Name = nameof(SelectCommand.TableHints), DefaultValue = string.Empty },
				new QueryArgument<StringGraphType> { Name = nameof(SelectCommand.Where), DefaultValue = string.Empty }
			),
			Name = string.Format("page{0}", schema.ObjectName),
			Description = string.Format("SELECT ... FROM {0} WHERE ... ORDER BY ... OFFSET ... FETCH ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiPage<T>),
			Type = typeof(GraphQLObjectType<PageResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Query!.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Query: select{Table}</term> Selects records based on a <c>WHERE</c> clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiSelectEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).GetAwaiter().GetResult();
		schema.AssertNotNull();

		var fieldType = new FieldType
		{
			Arguments = new(
				new QueryArgument<BooleanGraphType> { Name = nameof(SelectCommand.Distinct), DefaultValue = false },
				new QueryArgument<ListGraphType<GraphQLOrderByType<T>>> { Name = nameof(SelectCommand.OrderBy), DefaultValue = Array<OrderBy<T>>.Empty },
				new QueryArgument<ListGraphType<GraphQLInputType<Parameter>>> { Name = "Parameters", DefaultValue = Array<Parameter>.Empty },
				new QueryArgument<BooleanGraphType> { Name = nameof(SelectCommand.Percent), DefaultValue = false },
				new QueryArgument<StringGraphType> { Name = nameof(SelectCommand.TableHints), DefaultValue = string.Empty },
				new QueryArgument<UIntGraphType> { Name = nameof(SelectCommand.Top), DefaultValue = 0U },
				new QueryArgument<StringGraphType> { Name = nameof(SelectCommand.Where), DefaultValue = string.Empty, Description = "If `where` is omitted, all records will be returned." },
				new QueryArgument<BooleanGraphType> { Name = nameof(SelectCommand.WithTies), DefaultValue = false }
			),
			Name = string.Format("select{0}", schema.ObjectName),
			Description = string.Format("SELECT ... FROM {0} WHERE ... ORDER BY ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiSelect<T>),
			Type = typeof(GraphQLObjectType<SelectResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Query!.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: update{Table}Data</term> Updates a batch of records.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiUpdateDataEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).GetAwaiter().GetResult();
		var fieldType = new FieldType
		{
			Arguments = new(
				new QueryArgument(typeof(NonNullGraphType<ListGraphType<NonNullGraphType<GraphQLInputType<T>>>>)) { Name = "Set", Description = "The data to be updated." },
				new QueryArgument<StringGraphType> { Name = nameof(UpdateCommand.TableHints), DefaultValue = string.Empty }
			),
			Name = string.Format("update{0}Data", schema.ObjectName),
			Description = string.Format("UPDATE {0} SET ... OUTPUT ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiUpdateData<T>),
			Type = typeof(GraphQLObjectType<UpdateResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Mutation!.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: update{Table}</term> Updates records based on a WHERE clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void AddSqlApiUpdateEndpoint<T>(string dataSource, string table)
		where T : class, new()
	{
		dataSource.AssertNotBlank();
		table.AssertNotBlank();

		var schema = this.Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(dataSource, table)).GetAwaiter().GetResult();
		var fieldType = new FieldType
		{
			Arguments = new(
				new QueryArgument<ListGraphType<GraphQLInputType<Parameter>>> { Name = "Parameters", DefaultValue = Array<Parameter>.Empty },
				new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>> { Name = nameof(UpdateCommand.Set), DefaultValue = Array<string>.Empty, Description = "[Column] = {Value}" },
				new QueryArgument<StringGraphType> { Name = nameof(UpdateCommand.TableHints), DefaultValue = string.Empty },
				new QueryArgument<StringGraphType> { Name = nameof(InsertCommand.Where), DefaultValue = string.Empty, Description = "If `where` is omitted, all records will be updated." }
			),
			Name = string.Format("update{0}", schema.ObjectName),
			Description = string.Format("UPDATE {0} SET ... OUTPUT ... WHERE ...", schema.Name),
			Resolver = CreateFieldResolver(ResolveSqlApiUpdate<T>),
			Type = typeof(GraphQLObjectType<SelectResponse<T>>)
		};
		fieldType.Metadata[nameof(SqlApiMetadata)] = new SqlApiMetadata
		{
			Columns = TypeOf<T>.Properties.Map(_ => _.Name).ToArray(),
			DataSource = dataSource,
			Mediator = this.Mediator,
			Table = schema.Name
		};

		this.Mutation!.AddField(fieldType);
	}

	private static FuncFieldResolver<T?> CreateFieldResolver<T>(Func<IResolveFieldContext, Task<T>> resolve)
		=> new FuncFieldResolver<T?>(async context =>
		{
			try
			{
				return await resolve(context);
			}
			catch (Exception error)
			{
				HandleError(context, error);
				return default;
			}
		});

	private static async Task<DeleteResponse<T>?> ResolveSqlApiDelete<T>(IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));

		var command = new DeleteCommand
		{
			DataSource = metadata.DataSource,
			Output = selections
				.If(column => selections.AnyLeft(Invariant($"{nameof(DeleteResponse<T>.Deleted)}.{column}")))
				.Each(column => Invariant($"DELETED.[{column}]"))
				.ToArray(),
			Table = metadata.Table,
			Where = context.GetArgument<string>(nameof(InsertCommand.Where))
		};
		context.GetArgument<Parameter[]>("Parameters")?.Do(parameter => command.InputParameters[parameter.Name] = parameter.Value);

		var rowSet = await metadata.Mediator.ApplyRuleAsync<DeleteCommand, RowSetResponse<T>>(command, context.CancellationToken);

		var sql = string.Empty;
		if (selections.Has(nameof(DeleteResponse<T>.Sql)))
			sql = await metadata.Mediator.ApplyRuleAsync<DeleteCommand, string>(command, context.CancellationToken);

		return new()
		{
			Count = rowSet.Count,
			DataSource = metadata.DataSource,
			Deleted = rowSet.Rows,
			Sql = sql,
			Table = metadata.Table
		};
	}

	private static async Task<DeleteResponse<T>?> ResolveSqlApiDeleteData<T>(IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));

		var command = new DeleteDataCommand<T>
		{
			DataSource = metadata.DataSource,
			Input = context.GetArgument<T[]>("Data"),
			Output = selections
				.If(column => selections.AnyLeft(Invariant($"{nameof(DeleteResponse<T>.Deleted)}.{column}")))
				.Each(column => Invariant($"DELETED.[{column}]"))
				.ToArray(),
			Table = metadata.Table
		};

		var rowSet = await metadata.Mediator.ApplyRuleAsync<DeleteDataCommand<T>, RowSetResponse<T>>(command, context.CancellationToken);

		var sql = string.Empty;
		if (selections.Has(nameof(DeleteResponse<T>.Sql)))
			sql = await metadata.Mediator.ApplyRuleAsync<DeleteDataCommand<T>, string>(command, context.CancellationToken);

		return new()
		{
			Count = rowSet.Count,
			DataSource = metadata.DataSource,
			Deleted = rowSet.Rows,
			Sql = sql,
			Table = metadata.Table
		};
	}

	private static async Task<InsertResponse<T>?> ResolveSqlApiInsert<T>(IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));
		var sourceColumns = context.GetArgument<string[]>("SourceColumns");

		var command = new InsertCommand
		{
			Columns = context.GetArgument<string[]>("TargetColumns"),
			DataSource = metadata.DataSource,
			Distinct = context.GetArgument<bool>(nameof(InsertCommand.Distinct)),
			From = context.GetArgument<string>("SourceTable"),
			OrderBy = context.GetArgument<OrderBy<T>[]>(nameof(InsertCommand.OrderBy))
				.ToArray(_ => Invariant($"{_.Expression} {_.Sort.ToSQL()}")),
			Output = selections
				.If(column => selections.AnyLeft(Invariant($"{nameof(InsertResponse<T>.Inserted)}.{column}")))
				.Each(column => Invariant($"INSERTED.[{column}]"))
				.ToArray(),
			Select = metadata.Columns
				.If(column => sourceColumns.AnyRight(Invariant($"SourceColumns.{column}")))
				.ToArray(),
			Table = metadata.Table,
			TableHints = context.GetArgument<string>(nameof(InsertCommand.TableHints)),
			Top = context.GetArgument<uint>(nameof(InsertCommand.Top)),
			Percent = context.GetArgument<bool>(nameof(InsertCommand.Percent)),
			Where = context.GetArgument<string>(nameof(InsertCommand.Where)),
			WithTies = context.GetArgument<bool>(nameof(InsertCommand.WithTies))
		};
		context.GetArgument<Parameter[]>("Parameters")?.Do(parameter => command.InputParameters[parameter.Name] = parameter.Value);

		var rowSet = await metadata.Mediator.ApplyRuleAsync<InsertCommand, RowSetResponse<T>>(command, context.CancellationToken);

		var sql = string.Empty;
		if (selections.Has(nameof(InsertResponse<T>.Sql)))
			sql = await metadata.Mediator.ApplyRuleAsync<InsertCommand, string>(command, context.CancellationToken);

		return new()
		{
			Count = rowSet.Count,
			DataSource = metadata.DataSource,
			Inserted = rowSet.Rows,
			Sql = sql,
			Table = metadata.Table
		};
	}

	private static async Task<InsertResponse<T>?> ResolveSqlApiInsertData<T>(IResolveFieldContext context)
	{
		var inputs = context.GetInputs().Keys.ToArray();
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));

		var command = new InsertDataCommand<T>
		{
			Columns = context.GetArgument<string[]>(nameof(InsertDataCommand<T>.Columns)),
			DataSource = metadata.DataSource,
			Input = context.GetArgument<T[]>("Data"),
			Output = metadata.Columns
				.If(column => selections.AnyRight(Invariant($"{nameof(InsertResponse<T>.Inserted)}.{column}")))
				.Each(column => Invariant($"INSERTED.[{column}]"))
				.ToArray(),
			Table = metadata.Table
		};

		var sql = string.Empty;
		if (selections.Has(nameof(SelectResponse<T>.Sql)))
			sql = await metadata.Mediator.ApplyRuleAsync<InsertDataCommand<T>, string>(command, context.CancellationToken);

		var rowSet = await metadata.Mediator.ApplyRuleAsync<InsertDataCommand<T>, RowSetResponse<T>>(command, context.CancellationToken);

		return new()
		{
			Count = rowSet.Count,
			DataSource = metadata.DataSource,
			Inserted = rowSet.Rows,
			Sql = sql,
			Table = metadata.Table
		};
	}

	private static async Task<PageResponse<T>?> ResolveSqlApiPage<T>(IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));

		var command = new SelectCommand
		{
			DataSource = metadata.DataSource,
			Distinct = context.GetArgument<bool>(nameof(SelectCommand.Distinct)),
			From = metadata.Table,
			OrderBy = context.GetArgument<OrderBy<T>[]>(nameof(SelectCommand.OrderBy))
				.ToArray(_ => Invariant($"{_.Expression} {_.Sort.ToSQL()}")),
			Select = metadata.Columns
				.If(column => selections.AnyRight(Invariant($"{nameof(PageResponse<T>.Select)}.{column}")))
				.ToArray(),
			Pager = new(context.GetArgument<uint>(nameof(Pager.First)), context.GetArgument<uint>(nameof(Pager.After))),
			TableHints = context.GetArgument<string>(nameof(SelectCommand.TableHints)),
			Where = context.GetArgument<string>(nameof(SelectCommand.Where)),
		};
		context.GetArgument<Parameter[]>("Parameters")?.Do(parameter => command.InputParameters[parameter.Name] = parameter.Value);

		Connection<T>? data = null;
		var sql = string.Empty;
		if (selections.AnyLeft(Invariant($"{nameof(PageResponse<T>.Select)}.{nameof(Connection<T>.Items)}."))
			|| selections.AnyLeft(Invariant($"{nameof(PageResponse<T>.Select)}.{nameof(Connection<T>.Edges)}.")))
		{
			await metadata.Mediator.ApplyRuleAsync<SelectCommand, RowSetResponse<T>>(command
				, output => data = output.Rows.ToConnection((int)output.Count, command.Pager.Value)
				, error => HandleError(context, error)
				, context.CancellationToken);

			if (selections.Has(nameof(SelectResponse<T>.Sql)))
				await metadata.Mediator.ApplyRuleAsync<SelectCommand, string>(command
					, result => sql = result
					, error => HandleError(context, error)
					, context.CancellationToken);
		}
		else if (selections.Has(Invariant($"{nameof(PageResponse<T>.Select)}.{nameof(Connection<T>.TotalCount)}")))
		{
			var countCommand = new CountCommand
			{
				DataSource = metadata.DataSource,
				Table = metadata.Table,
				Where = command.Where
			};
			await metadata.Mediator.ApplyRuleAsync<CountCommand, long>(countCommand
				, count => data = new() { TotalCount = (int?)count }
				, error => HandleError(context, error)
				, context.CancellationToken);

			if (selections.Has(nameof(PageResponse<T>.Sql)))
				await metadata.Mediator.ApplyRuleAsync<CountCommand, string>(countCommand
					, result => sql = result
					, error => HandleError(context, error)
					, context.CancellationToken);
		}

		return new()
		{
			Select = data,
			DataSource = metadata.DataSource,
			Sql = sql,
			Table = metadata.Table
		};
	}

	private static async Task<SelectResponse<T>?> ResolveSqlApiSelect<T>(IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));

		var command = new SelectCommand
		{
			DataSource = metadata.DataSource,
			Distinct = context.GetArgument<bool>(nameof(SelectCommand.Distinct)),
			From = metadata.Table,
			OrderBy = context.GetArgument<OrderBy<T>[]>(nameof(SelectCommand.OrderBy))
				.ToArray(_ => Invariant($"{_.Expression} {_.Sort.ToSQL()}")),
			Select = metadata.Columns
				.If(column => selections.AnyRight(Invariant($"{nameof(SelectResponse<T>.Select)}.{column}")))
				.ToArray(),
			TableHints = context.GetArgument<string>(nameof(SelectCommand.TableHints)),
			Top = context.GetArgument<uint>(nameof(SelectCommand.Top)),
			Percent = context.GetArgument<bool>(nameof(SelectCommand.Percent)),
			Where = context.GetArgument<string>(nameof(SelectCommand.Where)),
			WithTies = context.GetArgument<bool>(nameof(SelectCommand.WithTies))
		};
		context.GetArgument<Parameter[]>("Parameters")?.Do(parameter => command.InputParameters[parameter.Name] = parameter.Value);

		var response = new SelectResponse<T>
		{
			DataSource = metadata.DataSource,
			Table = metadata.Table
		};

		if (selections.AnyLeft(Invariant($"{nameof(SelectResponse<T>.Select)}.")))
		{
			await metadata.Mediator.ApplyRuleAsync<SelectCommand, RowSetResponse<T>>(command, output =>
			{
				response.Select = output.Rows;
				response.Count = output.Count;
			}, context.CancellationToken);

			if (selections.Has(nameof(SelectResponse<T>.Sql)))
				await metadata.Mediator.ApplyRuleAsync<SelectCommand, string>(command
					, sql => response.Sql = sql
					, error => HandleError(context, error)
					, context.CancellationToken);
		}
		else if (selections.Has(nameof(SelectResponse<T>.Count)))
		{
			var countCommand = new CountCommand
			{
				DataSource = metadata.DataSource,
				Table = metadata.Table,
				Where = command.Where
			};
			await metadata.Mediator.ApplyRuleAsync<CountCommand, long>(countCommand
				, count => response.Count = count
				, error => HandleError(context, error)
				, context.CancellationToken);

			if (selections.Has(nameof(SelectResponse<T>.Sql)))
				await metadata.Mediator.ApplyRuleAsync<CountCommand, string>(countCommand
					, sql => response.Sql = sql
					, error => HandleError(context, error)
					, context.CancellationToken);
		}

		return response;
	}

	private static async Task<UpdateResponse<T>?> ResolveSqlApiUpdate<T>(IResolveFieldContext context)
	{
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));

		var command = new UpdateCommand
		{
			Set = context.GetArgument<string[]>(nameof(UpdateCommand.Set)),
			DataSource = metadata.DataSource,
			Output = selections
				.If(column => selections.AnyLeft(Invariant($"{nameof(UpdateResponse<T>.Deleted)}.{column}")))
				.Each(column => Invariant($"DELETED.[{column}]"))
				.Union(selections
					.If(column => selections.AnyLeft(Invariant($"{nameof(UpdateResponse<T>.Inserted)}.{column}")))
					.Each(column => Invariant($"INSERTED.[{column}]"))
				).ToArray(),
			Table = metadata.Table,
			TableHints = context.GetArgument<string>(nameof(UpdateCommand.TableHints)),
			Where = context.GetArgument<string>(nameof(UpdateCommand.Where)),
		};
		context.GetArgument<Parameter[]>("Parameters")?.Do(parameter => command.InputParameters[parameter.Name] = parameter.Value);

		var rowSet = await metadata.Mediator.ApplyRuleAsync<UpdateCommand, UpdateRowSetResponse<T>>(command, context.CancellationToken);

		var sql = string.Empty;
		if (selections.Has(nameof(InsertResponse<T>.Sql)))
			sql = await metadata.Mediator.ApplyRuleAsync<UpdateCommand, string>(command, context.CancellationToken);

		return new()
		{
			Count = rowSet.Count,
			DataSource = metadata.DataSource,
			Deleted = rowSet.Deleted,
			Inserted = rowSet.Inserted,
			Sql = sql,
			Table = metadata.Table
		};
	}

	private static async Task<UpdateResponse<T>?> ResolveSqlApiUpdateData<T>(IResolveFieldContext context)
	{
		var inputs = context.GetInputs().Keys.ToArray();
		var selections = context.GetSelections().ToArray();
		var metadata = context.FieldDefinition.GetMetadata<SqlApiMetadata>(nameof(SqlApiMetadata));

		var command = new UpdateDataCommand<T>
		{
			Columns = metadata.Columns
				.If(column => inputs.AnyRight(Invariant($"Data.{column}")))
				.ToArray(),
			DataSource = metadata.DataSource,
			Input = context.GetArgument<T[]>("Data"),
			Output = selections
				.If(column => selections.AnyLeft(Invariant($"{nameof(UpdateResponse<T>.Deleted)}.{column}")))
				.Each(column => Invariant($"DELETED.[{column}]"))
				.Union(selections
					.If(column => selections.AnyLeft(Invariant($"{nameof(UpdateResponse<T>.Inserted)}.{column}")))
					.Each(column => Invariant($"INSERTED.[{column}]"))
				).ToArray(),
			Table = metadata.Table
		};

		var sql = string.Empty;
		if (selections.Has(nameof(SelectResponse<T>.Sql)))
			sql = await metadata.Mediator.ApplyRuleAsync<UpdateDataCommand<T>, string>(command, context.CancellationToken);

		var rowSet = await metadata.Mediator.ApplyRuleAsync<UpdateDataCommand<T>, UpdateRowSetResponse<T>>(command, context.CancellationToken);

		return new()
		{
			Count = rowSet.Count,
			DataSource = metadata.DataSource,
			Deleted = rowSet.Deleted,
			Inserted = rowSet.Inserted,
			Sql = sql,
			Table = metadata.Table
		};
	}

	private static void HandleError(IResolveFieldContext context, Exception error)
	{
		if (error is ValidationException exception)
			exception.ValidationMessages.Do(message => context.Errors.Add(new ExecutionError(message)));
		else
			context.Errors.Add(new ExecutionError(error.Message, error));
	}
}
