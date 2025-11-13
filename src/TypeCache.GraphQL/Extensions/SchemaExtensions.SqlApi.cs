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
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SqlApi;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static partial class SchemaExtensions
{
	extension(ISchema @this)
	{
		/// <exception cref="ArgumentNullException"/>
		public void AddDatabaseEndpoints(IDataSource dataSource, SqlApiAction actions, string? database = null, string? schema = null)
		{
			const string ColumnName = nameof(ColumnName);
			const string ColumnType = nameof(ColumnType);

			dataSource.ThrowIfNull();

			database ??= dataSource.DefaultDatabase;
			var objectSchemas = dataSource.ObjectSchemas.Values;
			if (schema.IsNotBlank)
				objectSchemas = objectSchemas.Where(_ => _.DatabaseName.EqualsIgnoreCase(database) && _.SchemaName.EqualsIgnoreCase(schema));
			else if (database.IsNotBlank)
				objectSchemas = objectSchemas.Where(_ => _.DatabaseName.EqualsIgnoreCase(database));

			objectSchemas.ForEach(objectSchema =>
			{
				var table = objectSchema.ObjectName.ToPascalCase();
				var dataInputType = new InputObjectGraphType
				{
					Name = Invariant($"{objectSchema.ObjectName}Input"),
					Description = Invariant($"{objectSchema.Type.Name}: `{objectSchema.Name}`")
				};
				var graphOrderByEnum = new EnumerationGraphType
				{
					Name = Invariant($"{table}OrderBy")
				};
				var resolvedType = new ObjectGraphType
				{
					Name = objectSchema.ObjectName,
					Description = Invariant($"{objectSchema.Type.Name}: `{objectSchema.Name}`")
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
					var selectResponseType = SelectResponse<DataRow>.CreateGraphType(table, Invariant($"{objectSchema.Type.Name}: `{objectSchema.Name}`"), resolvedType);
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

					@this.GetQuery().AddField(field);
				}

				if (objectSchema.Type is DatabaseObjectType.Table)
				{
					var outputResponseType = OutputResponse<DataRow>.CreateGraphType(table, $"{objectSchema.Type.Name}: `{objectSchema.Name}`", resolvedType);

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

						@this.GetMutation().AddField(fieldType);
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

						@this.GetMutation().AddField(fieldType);
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

						@this.GetMutation().AddField(fieldType);
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

						@this.GetMutation().AddField(fieldType);
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

						@this.GetMutation().AddField(fieldType);
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

						@this.GetMutation().AddField(fieldType);
					}
				}
			});
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
		/// <code><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddSqlCommandRules(IServiceCollection)"/></code>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void AddSqlApiEndpoints<T>(IDataSource dataSource, string table, string? graphQlName = null)
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
		public FieldType AddSqlApiCallProcedureEndpoint<T>(IDataSource dataSource, string procedure, bool mutation, string? graphQlName = null, IGraphType? graphQlType = null)
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
				@this.GetMutation().AddField(fieldType);
			else
				@this.GetQuery().AddField(fieldType);

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
		public FieldType AddSqlApiDeleteDataEndpoint<T>(IDataSource dataSource, string table, string? graphQlName = null)
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

			return @this.GetMutation().AddField(fieldType);
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
		public FieldType AddSqlApiDeleteEndpoint<T>(IDataSource dataSource, string table, string? graphQlName = null)
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

			return @this.GetMutation().AddField(fieldType);
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
		public FieldType AddSqlApiInsertDataEndpoint<T>(IDataSource dataSource, string table, string? graphQlName = null)
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

			return @this.GetMutation().AddField(fieldType);
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
		public FieldType AddSqlApiInsertEndpoint<T>(IDataSource dataSource, string table, string? graphQlName = null)
			where T : notnull, new()
		{
			dataSource.ThrowIfNull();
			table.ThrowIfBlank();

			var name = dataSource.Escape(table);
			var objectSchema = dataSource.ObjectSchemas[name];
			var graphOrderByEnum = new EnumerationGraphType
			{
				Name = Invariant($"{Type<T>.Attributes.GraphQLName ?? typeof(T).GraphQLName}OrderBy"),
			};
			foreach (var property in Type<T>.Properties.Values.Where(_ => !_.Attributes.GraphQLIgnore))
			{
				var propertyName = property.Attributes.GraphQLName ?? property.Name;
				var propertyDeprecationReason = property.Attributes.GraphQLDeprecationReason;

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

			return @this.GetMutation().AddField(fieldType);
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
		public FieldType AddSqlApiSelectEndpoint<T>(IDataSource dataSource, string table, string? graphQlName = null)
			where T : notnull, new()
		{
			dataSource.ThrowIfNull();
			table.ThrowIfBlank();

			var name = dataSource.Escape(table);
			var objectSchema = dataSource.ObjectSchemas[name];
			var graphOrderByEnum = new EnumerationGraphType
			{
				Name = Invariant($"{Type<T>.Attributes.GraphQLName ?? typeof(T).GraphQLName}OrderBy"),
			};
			foreach (var property in Type<T>.Properties.Values.Where(_ => !_.Attributes.GraphQLIgnore))
			{
				var propertyName = property.Attributes.GraphQLName ?? property.Name;
				var propertyDeprecationReason = property.Attributes.GraphQLDeprecationReason;

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

			return @this.GetQuery().AddField(fieldType);
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
		public FieldType AddSqlApiUpdateDataEndpoint<T>(IDataSource dataSource, string table, string? graphQlName = null)
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

			return @this.GetMutation().AddField(fieldType);
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
		public FieldType AddSqlApiUpdateEndpoint<T>(IDataSource dataSource, string table, string? graphQlName = null)
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

			return @this.GetMutation().AddField(fieldType);
		}
	}
}
