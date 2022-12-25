﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Attributes;
using TypeCache.Collections;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SqlApi;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Extensions;

public static class SchemaExtensions
{
	/// <summary>
	/// Adds a GraphQL endpoint that returns the version of the GraphQL schema.
	/// </summary>
	/// <param name="version">The version of this GraphQL schema</param>
	public static FieldType AddVersion(this ISchema @this, string version)
	{
		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		return @this.Query.AddField(new()
		{
			Name = "Version",
			DefaultValue = "0",
			Description = Invariant($"The version of this GraphQL Schema: {version}."),
			Resolver = new FuncFieldResolver<string>(context => version),
			Type = typeof(NonNullGraphType<StringGraphType>)
		});
	}

	/// <summary>
	/// Adds all of the queries for all of the database schema data made available by the data provider.
	/// </summary>
	/// <param name="dataSourceName">The name of the <see cref="DataSource"/></param>
	/// <exception cref="ArgumentNullException"/>
	public static FieldType[] AddDatabaseSchemaQueries(this ISchema @this, IDataSource dataSource)
	{
		var table = dataSource.GetDatabaseSchema(SchemaCollection.MetaDataCollections);
		var rows = table.Rows.OfType<DataRow>();

		return rows.Select(row => @this.AddDatabaseSchemaQuery(dataSource, row[SchemaColumn.collectionName].ToString().ToEnum<SchemaCollection>()!.Value)).ToArray();
	}

	/// <summary>
	/// Adds a query for the specified collection of database schema data made available by the data provider.
	/// </summary>
	/// <param name="dataSourceName">The name of the <see cref="DataSource"/></param>
	/// <exception cref="ArgumentNullException"/>
	public static FieldType AddDatabaseSchemaQuery(this ISchema @this, IDataSource dataSource, SchemaCollection collection)
	{
		dataSource.AssertNotNull();

		var table = dataSource.GetDatabaseSchema(collection);
		var graphDatabasesEnum = new EnumerationGraphType
		{
			Name = Invariant($"{dataSource.Name.ToPascalCase()}Database"),
			Description = Invariant($"`{dataSource}` databases."),
		};
		foreach (var database in dataSource.Databases)
			graphDatabasesEnum.Add(new(database, database));

		var graphOrderByEnum = new EnumerationGraphType
		{
			Name = Invariant($"{table.TableName.ToPascalCase()}Column"),
			Description = Invariant($"`{table.TableName}` columns.")
		};
		var resolvedType = new ObjectGraphType
		{
			Name = table.TableName,
			Description = Invariant($"Database Schema Collection: `{table.TableName}`")
		};
		foreach (var column in table.Columns.OfType<DataColumn>())
		{
			graphOrderByEnum.AddOrderBy(new(column.ColumnName, Sort.Ascending));
			graphOrderByEnum.AddOrderBy(new(column.ColumnName, Sort.Descending));

			var field = resolvedType.AddField(new()
			{
				Name = FixName(column!.ColumnName),
				Description = Invariant($"`{column.ColumnName}`"),
				Resolver = new FuncFieldResolver<DataRow, object?>(static context =>
				{
					var columnName = context.FieldDefinition.Metadata[nameof(DataColumn.ColumnName)]!.ToString()!;
					var columnType = ((RuntimeTypeHandle)context.FieldDefinition.Metadata[nameof(DataColumn.DataType)]!).ToType();
					var fieldType = context.FieldDefinition.Type;
					var value = context.Source[columnName];
					return value switch
					{
						DBNull or null => null,
						byte[] bytes when fieldType == typeof(StringGraphType) => bytes.ToBase64(),
						_ when fieldType == typeof(StringGraphType) => value.ToString(),
						_ => value
					};
				}),
				Type = column.DataType switch
				{
					_ when column.DataType == typeof(object) => typeof(StringGraphType),
					_ => column.DataType.GetTypeMember().NullableGraphQLType()
				}
			});
			field.Metadata.Add(nameof(DataColumn.ColumnName), column.ColumnName);
			field.Metadata.Add(nameof(DataColumn.DataType), column.DataType.TypeHandle);
		}

		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		var fieldType = @this.Query.AddField(new()
		{
			Name = table.TableName,
			Description = Invariant($"Database Schema: {table.TableName}"),
			Arguments = new QueryArguments(
				new QueryArgument(graphDatabasesEnum) { Name = "database" },
				new QueryArgument<StringGraphType> { Name = "where" },
				new QueryArgument(new ListGraphType(new NonNullGraphType(graphOrderByEnum))) { Name = "orderBy" }
			),
			ResolvedType = new ListGraphType(resolvedType),
			Resolver = new DatabaseSchemaFieldResolver()
		});
		fieldType.Metadata.Add(nameof(IDataSource), dataSource);
		fieldType.Metadata.Add(nameof(SchemaCollection), table.TableName.ToEnum<SchemaCollection>());

		return fieldType;
	}

