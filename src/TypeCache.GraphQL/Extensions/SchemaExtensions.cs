// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Attributes;
using TypeCache.Collections;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SqlApi;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static class SchemaExtensions
{
	public static IObjectGraphType Query(this ISchema @this)
		=> @this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };

	public static IObjectGraphType Mutation(this ISchema @this)
		=> @this.Mutation ??= new ObjectGraphType { Name = nameof(ISchema.Mutation) };

	public static IObjectGraphType Subscription(this ISchema @this)
		=> @this.Subscription ??= new ObjectGraphType { Name = nameof(ISchema.Subscription) };

	/// <summary>
	/// Adds a GraphQL endpoint that returns the version of the GraphQL schema.
	/// </summary>
	/// <param name="version">The version of this GraphQL schema</param>
	public static FieldType AddVersion(this ISchema @this, string version)
	{
		var field = @this.AddQuery("Version", () => version);
		field.Description = Invariant($"The version of this GraphQL Schema: {version}.");
		return field;
	}

	/// <summary>
	/// Adds all of the queries for all of the database schema data made available by the data provider.
	/// </summary>
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
	/// <exception cref="ArgumentNullException"/>
	public static FieldType AddDatabaseSchemaQuery(this ISchema @this, IDataSource dataSource, SchemaCollection collection)
	{
		dataSource.ThrowIfNull();
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
			graphOrderByEnum.AddOrderBy(column.ColumnName);

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
					_ when column.AllowDBNull => column.DataType.ToGraphQLType(false),
					_ => column.DataType.ToGraphQLType(false).ToNonNullGraphType()
				}
			});
			field.Metadata.Add(nameof(DataColumn.ColumnName), column.ColumnName);
			field.Metadata.Add(nameof(DataColumn.DataType), column.DataType.TypeHandle);
		}

		var fieldType = @this.Query().AddField(new()
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

		dataSource.ThrowIfNull();

		database ??= dataSource.DefaultDatabase;
		var objectSchemas = dataSource.ObjectSchemas.Values;
		if (schema.IsNotBlank())
			objectSchemas = objectSchemas.Where(_ => _.DatabaseName.EqualsIgnoreCase(database) && _.SchemaName.EqualsIgnoreCase(schema));
		else if (database.IsNotBlank())
			objectSchemas = objectSchemas.Where(_ => _.DatabaseName.EqualsIgnoreCase(database));

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
						_ => columnDataType.ToGraphQLType(false)
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
						_ when column.Nullable => columnDataType.ToGraphQLType(false),
						_ => columnDataType.ToGraphQLType(false).ToNonNullGraphType()
					}
				});
				field.Metadata.Add(ColumnName, column.Name);
				field.Metadata.Add(ColumnType, column.DataTypeHandle);

				graphOrderByEnum.AddOrderBy(column.Name);
			}

			if ((objectSchema.Type is DatabaseObjectType.Table || objectSchema.Type is DatabaseObjectType.View) && actions.HasFlag(SqlApiAction.Select))
			{
				var selectResponseType = SelectResponse<DataRow>.CreateGraphType(table, Invariant($"{objectSchema.Type.Name()}: `{objectSchema.Name}`"), resolvedType);
				var arguments = new QueryArguments();
				arguments.Add<Parameter[]>("parameters", nullable: true, description: "Used to reference user input values from the where clause.");
				if (dataSource.Type is DataSourceType.SqlServer)
					arguments.Add<string>(nameof(SelectQuery.Top), nullable: true, description: "Accepts integer `n` or `n%`.");

				arguments.Add<bool>(nameof(SelectQuery.Distinct), defaultValue: false);
				arguments.Add<string>(nameof(SelectQuery.Where), nullable: true, description: "If `where` is omitted, all records will be returned.");
				arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)));
				arguments.Add<uint>(nameof(SelectQuery.Fetch), defaultValue: 0U);
				arguments.Add<uint>(nameof(SelectQuery.Offset), defaultValue: 0U);
				arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U);

				var field = new FieldType
				{
					Arguments = arguments,
					Name = Invariant($"select{table}"),
					Description = Invariant($"SELECT ... FROM {objectSchema.Name} WHERE ... ORDER BY ..."),
					Resolver = new SqlApiSelectFieldResolver(),
					ResolvedType = selectResponseType
				};
				field.Metadata[nameof(ObjectSchema)] = objectSchema;

				@this.Query().AddField(field);
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
							DefaultValue = 120U,
							Description = "The SQL command timeout in seconds."
						}),
						Name = Invariant($"delete{table}"),
						Description = Invariant($"DELETE ... OUTPUT ... FROM {objectSchema.Name} ... VALUES ..."),
						Resolver = new SqlApiDeleteFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation().AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.DeleteData))
				{
					var fieldType = new FieldType
					{
						Arguments = new(
							new QueryArgument<ListGraphType<InputGraphType<Parameter>>>
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
								DefaultValue = 120U,
								Description = "The SQL command timeout in seconds."
							}),
						Name = Invariant($"delete{table}Data"),
						Description = Invariant($"DELETE ... OUTPUT ... FROM {objectSchema.Name} ... VALUES ..."),
						Resolver = new SqlApiDeleteFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation().AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.Insert))
				{
					var arguments = new QueryArguments();
					arguments.Add<Parameter[]>("parameters", nullable: true, description: "Used to reference user input values from the where clause.");
					if (dataSource.Type is DataSourceType.SqlServer)
						arguments.Add<string>(nameof(SelectQuery.Top), nullable: true, description: "Accepts integer `n` or `n%`.");

					arguments.Add<bool>(nameof(SelectQuery.Distinct), defaultValue: false);
					arguments.Add<string>(nameof(SelectQuery.Where), nullable: true, description: "If `where` is omitted, all records will be returned.");
					arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)));
					arguments.Add<uint>(nameof(SelectQuery.Fetch), defaultValue: 0U);
					arguments.Add<uint>(nameof(SelectQuery.Offset), defaultValue: 0U);
					arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U);

					var fieldType = new FieldType
					{
						Arguments = arguments,
						Name = Invariant($"insert{table}"),
						Description = Invariant($"INSERT INTO {objectSchema.Name} SELECT ... FROM ... WHERE ... ORDER BY ..."),
						Resolver = new SqlApiInsertFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation().AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.InsertData))
				{
					var arguments = new QueryArguments();
					arguments.Add<string[]>("columns", description: "The columns to insert data into.");
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

					@this.Mutation().AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.Update))
				{
					var arguments = new QueryArguments();
					arguments.Add<Parameter[]>("parameters", nullable: true, description: "Used to reference user input values from the where clause.");
					arguments.Add<string[]>("set", description: "SET [Column1] = 111, [Column2] = N'111', [Column3] = GETDATE()");
					arguments.Add<string>("where", nullable: true, description: "If `where` is omitted, all records will be updated.");

					var fieldType = new FieldType
					{
						Arguments = arguments,
						Name = Invariant($"update{table}"),
						Description = Invariant($"UPDATE {objectSchema.Name} SET ... OUTPUT ... WHERE ..."),
						Resolver = new SqlApiUpdateFieldResolver(),
						ResolvedType = outputResponseType
					};
					fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

					@this.Mutation().AddField(fieldType);
				}

				if (actions.HasFlag(SqlApiAction.UpdateData))
				{
					var arguments = new QueryArguments();
					arguments.Add<string[]>("columns", description: "The columns to be updated.");
					arguments.Add("data", new NonNullGraphType(new ListGraphType(new NonNullGraphType(dataInputType))), description: "The data to be inserted.");

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

					@this.Mutation().AddField(fieldType);
				}
			}
		});
	}

	/// <summary>
	/// Adds GraphQL endpoints based on <typeparamref name="T"/> controller public methods decorated with the following attributes:
	/// <list type="bullet">
	/// <item><see cref="GraphQLQueryAttribute"/></item>
	/// <item><see cref="GraphQLMutationAttribute"/></item>
	/// <item><see cref="GraphQLSubscriptionAttribute"/></item>
	/// </list>
	/// </summary>
	/// <typeparam name="T">The class containing the decorated public methods that will be converted into GraphQL endpoints.</typeparam>
	/// <returns>The added <see cref="FieldType"/>(s).</returns>
	public static FieldType[] AddEndpoints<T>(this ISchema @this)
		where T : notnull
	{
		var methods = Type<T>.Methods.SelectMany(_ => _.Value).Where(_ => _.IsPublic).ToArray();
		var fieldTypes = new List<FieldType>();
		fieldTypes.AddRange(methods.Where(_ => _.Attributes.Any<GraphQLQueryAttribute>()).Select(@this.AddQuery));
		fieldTypes.AddRange(methods.Where(_ => _.Attributes.Any<GraphQLMutationAttribute>()).Select(@this.AddMutation));
		fieldTypes.AddRange(methods.Where(_ => _.Attributes.Any<GraphQLSubscriptionAttribute>()).Select(@this.AddSubscription));

		var staticMethods = Type<T>.StaticMethods.SelectMany(_ => _.Value).Where(_ => _.IsPublic).ToArray();
		fieldTypes.AddRange(staticMethods.Where(_ => _.Attributes.Any<GraphQLQueryAttribute>()).Select(@this.AddQuery));
		fieldTypes.AddRange(staticMethods.Where(_ => _.Attributes.Any<GraphQLMutationAttribute>()).Select(@this.AddMutation));
		fieldTypes.AddRange(staticMethods.Where(_ => _.Attributes.Any<GraphQLSubscriptionAttribute>()).Select(@this.AddSubscription));

		return fieldTypes.ToArray();
	}

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddMutation<R>(this ISchema @this, string name, Func<R> handler)
		=> @this.Mutation().AddField(new()
		{
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler()),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddMutation<T, R>(this ISchema @this, string name, string argument, Func<T, R> handler)
		=> @this.Mutation().AddField(new()
		{
			Arguments = new(new QueryArgument(typeof(T).ToGraphQLType(true).ToNonNullGraphType()) { Name = argument }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(context.GetArgument<T>(argument))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddMutation<T1, T2, R>(this ISchema @this, string name, (string, string) arguments, Func<T1, T2, R> handler)
		=> @this.Mutation().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddMutation<T1, T2, T3, R>(this ISchema @this, string name, (string, string, string) arguments, Func<T1, T2, T3, R> handler)
		=> @this.Mutation().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddMutation<T1, T2, T3, T4, R>(this ISchema @this, string name, (string, string, string, string) arguments, Func<T1, T2, T3, T4, R> handler)
		=> @this.Mutation().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
				new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3),
				context.GetArgument<T4>(arguments.Item4))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddMutation<T1, T2, T3, T4, T5, R>(this ISchema @this, string name, (string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, R> handler)
		=> @this.Mutation().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
				new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 },
				new QueryArgument(typeof(T5).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item5 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3),
				context.GetArgument<T4>(arguments.Item4),
				context.GetArgument<T5>(arguments.Item5))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddMutation<T1, T2, T3, T4, T5, T6, R>(this ISchema @this, string name, (string, string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, T6, R> handler)
		=> @this.Mutation().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
				new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 },
				new QueryArgument(typeof(T5).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item5 },
				new QueryArgument(typeof(T6).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item6 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3),
				context.GetArgument<T4>(arguments.Item4),
				context.GetArgument<T5>(arguments.Item5),
				context.GetArgument<T6>(arguments.Item6))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// Method parameters with the following type are ignored in the schema and will have their value injected:
	/// <list type="bullet">
	/// <item><see cref="IResolveFieldContext"/></item>
	/// </list>
	/// </summary>
	/// <remarks>The method's declaring type must be registered in the <see cref="IServiceCollection"/>.</remarks>
	/// <param name="method">Graph endpoint implementation.</param>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	public static FieldType AddMutation(this ISchema @this, MethodEntity method)
	{
		if (!method.HasReturnValue)
			throw new ArgumentException($"{nameof(AddMutation)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		return @this.Mutation().AddField(method);
	}

	/// <summary>
	/// Method parameters with the following type are ignored in the schema and will have their value injected:
	/// <list type="bullet">
	/// <item><see cref="IResolveFieldContext"/></item>
	/// </list>
	/// </summary>
	/// <param name="method">Graph endpoint implementation.</param>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	public static FieldType AddMutation(this ISchema @this, StaticMethodEntity method)
	{
		if (!method.HasReturnValue)
			throw new ArgumentException($"{nameof(AddMutation)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		return @this.Mutation().AddField(method);
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
		where T : notnull
	{
		var publicMethods = Type<T>.Methods[method]
			.Where(_ => _.IsPublic)
			.Select(@this.AddMutation);
		var publicStaticMethods = Type<T>.StaticMethods[method]
			.Where(_ => _.IsPublic)
			.Select(@this.AddMutation);
		return [..publicMethods, ..publicStaticMethods];
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
	public static FieldType[] AddQueries<T>(this ISchema @this, string method)
		where T : notnull
	{
		var publicMethods = Type<T>.Methods[method]
			.Where(_ => _.IsPublic)
			.Select(@this.AddQuery);
		var publicStaticMethods = Type<T>.StaticMethods[method]
			.Where(_ => _.IsPublic)
			.Select(@this.AddQuery);
		return [.. publicMethods, .. publicStaticMethods];
	}

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddQuery<R>(this ISchema @this, string name, Func<R> handler)
		=> @this.Query().AddField(new()
		{
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler()),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddQuery<T, R>(this ISchema @this, string name, string argument, Func<T, R> handler)
		=> @this.Query().AddField(new()
		{
			Arguments = new(new QueryArgument(typeof(T).ToGraphQLType(true).ToNonNullGraphType()) { Name = argument }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(context.GetArgument<T>(argument))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddQuery<T1, T2, R>(this ISchema @this, string name, (string, string) arguments, Func<T1, T2, R> handler)
		=> @this.Query().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddQuery<T1, T2, T3, R>(this ISchema @this, string name, (string, string, string) arguments, Func<T1, T2, T3, R> handler)
		=> @this.Query().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddQuery<T1, T2, T3, T4, R>(this ISchema @this, string name, (string, string, string, string) arguments, Func<T1, T2, T3, T4, R> handler)
		=> @this.Query().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
				new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3),
				context.GetArgument<T4>(arguments.Item4))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddQuery<T1, T2, T3, T4, T5, R>(this ISchema @this, string name, (string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, R> handler)
		=> @this.Query().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
				new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 },
				new QueryArgument(typeof(T5).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item5 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3),
				context.GetArgument<T4>(arguments.Item4),
				context.GetArgument<T5>(arguments.Item5))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// <b>GraphQL Minimal API.</b>
	/// </summary>
	public static FieldType AddQuery<T1, T2, T3, T4, T5, T6, R>(this ISchema @this, string name, (string, string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, T6, R> handler)
		=> @this.Query().AddField(new()
		{
			Arguments = new(
				new QueryArgument(typeof(T1).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item1 },
				new QueryArgument(typeof(T2).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item2 },
				new QueryArgument(typeof(T3).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item3 },
				new QueryArgument(typeof(T4).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item4 },
				new QueryArgument(typeof(T5).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item5 },
				new QueryArgument(typeof(T6).ToGraphQLType(true).ToNonNullGraphType()) { Name = arguments.Item6 }),
			Name = name,
			Resolver = new FuncFieldResolver<R>(context => handler(
				context.GetArgument<T1>(arguments.Item1),
				context.GetArgument<T2>(arguments.Item2),
				context.GetArgument<T3>(arguments.Item3),
				context.GetArgument<T4>(arguments.Item4),
				context.GetArgument<T5>(arguments.Item5),
				context.GetArgument<T6>(arguments.Item6))),
			Type = typeof(R).ToGraphQLType(false)
		});

	/// <summary>
	/// Method parameters with the following type are ignored in the schema and will have their value injected:
	/// <list type="bullet">
	/// <item><see cref="IResolveFieldContext"/></item>
	/// </list>
	/// </summary>
	/// <param name="method">Graph endpoint implementation.</param>
	/// <remarks>Any method's declaring type instance must be registered in the <see cref="IServiceCollection"/>.</remarks>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	public static FieldType AddQuery(this ISchema @this, MethodEntity method)
	{
		if (method.Return.ParameterType.IsAny([typeof(void), typeof(Task), typeof(ValueTask)]))
			throw new ArgumentException($"{nameof(AddQuery)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		return @this.Query().AddField(method);
	}

	/// <summary>
	/// Method parameters with the following type are ignored in the schema and will have their value injected:
	/// <list type="bullet">
	/// <item><see cref="IResolveFieldContext"/></item>
	/// </list>
	/// </summary>
	/// <param name="method">Graph endpoint implementation.</param>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	public static FieldType AddQuery(this ISchema @this, StaticMethodEntity method)
	{
		if (method.Return.ParameterType.IsAny([typeof(void), typeof(Task), typeof(ValueTask)]))
			throw new ArgumentException($"{nameof(AddQuery)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

		return @this.Query().AddField(method);
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
		where T : notnull
	{
		var publicMethods = Type<T>.Methods[method]
			.Where(_ => _.IsPublic)
			.Select(@this.AddSubscription);
		var publicStaticMethods = Type<T>.StaticMethods[method]
			.Where(_ => _.IsPublic)
			.Select(@this.AddSubscription);
		return [.. publicMethods, .. publicStaticMethods];
	}

	/// <summary>
	/// Method parameters with the following type are ignored in the schema and will have their value injected:
	/// <list type="bullet">
	/// <item><see cref="IResolveFieldContext"/></item>
	/// </list>
	/// </summary>
	/// <param name="method">Graph endpoint implementation that returns <c><see cref="IObservable{T}"/></c>, <c>Task&lt;<see cref="IObservable{T}"/>&gt;</c> or <c>ValueTask&lt;<see cref="IObservable{T}"/>&gt;</c>.</param>
	/// <remarks>The method's type must be registered in the <see cref="IServiceCollection"/>.</remarks>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	public static FieldType AddSubscription(this ISchema @this, MethodEntity method)
		=> @this.Subscription().AddFieldStream(method);

	/// <summary>
	/// Method parameters with the following type are ignored in the schema and will have their value injected:
	/// <list type="bullet">
	/// <item><see cref="IResolveFieldContext"/></item>
	/// </list>
	/// </summary>
	/// <param name="method">Graph endpoint implementation that returns <c><see cref="IObservable{T}"/></c>, <c>Task&lt;<see cref="IObservable{T}"/>&gt;</c> or <c>ValueTask&lt;<see cref="IObservable{T}"/>&gt;</c>.</param>
	/// <returns>The added <see cref="FieldType"/>.</returns>
	/// <exception cref="ArgumentException"/>
	public static FieldType AddSubscription(this ISchema @this, StaticMethodEntity method)
		=> @this.Subscription().AddFieldStream(method);

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
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AddSqlApiEndpoints<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		var action = typeof(T).GetCustomAttribute<SqlApiAttribute>()?.Actions ?? SqlApiAction.CRUD;

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
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiCallProcedureEndpoint<T>(this ISchema @this, IDataSource dataSource, string procedure, bool mutation, string? graphQlName = null, IGraphType? graphQlType = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		procedure.ThrowIfBlank();

		var name = dataSource.Escape(procedure);
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
			fieldType.Type = typeof(T).ToGraphQLType(false).ToNonNullGraphType();

		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		if (mutation)
			@this.Mutation().AddField(fieldType);
		else
			@this.Query().AddField(fieldType);

		return fieldType;
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: delete{Table}Data</term> Deletes records passed in based on primary key value(s).</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiDeleteDataEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		table.ThrowIfBlank();

		var name = dataSource.Escape(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<T[]>("data", description: "The data to be deleted.");
		arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U, description: "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"delete{objectSchema.ObjectName.ToPascalCase()}Data"),
			Description = Invariant($"DELETE ... OUTPUT ... FROM {name} ... VALUES ..."),
			Resolver = new SqlApiDeleteFieldResolver<T>(),
			Type = typeof(OutputGraphType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		return @this.Mutation().AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoints:
	/// <list type="table">
	/// <item><term>Mutation: Delete{Table}</term> Deletes records based on a <c>WHERE</c> clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiDeleteEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		table.ThrowIfBlank();

		var name = dataSource.Escape(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<Parameter[]>("parameters", nullable: true, description: "Used to reference user input values from the where clause.");
		arguments.Add<string>("where", nullable: true, description: "If `where` is omitted, all records will be deleted!");
		arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U, description: "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"delete{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"DELETE ... OUTPUT ... FROM {name} WHERE ..."),
			Resolver = new SqlApiDeleteFieldResolver<T>(),
			Type = typeof(OutputGraphType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		return @this.Mutation().AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: insert{Table}Data</term> Inserts a batch of records.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiInsertDataEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		table.ThrowIfBlank();

		var name = dataSource.Escape(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<string[]>("columns", description: "The columns to insert data into.");
		arguments.Add<T[]>("data", description: "The data to be inserted.");
		arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U, description: "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"insert{objectSchema.ObjectName.ToPascalCase()}Data"),
			Description = Invariant($"INSERT INTO {name} ... VALUES ..."),
			Resolver = new SqlApiInsertFieldResolver<T>(),
			Type = typeof(OutputGraphType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		return @this.Mutation().AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: insert{Table}Data</term> Inserts a batch of records.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiInsertEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		table.ThrowIfBlank();

		var name = dataSource.Escape(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var graphOrderByEnum = new EnumerationGraphType
		{
			Name = Invariant($"{Type<T>.Attributes.GraphQLName() ?? Type<T>.Name}OrderBy"),
		};
		foreach (var property in Type<T>.Properties.Values.Where(_ => !_.Attributes.GraphQLIgnore()))
		{
			var propertyName = property.Attributes.GraphQLName() ?? property.Name;
			var propertyDeprecationReason = property.Attributes.GraphQLDeprecationReason();

			graphOrderByEnum.AddOrderBy(propertyName, propertyDeprecationReason);
		}

		var arguments = new QueryArguments();
		arguments.Add<Parameter[]>("parameters", nullable: true, description: "Used to reference user input values from the where clause.");
		if (dataSource.Type is DataSourceType.SqlServer)
			arguments.Add<string>(nameof(SelectQuery.Top), nullable: true, description: "Accepts integer `n` or `n%`.");

		arguments.Add<bool>(nameof(SelectQuery.Distinct), defaultValue: false);
		arguments.Add<string>(nameof(SelectQuery.From), description: "The table or view to pull the data from to insert.");
		arguments.Add<string>(nameof(SelectQuery.Where), nullable: true, description: "If `where` is omitted, all records will be returned.");
		arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)));
		arguments.Add<uint>(nameof(SelectQuery.Fetch), defaultValue: 0U);
		arguments.Add<uint>(nameof(SelectQuery.Offset), defaultValue: 0U);
		arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U, description: "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"insert{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"INSERT INTO {name} SELECT ... FROM ... WHERE ... ORDER BY ..."),
			Resolver = new SqlApiInsertFieldResolver<T>(),
			Type = typeof(OutputGraphType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		return @this.Mutation().AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Query: select{Table}</term> Selects records based on a <c>WHERE</c> clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiSelectEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		table.ThrowIfBlank();

		var name = dataSource.Escape(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var graphOrderByEnum = new EnumerationGraphType
		{
			Name = Invariant($"{Type<T>.Attributes.GraphQLName() ?? Type<T>.Name}OrderBy"),
		};
		foreach (var property in Type<T>.Properties.Values.Where(_ => !_.Attributes.GraphQLIgnore()))
		{
			var propertyName = property.Attributes.GraphQLName() ?? property.Name;
			var propertyDeprecationReason = property.Attributes.GraphQLDeprecationReason();

			graphOrderByEnum.AddOrderBy(propertyName, propertyDeprecationReason);
		}

		var arguments = new QueryArguments();
		arguments.Add<Parameter[]>("parameters", nullable: true, description: "Used to reference user input values from the where clause.");
		if (dataSource.Type is DataSourceType.SqlServer)
			arguments.Add<string>(nameof(SelectQuery.Top), nullable: true, description: "Accepts integer `n` or `n%`.");

		arguments.Add<bool>(nameof(SelectQuery.Distinct), defaultValue: false);
		arguments.Add<string>(nameof(SelectQuery.Where), nullable: true, description: "If `where` is omitted, all records will be returned.");
		arguments.Add(nameof(SelectQuery.OrderBy), new ListGraphType(new NonNullGraphType(graphOrderByEnum)));
		arguments.Add<uint>(nameof(SelectQuery.Fetch), defaultValue: 0U);
		arguments.Add<uint>(nameof(SelectQuery.Offset), defaultValue: 0U);
		arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U, description: "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"select{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"SELECT ... FROM {name} WHERE ... ORDER BY ..."),
			Resolver = new SqlApiSelectFieldResolver<T>(),
			Type = typeof(OutputGraphType<SelectResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		return @this.Query().AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: update{Table}Data</term> Updates a batch of records.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiUpdateDataEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		table.ThrowIfBlank();

		var name = dataSource.Escape(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<T[]>("set", description: "The columns to be updated.");
		arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U, description: "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName?.ToCamelCase() ?? Invariant($"update{objectSchema.ObjectName.ToPascalCase()}Data"),
			Description = Invariant($"UPDATE {name} SET ... OUTPUT ..."),
			Resolver = new SqlApiUpdateFieldResolver<T>(),
			Type = typeof(OutputGraphType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		return @this.Mutation().AddField(fieldType);
	}

	/// <summary>
	/// Creates the following GraphQL endpoint:
	/// <list type="table">
	/// <item><term>Mutation: update{Table}</term> Updates records based on a WHERE clause.</item>
	/// </list>
	/// <i>Requires call to:</i>
	/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static FieldType AddSqlApiUpdateEndpoint<T>(this ISchema @this, IDataSource dataSource, string table, string? graphQlName = null)
		where T : notnull, new()
	{
		dataSource.ThrowIfNull();
		table.ThrowIfBlank();

		var name = dataSource.Escape(table);
		var objectSchema = dataSource.ObjectSchemas[name];
		var arguments = new QueryArguments();
		arguments.Add<Parameter[]>("parameters", nullable: true, description: "Used to reference user input values from the where clause.");
		arguments.Add<string[]>("set", description: "SET [Column1] = 111, [Column2] = N'111', [Column3] = GETDATE()");
		arguments.Add<string>("where", nullable: true, description: "If `where` is omitted, all records will be updated.");
		arguments.Add<uint>(nameof(SqlCommand.Timeout), defaultValue: 120U, description: "SQL Command timeout in seconds.");

		var fieldType = new FieldType
		{
			Arguments = arguments,
			Name = graphQlName ?? Invariant($"update{objectSchema.ObjectName.ToPascalCase()}"),
			Description = Invariant($"UPDATE {name} SET ... OUTPUT ... WHERE ..."),
			Resolver = new SqlApiUpdateFieldResolver<T>(),
			Type = typeof(OutputGraphType<OutputResponse<T>>)
		};
		fieldType.Metadata[nameof(ObjectSchema)] = objectSchema;

		return @this.Mutation().AddField(fieldType);
	}

	private static string FixName(string name)
	{
		if (!name.Contains('_'))
			return name;

		return name.ToLowerInvariant().Split('_').Select(_ => _.ToPascalCase()).Concat();
	}
}
