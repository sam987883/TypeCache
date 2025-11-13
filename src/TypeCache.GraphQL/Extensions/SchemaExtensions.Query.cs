// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static partial class SchemaExtensions
{
	extension(ISchema @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.Query ??= <see langword="new"/> <see cref="ObjectGraphType"/> { Name = <see langword="nameof"/>(<see cref="ISchema.Query"/>) };</c>
		/// </remarks>
		public IObjectGraphType GetQuery()
			=> @this.Query ??= new ObjectGraphType { Name = nameof(ISchema.Query) };

		/// <summary>
		/// Adds a GraphQL endpoint that returns the version of the GraphQL schema.
		/// </summary>
		/// <param name="version">The version of this GraphQL schema</param>
		public FieldType AddVersion(string version)
		{
			var field = @this.AddQuery("Version", () => version);
			field.Description = Invariant($"The version of this GraphQL Schema: {version}.");
			return field;
		}

		/// <summary>
		/// Adds all of the queries for all of the database schema data made available by the data provider.
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public FieldType[] AddDatabaseSchemaQueries(IDataSource dataSource)
		{
			var table = dataSource.GetDatabaseSchema(SchemaCollection.MetaDataCollections);
			var rows = table.Rows.OfType<DataRow>();

			return rows.Select(row => @this.AddDatabaseSchemaQuery(dataSource, row[SchemaColumn.collectionName].ToString().ToEnum<SchemaCollection>()!.Value)).ToArray();
		}

		/// <summary>
		/// Adds a query for the specified collection of database schema data made available by the data provider.
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public FieldType AddDatabaseSchemaQuery(IDataSource dataSource, SchemaCollection collection)
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

			var fieldType = @this.GetQuery().AddField(new()
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
		public FieldType[] AddQueries<T>(string method)
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
		public FieldType AddQuery<R>(string name, Func<R> handler)
			=> @this.GetQuery().AddField(new()
			{
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler()),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddQuery<T, R>(string name, string argument, Func<T, R> handler)
			=> @this.GetQuery().AddField(new()
			{
				Arguments = new(new QueryArgument(typeof(T).ToGraphQLType(true).ToNonNullGraphType()) { Name = argument }),
				Name = name,
				Resolver = new FuncFieldResolver<R>(context => handler(context.GetArgument<T>(argument))),
				Type = typeof(R).ToGraphQLType(false)
			});

		/// <summary>
		/// <b>GraphQL Minimal API.</b>
		/// </summary>
		public FieldType AddQuery<T1, T2, R>(string name, (string, string) arguments, Func<T1, T2, R> handler)
			=> @this.GetQuery().AddField(new()
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
		public FieldType AddQuery<T1, T2, T3, R>(string name, (string, string, string) arguments, Func<T1, T2, T3, R> handler)
			=> @this.GetQuery().AddField(new()
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
		public FieldType AddQuery<T1, T2, T3, T4, R>(string name, (string, string, string, string) arguments, Func<T1, T2, T3, T4, R> handler)
			=> @this.GetQuery().AddField(new()
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
		public FieldType AddQuery<T1, T2, T3, T4, T5, R>(string name, (string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, R> handler)
			=> @this.GetQuery().AddField(new()
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
		public FieldType AddQuery<T1, T2, T3, T4, T5, T6, R>(string name, (string, string, string, string, string, string) arguments, Func<T1, T2, T3, T4, T5, T6, R> handler)
			=> @this.GetQuery().AddField(new()
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
		public FieldType AddQuery(MethodEntity method)
		{
			if (!method.HasReturnValue)
				throw new ArgumentException($"{nameof(AddQuery)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

			return @this.GetQuery().AddField(method);
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
		public FieldType AddQuery(StaticMethodEntity method)
		{
			if (!method.HasReturnValue)
				throw new ArgumentException($"{nameof(AddQuery)}: GraphQL endpoints cannot have a return type that is void, {nameof(Task)} or {nameof(ValueTask)}.");

			return @this.GetQuery().AddField(method);
		}
	}
}