	/// <exception cref="ArgumentNullException"/>
	public static void AddDatabaseEndpoints(this ISchema @this, IDataSource dataSource, SqlApiAction actions, string? database = null, string? schema = null)
	{
		const string ColumnName = nameof(ColumnName);
		const string ColumnType = nameof(ColumnType);

		dataSource.AssertNotNull();

		database ??= dataSource.DefaultDatabase;
		var objectSchemas = dataSource.ObjectSchemas.Values.ToArray();
		if (schema.IsNotBlank())
			objectSchemas = objectSchemas.Where(_ => _.DatabaseName.Is(database) && _.SchemaName.Is(schema)).ToArray();
		else if (database.IsNotBlank())
			objectSchemas = objectSchemas.Where(_ => _.DatabaseName.Is(database)).ToArray();

		objectSchemas.ForEach(objectSchema =>
		{
			var table = objectSchema.ObjectName.ToPascalCase();
			var dataInputType = new InputObjectGraphType
			{
				Name = Invariant($"{objectSchema.ObjectName}Input"),
				Description = Invariant($"{objectSchema.Type.Name()}: `{objectSchema.Name}`")
			};
			var graphOrderByEnum = new EnumerationGraphType
			{
				Name = Invariant($"{table}OrderBy")
			};
			var resolvedType = new ObjectGraphType
			{
				Name = objectSchema.ObjectName,
				Description = Invariant($"{objectSchema.Type.Name()}: `{objectSchema.Name}`")
			};
			foreach (var column in objectSchema.Columns)
			{
				var columnDataType = column.DataTypeHandle.ToType();
				dataInputType.AddField(new()
				{
					Name = column.Name,
					Description = Invariant($"`{column.Name}`"),
					Type = columnDataType switch
					{
						_ when columnDataType == typeof(object) => typeof(StringGraphType),
						_ => columnDataType.GetTypeMember().NullableGraphQLType()
					}
				});

				var field = resolvedType.AddField(new()
				{
					Name = FixName(column.Name),
					Description = Invariant($"{(column.PrimaryKey ? "Primary Key: " : string.Empty)}`{column.Name}`"),
					Resolver = new FuncFieldResolver<DataRow, object?>(static context =>
					{
						var columnName = context.FieldDefinition.Metadata[ColumnName]!.ToString()!;
						var columnType = ((RuntimeTypeHandle)context.FieldDefinition.Metadata[ColumnType]!).ToType();
						var fieldType = context.FieldDefinition.Type;
						var value = context.Source[columnName];
						return value switch
						{
							DBNull => null,
							byte[] bytes when fieldType == typeof(StringGraphType) => bytes.ToBase64(),
							_ when fieldType == typeof(StringGraphType) => value.ToString(),
							_ => value
						};
					}),
					Type = column.DataTypeHandle switch
					{
						_ when columnDataType == typeof(object) => typeof(StringGraphType),
						_ => columnDataType.GetTypeMember().NullableGraphQLType()
					}
				});
				field.Metadata.Add(ColumnName, column.Name);
				field.Metadata.Add(ColumnType, column.DataTypeHandle);

				graphOrderByEnum.AddOrderBy(new(column.Name, Sort.Ascending));
				graphOrderByEnum.AddOrderBy(new(column.Name, Sort.Descending));
			}

			if (objectSchema.Type.IsAny(DatabaseObjectType.Table, DatabaseObjectType.View) && actions.HasFlag(SqlApiAction.Select))
			{
				var selectResponseType = SelectResponse<DataRow>.CreateGraphType(table, $"{objectSchema.Type.Name()}: `{objectSchema.Name}`", resolvedType);
				var arguments = new QueryArguments();
				arguments.Add<ListGraphType<NonNullGraphType<GraphQLInputType<Parameter>>>>("parameters", null, "Used to reference user input values from the where clause.");
				if (dataSource.Type is DataSourceType.SqlServer)
					arguments.Add<StringGraphType>(nameof(SelectQuery.Top));

				arguments.Add<BooleanGraphType>(nameof(SelectQuery.Distinct), false);
				arguments.Add<StringGraphType>(nameof(SelectQuery.Where), null, "If `where` is omitted, all records will be returned.");
				arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)), Array<OrderBy>.Empty);
				arguments.Add<UIntGraphType>(nameof(SelectQuery.Fetch), 0U);
				arguments.Add<UIntGraphType>(nameof(SelectQuery.Offset), 0U);
				arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null);

				var field = new FieldType
				{
					Arguments = arguments,
					Name = Invariant($"select{table}"),
					Description = Invariant($"SELECT ... FROM {objectSchema.Name} WHERE ... ORDER BY ..."),
					Resolver = new SqlApiSelectFieldResolver(),
					ResolvedType = selectResponseType
				};
				field.Metadata[nameof(ObjectSchema)] = objectSchema;

				@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
				@this.Query.AddField(field);
			}

			if (objectSchema.Type is DatabaseObjectType.Table)
			{
				var outputResponseType = OutputResponse<DataRow>.CreateGraphType(table, $"{objectSchema.Type.Name()}: `{objectSchema.Name}`", resolvedType);

				if (actions.HasFlag(SqlApiAction.Delete))
				{
					var fieldType = new FieldType
					{
						Arguments = new(new QueryArgument(new NonNullGraphType(new ListGraphType(new NonNullGraphType(dataInputType))))
						{
							Name = "data",
							DefaultValue = Array<DataRow>.Empty,
							Description = "The data to be deleted."
						},
						new QueryArgument<TimeSpanSecondsGraphType>
						{
							Name = nameof(SqlCommand.Timeout),
							Description = "The SQL command timeout in seconds."
						}),
						Name = Invariant($"delete{table}"),
						Description = Invariant($"DELETE ... OUTPUT ... FROM {objectSchema.Name} ... VALUES ..."),
						Resolver = new SqlApiDeleteFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
					@this.Mutation.AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.DeleteData))
				{
					var fieldType = new FieldType
					{
						Arguments = new(
							new QueryArgument<ListGraphType<GraphQLInputType<Parameter>>>
							{
								Name = "parameters",
								Description = "Used to reference user input values from the where clause."
							},
							new QueryArgument<StringGraphType>
							{
								Name = "where",
								Description = "If `where` is omitted, all records will be deleted!"
							},
							new QueryArgument<TimeSpanSecondsGraphType>
							{
								Name = nameof(SqlCommand.Timeout),
								Description = "The SQL command timeout in seconds."
							}),
						Name = Invariant($"delete{table}Data"),
						Description = Invariant($"DELETE ... OUTPUT ... FROM {objectSchema.Name} ... VALUES ..."),
						Resolver = new SqlApiDeleteFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
					@this.Mutation.AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.Insert))
				{
					var arguments = new QueryArguments();
					arguments.Add<ListGraphType<GraphQLInputType<Parameter>>>("parameters", null, "Used to reference user input values from the where clause.");
					if (dataSource.Type is DataSourceType.SqlServer)
						arguments.Add<StringGraphType>(nameof(SelectQuery.Top));

					arguments.Add<BooleanGraphType>(nameof(SelectQuery.Distinct), false);
					arguments.Add<StringGraphType>(nameof(SelectQuery.Where), null, "If `where` is omitted, all records will be returned.");
					arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)), Array<OrderBy>.Empty);
					arguments.Add<UIntGraphType>(nameof(SelectQuery.Fetch), 0U);
					arguments.Add<UIntGraphType>(nameof(SelectQuery.Offset), 0U);
					arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null);

					var fieldType = new FieldType
					{
						Arguments = arguments,
						Name = Invariant($"insert{table}"),
						Description = Invariant($"INSERT INTO {objectSchema.Name} SELECT ... FROM ... WHERE ... ORDER BY ..."),
						Resolver = new SqlApiInsertFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
					@this.Mutation.AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.InsertData))
				{
					var arguments = new QueryArguments();
					arguments.Add<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("columns", null, "The columns to insert data into.");
					arguments.Add("data", new NonNullGraphType(new ListGraphType(new NonNullGraphType(dataInputType))), null, "The data to be inserted.");

					var fieldType = new FieldType
					{
						Arguments = arguments,
						Name = Invariant($"insert{table}Data"),
						Description = Invariant($"INSERT INTO {objectSchema.Name} ... VALUES ..."),
						Resolver = new SqlApiInsertFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
					@this.Mutation.AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.Update))
				{
					var arguments = new QueryArguments();
					arguments.Add<ListGraphType<NonNullGraphType<GraphQLInputType<Parameter>>>>("parameters", null, "Used to reference user input values from the where clause.");
					arguments.Add<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("set", null, "SET [Column1] = 111, [Column2] = N'111', [Column3] = GETDATE()");
					arguments.Add<StringGraphType>("where", null, "If `where` is omitted, all records will be updated.");

					var fieldType = new FieldType
					{
						Arguments = arguments,
						Name = Invariant($"update{table}"),
						Description = Invariant($"UPDATE {objectSchema.Name} SET ... OUTPUT ... WHERE ..."),
						Resolver = new SqlApiUpdateFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
					@this.Mutation.AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.UpdateData))
				{
					var arguments = new QueryArguments();
					arguments.Add<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("columns", null, "The columns to be updated.");
					arguments.Add("data", new NonNullGraphType(new ListGraphType(new NonNullGraphType(dataInputType))), null, "The data to be inserted.");

					var fieldType = new FieldType
					{
						Arguments = new(
							new QueryArgument(new NonNullGraphType(new ListGraphType(new NonNullGraphType(dataInputType))))
							{
								Name = "set",
								Description = "The columns and data to be updated."
							}),
						Name = Invariant($"update{table}Data"),
						Description = Invariant($"UPDATE {objectSchema.Name} SET ... OUTPUT ..."),
						Resolver = new SqlApiUpdateFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
					@this.Mutation.AddField(fieldType);
				}
			}
		});
	}

	/// <summary>
	/// Adds GraphQL endpoints based on a <typeparamref name="T"/> controller methods decorated with the following attributes:
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
	public static FieldType[] AddEndpoints<T>(this ISchema @this)
	{
		var fieldTypes = new List<FieldType>();

		var methods = TypeOf<T>.Methods.ToArray();
		fieldTypes.AddRange(methods.Where(method => method.Attributes.Any<GraphQLQueryAttribute>()).Select(@this.AddQuery));
		fieldTypes.AddRange(methods.Where(method => method.Attributes.Any<GraphQLMutationAttribute>()).Select(@this.AddMutation));
		fieldTypes.AddRange(methods.Where(method => method.Attributes.Any<GraphQLSubqueryAttribute>()).Select(method =>
		{
			var parentType = method.Attributes.OfType<GraphQLSubqueryAttribute>().First().GetType().GenericTypeArguments[0];
			var fieldType = method.ToFieldType();
			fieldType.Resolver = (IFieldResolver)typeof(ItemLoaderFieldResolver<>).MakeGenericType(parentType).GetTypeMember().Create(method)!;

			@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
			return @this.Query.AddField(fieldType);
		}));
		fieldTypes.AddRange(methods.Where(method => method.Attributes.Any<GraphQLSubqueryBatchAttribute>()).Select(method =>
		{
			var attribute = method.Attributes.OfType<GraphQLSubqueryBatchAttribute>().First();
			return @this.AddSubqueryBatch(method, attribute.ParentType, attribute.Key);
		}));
		fieldTypes.AddRange(methods.Where(method => method.Attributes.Any<GraphQLSubqueryCollectionAttribute>()).Select(method =>
		{
			var attribute = method.Attributes.OfType<GraphQLSubqueryCollectionAttribute>().First();
			return @this.AddSubqueryCollection(method, attribute.ParentType, attribute.Key);
		}));
		fieldTypes.AddRange(methods.Where(method => method.Attributes.Any<GraphQLSubscriptionAttribute>()).Select(@this.AddSubscription));

		return fieldTypes.ToArray();
	}

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
	public static FieldType AddMutation(this ISchema @this, MethodMember method)
	{
		if (method.Return.Void)
			throw new ArgumentException($"{nameof(AddMutation)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		var fieldType = method.ToFieldType();
		fieldType.Resolver = new MethodFieldResolver(method);

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Mutation.AddField(fieldType);
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
	public static FieldType[] AddMutations<T>(this ISchema @this, string method)
		=> TypeOf<T>.Methods.Where(_ => _.Name.Is(method)).Select(@this.AddMutation).ToArray();

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
	public static FieldType[] AddQueries<T>(this ISchema @this, string method)
		=> TypeOf<T>.Methods.Where(_ => _.Name.Is(method)).Select(@this.AddQuery).ToArray();

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
	public static FieldType AddQuery(this ISchema @this, MethodMember method)
	{
		if (method.Return.Void)
			throw new ArgumentException($"{nameof(AddQuery)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		var fieldType = method.ToFieldType();
		fieldType.Resolver = new MethodFieldResolver(method);

		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		return @this.Query.AddField(fieldType);
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
	public static FieldType[] AddSubscriptions<T>(this ISchema @this, string method)
		=> TypeOf<T>.Methods.Where(_ => _.Name.Is(method)).Select(@this.AddSubscription).ToArray();

	/// <summary>
	/// Method parameters with the following type are ignored in the schema and will have their value injected:
	/// <list type="bullet">
	/// <item><see cref="IResolveFieldContext"/></item>
	/// </list>
	/// </summary>
	/// <param name="method">Graph endpoint implementation that returns <see cref="IObservable{T}"/> or a ValueTask or Task of one.</param>
	/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	public static FieldType AddSubscription(this ISchema @this, MethodMember method)
	{
		var returnsObservable = method.Return.Type.IsOrImplements(typeof(IObservable<>));
		if (!returnsObservable && method.Return.Type.SystemType.IsAny(SystemType.ValueTask, SystemType.Task))
			returnsObservable = method.Return.Type.GenericTypes.Single().ObjectType == ObjectType.Observable;

		returnsObservable.AssertTrue();

		var fieldType = method.ToFieldType();
		fieldType.StreamResolver = new MethodSourceStreamResolver(method);

		@this.Subscription ??= new ObjectGraphType { Name = nameof(ISchema.Subscription) };
		return @this.Subscription.AddField(fieldType);
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
	public static FieldType AddSubquery<T>(this ISchema @this, MethodMember method)
	{
		var fieldType = method.ToFieldType();
		fieldType.Resolver = new ItemLoaderFieldResolver<T>(method);

		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		return @this.Query.AddField(fieldType);
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
	public static FieldType AddSubqueryBatch<PARENT, CHILD, KEY>(this ISchema @this, MethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
		where PARENT : class
	{
		var fieldType = method.ToFieldType();
		fieldType.Resolver = new BatchLoaderFieldResolver<PARENT, CHILD, KEY>(method, getParentKey, getChildKey);

		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		return @this.Query.AddField(fieldType);
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
	/// <remarks>The method's containing type must be registered in the <see cref="IServiceCollection"/>, unless the method is static.</remarks>
	/// <param name="method">Graph endpoint implementation.</param>
	/// <param name="parentType">Gets the parent instance type.</param>
	/// <param name="key">The <see cref="GraphQLKeyAttribute"/> used to match parent and child type properties.</param>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static FieldType AddSubqueryBatch(this ISchema @this, MethodMember method, Type parentType, string key)
	{
		if (!method.Return.Type.SystemType.IsCollection())
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: [{nameof(method)}] must return a collection instead of [{method.Return.Type.Name}].");

		if (!parentType.GetTypeMember().Properties.Where(property => property.GraphQLKey()?.Is(key) is true).TryFirst(out var parentKeyProperty)
			|| parentKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var childType = method.Return.Type.ElementType ?? method.Return.Type.GenericTypes.First();
		if (!childType.Properties.Where(property => property.GraphQLKey().Is(key)).TryFirst(out var childKeyProperty)
			|| childKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryBatch)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var resolverType = typeof(BatchLoaderFieldResolver<,,>).MakeGenericType(parentType, (Type)childType, (Type)childKeyProperty.PropertyType);
		var fieldType = method.ToFieldType();
		fieldType.Resolver = (IFieldResolver)resolverType.GetTypeMember().Create(method, parentKeyProperty.Getter.Method, childKeyProperty.Getter.Method)!;

		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		return @this.Query.AddField(fieldType);
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
	public static FieldType AddSubqueryCollection<PARENT, CHILD, KEY>(this ISchema @this, MethodMember method, Func<PARENT, KEY> getParentKey, Func<CHILD, KEY> getChildKey)
		where PARENT : class
	{
		var fieldType = method.ToFieldType();
		fieldType.Resolver = new CollectionLoaderFieldResolver<PARENT, CHILD, KEY>(method, getParentKey, getChildKey);

		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		return @this.Query.AddField(fieldType);
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
	public static FieldType AddSubqueryCollection(this ISchema @this, MethodMember method, Type parentType, string key)
	{
		if (!method.Return.Type.SystemType.IsCollection())
			throw new ArgumentException($"{nameof(AddSubqueryCollection)}: [{nameof(method)}] must return a collection instead of [{method.Return.Type.Name}].");

		if (!parentType.GetTypeMember().Properties.Where(property => property.GraphQLKey()?.Is(key) is true).TryFirst(out var parentKeyProperty)
			|| parentKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryCollection)}: The parent model [{parentType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var childType = method.Return.Type.ElementType ?? method.Return.Type.GenericTypes.First();
		if (!childType.Properties.Where(property => property.GraphQLKey().Is(key)).TryFirst(out var childKeyProperty)
			|| childKeyProperty!.Getter is null)
			throw new ArgumentException($"{nameof(AddSubqueryCollection)}: The child model [{childType.Name}] requires a readable property with [{nameof(GraphQLKeyAttribute)}] having a {nameof(key)} of \"{key}\".");

		var resolverType = typeof(CollectionLoaderFieldResolver<,,>).MakeGenericType(parentType, (Type)childType, (Type)childKeyProperty.PropertyType);

		var fieldType = method.ToFieldType();
		fieldType.Resolver = (IFieldResolver)resolverType.GetTypeMember().Create(method, parentKeyProperty.Getter.Method, childKeyProperty.Getter.Method)!;

		@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
		return @this.Query.AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: delete{Table}</term> Deletes records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: delete{Table}Data</term> Deletes a batch of records based on a table's <c>Primary Key</c>.</item>
	/// <item><term>Mutation: insert{Table}</term> Inserts records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: insert{Table}Data</term> Inserts a batch of records.</item>
	/// <item><term>Query: page{Table}</term> Pages records based on a <c>WHERE</c> clause and <c>FETCH</c> &amp; <c>OFFSET</c>.</item>
	/// <item><term>Query: select{Table}</term> Selects records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: update{Table}</term> Updates records based on a <c>WHERE</c> clause.</item>
	/// <item><term>Mutation: update{Table}Data</term> Updates a batch of records based on a table's <c>Primary Key</c>.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AddSqlApiEndpoints<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		var action = TypeOf<T>.Attributes.FirstOrDefault<SqlApiAttribute>()?.Actions ?? SqlApiAction.CRUD;

		if (action.HasFlag(SqlApiAction.DeleteData))
			@this.AddSqlApiDeleteDataEndpoint<T>(dataSource, table, graphQlName is not null ? Invariant($"delete{graphQlName}") : null);

		if (action.HasFlag(SqlApiAction.Delete))
			@this.AddSqlApiDeleteEndpoint<T>(dataSource, table, graphQlName is not null ? Invariant($"delete{graphQlName}") : null);

		if (action.HasFlag(SqlApiAction.InsertData))
			@this.AddSqlApiInsertDataEndpoint<T>(dataSource, table, graphQlName is not null ? Invariant($"insert{graphQlName}") : null);

		if (action.HasFlag(SqlApiAction.Insert))
			@this.AddSqlApiInsertEndpoint<T>(dataSource, table, graphQlName is not null ? Invariant($"insert{graphQlName}") : null);

		if (action.HasFlag(SqlApiAction.UpdateData))
			@this.AddSqlApiUpdateDataEndpoint<T>(dataSource, table, graphQlName is not null ? Invariant($"update{graphQlName}") : null);

		if (action.HasFlag(SqlApiAction.Update))
			@this.AddSqlApiUpdateEndpoint<T>(dataSource, table, graphQlName is not null ? Invariant($"update{graphQlName}") : null);

		if (action.HasFlag(SqlApiAction.Select))
			@this.AddSqlApiSelectEndpoint<T>(dataSource, table, graphQlName is not null ? Invariant($"select{graphQlName}") : null);
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
	public static FieldType AddSqlApiCallProcedureEndpoint<T>(this ISchema @this, IDataSource dataSource, string procedure, bool mutation, string? graphQlName = null, IGraphType? graphQlType = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		procedure.AssertNotBlank();

		var name = dataSource.CreateName(procedure);
		var objectSchema = dataSource.ObjectSchemas[name];
		var parameters = objectSchema.Parameters
			.Where(_ => _.Direction is ParameterDirection.Input || _.Direction is ParameterDirection.InputOutput)
			.Select(parameter => parameter.Name)
			.ToArray();
		var fieldType = new FieldType
		{
			Arguments = new QueryArguments(parameters.Select(parameter => new QueryArgument(typeof(StringGraphType)) { Name = parameter })),
			Name = graphQlName?.ToCamelCase() ?? Invariant($"call{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"Calls stored procedure: {name}."),
			Resolver = new SqlApiCallFieldResolver<T>()
		};

		if (graphQlType is not null)
			fieldType.ResolvedType = graphQlType;
		else
			fieldType.Type = TypeOf<T>.Member.GraphQLType(false).ToNonNullGraphType();

		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		if (mutation)
		{
			@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
			@this.Mutation.AddField(fieldType);
		}
		else
		{
			@this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };
			@this.Query.AddField(fieldType);
		}

		return fieldType;
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: delete{Table}Data</term> Deletes records passed in based on primary key value(s).</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterSqlApiRules"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiDeleteDataEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		table.AssertNotBlank();

		var name = dataSource.CreateName(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<ListGraphType<GraphQLInputType<T>>>("data", null, "The data to be deleted.");
		arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null, "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"delete{objectSchema.ObjectName.ToPascalCase()}Data"),
			Description = Invariant($"DELETE ... OUTPUT ... FROM {name} ... VALUES ..."),
			Resolver = new SqlApiDeleteFieldResolver<T>(),
			Type = typeof(GraphQLObjectType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Mutation.AddField(fieldType);
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
	public static FieldType AddSqlApiDeleteEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		table.AssertNotBlank();

		var name = dataSource.CreateName(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<ListGraphType<NonNullGraphType<GraphQLInputType<Parameter>>>>("parameters", null, "Used to reference user input values from the where clause.");
		arguments.Add<StringGraphType>("where", null, "If `where` is omitted, all records will be deleted!");
		arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null, "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"delete{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"DELETE ... OUTPUT ... FROM {name} WHERE ..."),
			Resolver = new SqlApiDeleteFieldResolver<T>(),
			Type = typeof(GraphQLObjectType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Mutation.AddField(fieldType);
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
	public static FieldType AddSqlApiInsertDataEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		table.AssertNotBlank();

		var name = dataSource.CreateName(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("columns", null, "The columns to insert data into.");
		arguments.Add<NonNullGraphType<ListGraphType<NonNullGraphType<GraphQLInputType<T>>>>>("data", null, "The data to be inserted.");
		arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null, "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"insert{objectSchema.ObjectName.ToPascalCase()}Data"),
			Description = Invariant($"INSERT INTO {name} ... VALUES ..."),
			Resolver = new SqlApiInsertFieldResolver<T>(),
			Type = typeof(GraphQLObjectType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Mutation.AddField(fieldType);
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
	public static FieldType AddSqlApiInsertEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		table.AssertNotBlank();

		var name = dataSource.CreateName(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var graphOrderByEnum = new EnumerationGraphType
		{
			Name = Invariant($"{TypeOf<T>.Member.GraphQLName()}OrderBy"),
		};
		foreach (var property in TypeOf<T>.Member.Properties.Where(property => !property.GraphQLIgnore()))
		{
			var propertyName = property.GraphQLName();
			var propertyDeprecationReason = property.GraphQLDeprecationReason();

			graphOrderByEnum.AddOrderBy(new(propertyName, Sort.Ascending), propertyDeprecationReason);
			graphOrderByEnum.AddOrderBy(new(propertyName, Sort.Descending), propertyDeprecationReason);
		}

		var arguments = new QueryArguments();
		arguments.Add<ListGraphType<NonNullGraphType<GraphQLInputType<Parameter>>>>("parameters", null, "Used to reference user input values from the where clause.");
		if (dataSource.Type is DataSourceType.SqlServer)
			arguments.Add<StringGraphType>(nameof(SelectQuery.Top));

		arguments.Add<BooleanGraphType>(nameof(SelectQuery.Distinct), false);
		arguments.Add<NonNullGraphType<StringGraphType>>(nameof(SelectQuery.From), null, "The table or view to pull the data from to insert.");
		arguments.Add<StringGraphType>(nameof(SelectQuery.Where), null, "If `where` is omitted, all records will be returned.");
		arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)), Array<OrderBy>.Empty);
		arguments.Add<UIntGraphType>(nameof(SelectQuery.Fetch), 0U);
		arguments.Add<UIntGraphType>(nameof(SelectQuery.Offset), 0U);
		arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null, "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"insert{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"INSERT INTO {name} SELECT ... FROM ... WHERE ... ORDER BY ..."),
			Resolver = new SqlApiInsertFieldResolver<T>(),
			Type = typeof(GraphQLObjectType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Mutation.AddField(fieldType);
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
	public static FieldType AddSqlApiSelectEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		table.AssertNotBlank();

		var name = dataSource.CreateName(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var graphOrderByEnum = new EnumerationGraphType
		{
			Name = Invariant($"{TypeOf<T>.Member.GraphQLName()}OrderBy"),
		};
		foreach (var property in TypeOf<T>.Member.Properties.Where(property => !property.GraphQLIgnore()))
		{
			var propertyName = property.GraphQLName();
			var propertyDeprecationReason = property.GraphQLDeprecationReason();

			graphOrderByEnum.AddOrderBy(new(propertyName, Sort.Ascending), propertyDeprecationReason);
			graphOrderByEnum.AddOrderBy(new(propertyName, Sort.Descending), propertyDeprecationReason);
		}

		var arguments = new QueryArguments();
		arguments.Add<ListGraphType<NonNullGraphType<GraphQLInputType<Parameter>>>>("parameters", null, "Used to reference user input values from the where clause.");
		if (dataSource.Type is DataSourceType.SqlServer)
			arguments.Add<StringGraphType>(nameof(SelectQuery.Top));

		arguments.Add<BooleanGraphType>(nameof(SelectQuery.Distinct), false);
		arguments.Add<StringGraphType>(nameof(SelectQuery.Where), null, "If `where` is omitted, all records will be returned.");
		arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)), Array<OrderBy>.Empty);
		arguments.Add<UIntGraphType>(nameof(SelectQuery.Fetch), 0U);
		arguments.Add<UIntGraphType>(nameof(SelectQuery.Offset), 0U);
		arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null, "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"select{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"SELECT ... FROM {name} WHERE ... ORDER BY ..."),
			Resolver = new SqlApiSelectFieldResolver<T>(),
			Type = typeof(GraphQLObjectType<SelectResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Query.AddField(fieldType);
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
	public static FieldType AddSqlApiUpdateDataEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		table.AssertNotBlank();

		var name = dataSource.CreateName(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<NonNullGraphType<ListGraphType<NonNullGraphType<GraphQLInputType<T>>>>>("set", null, "The columns to be updated.");
		arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null, "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"update{objectSchema.ObjectName.ToPascalCase()}Data"),
			Description = Invariant($"UPDATE {name} SET ... OUTPUT ..."),
			Resolver = new SqlApiUpdateFieldResolver<T>(),
			Type = typeof(GraphQLObjectType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Mutation.AddField(fieldType);
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
	public static FieldType AddSqlApiUpdateEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : new()
	{
		dataSource.AssertNotNull();
		table.AssertNotBlank();

		var name = dataSource.CreateName(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<ListGraphType<NonNullGraphType<GraphQLInputType<Parameter>>>>("parameters", null, "Used to reference user input values from the where clause.");
		arguments.Add<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("set", null, "SET [Column1] = 111, [Column2] = N'111', [Column3] = GETDATE()");
		arguments.Add<StringGraphType>("where", null, "If `where` is omitted, all records will be updated.");
		arguments.Add<TimeSpanSecondsGraphType>(nameof(SqlCommand.Timeout), null, "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName ?? Invariant($"update{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"UPDATE {name} SET ... OUTPUT ... WHERE ..."),
			Resolver = new SqlApiUpdateFieldResolver<T>(),
			Type = typeof(GraphQLObjectType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		@this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };
		return @this.Mutation.AddField(fieldType);
	}

	private static string FixName(string name)
	{
		if (!name.Contains('_'))
			return name;

		return string.Join(string.Empty, name.ToLowerInvariant().Split('_').Select(_ => _.ToPascalCase()));
	}
}
