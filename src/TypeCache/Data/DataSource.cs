// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Data;
using System.Data.Common;
using TypeCache.Collections;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using static System.StringSplitOptions;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.Data;

internal sealed class DataSource : IDataSource
{
	public DataSource(string name, DbProviderFactory dbProviderFactory, string connectionString, string[] databases)
	{
		this.Factory = dbProviderFactory;
		this.Name = name;
		this.ConnectionString = connectionString;

		using var connection = this.CreateDbConnection();
		connection.Open();

		this.DefaultDatabase = connection.Database;
		this.Server = connection.DataSource;
		this.Version = connection.ServerVersion;

		var @namespace = dbProviderFactory.GetType().Namespace;
		this.Type = @namespace switch
		{
			_ when @namespace.Is("Microsoft.Data.SqlClient") || @namespace.Is("System.Data.SqlClient") => DataSourceType.SqlServer,
			_ when @namespace.Is("Oracle.DataAccess.Client") => DataSourceType.Oracle,
			_ when @namespace.Is("Npgsql") => DataSourceType.PostgreSql,
			_ when @namespace.Is("MySql.Data.MySqlClient") => DataSourceType.MySql,
			_ => DataSourceType.Unknown
		};

		if (this.Type == DataSourceType.PostgreSql)
			this.DefaultSchema = "public";
		else if (this.Type == DataSourceType.MySql)
			this.DefaultSchema = this.DefaultDatabase;
		else if (this.Type == DataSourceType.Unknown)
			this.DefaultSchema = string.Empty;
		else
		{
			var sql = this.Type switch
			{
				DataSourceType.SqlServer => "SELECT SCHEMA_NAME();",
				DataSourceType.Oracle => "SELECT sys_context('USERENV', 'CURRENT_SCHEMA') FROM dual",
				_ => string.Empty
			};

			using var command = connection.CreateCommand();
			command.Connection = connection;
			command.CommandType = CommandType.Text;
			command.CommandText = sql;

			this.DefaultSchema = command.ExecuteScalar()?.ToString() ?? string.Empty;
		}

		var metadata = connection.GetSchema(SchemaCollection.MetaDataCollections.Name());
		this.SupportedMetadataCollections = metadata.Rows
			.Cast<DataRow>()
			.Select(row => row[SchemaColumn.collectionName]!.ToString()!.ToEnum<SchemaCollection>()!.Value)
			.WhereNotNull()
			.ToFrozenSet();

		if (databases?.Length > 0)
			this.Databases = new HashSet<string>(databases, StringComparer.OrdinalIgnoreCase)
				.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
		else
			this.Databases = this.GetDatabases().ToFrozenSet(StringComparer.OrdinalIgnoreCase);

		this.ObjectSchemas = this.GetObjectSchemas();
	}

	public ObjectSchema? this[string objectName]
		=> this.ObjectSchemas.TryGetValue(this.CreateName(objectName), out var objectSchema) ? objectSchema : null;

	public string ConnectionString { get; }

	public IReadOnlySet<string> Databases { get; }

	public string DefaultDatabase { get; }

	public string DefaultSchema { get; }

	public DbProviderFactory Factory { get; }

	public string Name { get; }

	public IReadOnlyDictionary<DatabaseObject, ObjectSchema> ObjectSchemas { get; }

	public string Server { get; }

	public IReadOnlySet<SchemaCollection> SupportedMetadataCollections { get; }

	public string Version { get; }

	public DataSourceType Type { get; }

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public DbConnection CreateDbConnection()
		=> this.Factory.CreateConnection(this.ConnectionString);

	public DatabaseObject CreateName(string databaseObject)
	{
		databaseObject.AssertNotNull();

		var items = databaseObject.Split('.', RemoveEmptyEntries | TrimEntries);
		if (items.Length > 3 || (this.Type == MySql && items.Length > 2))
			throw new ArgumentOutOfRangeException(Invariant($"{nameof(DataSource)}.{nameof(CreateName)}: Invalid name: {databaseObject}"), nameof(databaseObject));

		items = items.Select(item => this.Type switch
		{
			SqlServer when item[0] == '[' => item.TrimStart('[').TrimEnd(']'),
			MySql => item.Trim('`').Replace("``", "`"),
			_ => item.Trim('"').Replace("\"\"", "\"")
		}).ToArray();

		return items.Length switch
		{
			1 => this.CreateName(this.DefaultSchema, items[0]),
			2 when databaseObject.Contains("..") => this.CreateName(items[0], this.DefaultSchema, items[1]),
			2 => this.CreateName(items[0], items[1]),
			_ => this.CreateName(items[0], items[1], items[2])
		};
	}

	public DatabaseObject CreateName(string schema, string objectName)
		=> this.Type switch
		{
			MySql => new(Invariant($"{this.EscapeIdentifier(schema)}.{this.EscapeIdentifier(objectName)}")),
			_ => new(Invariant($"{this.EscapeIdentifier(this.DefaultDatabase)}.{this.EscapeIdentifier(schema)}.{this.EscapeIdentifier(objectName)}"))
		};

	public DatabaseObject CreateName(string database, string schema, string objectName)
	{
		this.Databases.Contains(database).AssertTrue();

		return new(Invariant($"{this.EscapeIdentifier(database)}.{this.EscapeIdentifier(schema)}.{this.EscapeIdentifier(objectName)}"));
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public SqlCommand CreateSqlCommand(string sql)
		=> new(this, sql);

	[DebuggerHidden]
	public string EscapeIdentifier([NotNull] string identifier)
		=> this.Type switch
		{
			SqlServer => Invariant($"[{identifier.Replace("]", "]]")}]"),
			Oracle or PostgreSql => Invariant($"\"{identifier.Replace("\"", "\"\"")}\""),
			MySql => Invariant($"`{identifier.Replace("`", "``")}`"),
			_ => identifier
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public string EscapeLikeValue([NotNull] string text)
		=> text.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public string EscapeValue([NotNull] string text)
		=> text.Replace("'", "''");

	public async Task<DataSet> GetDatabaseSchemaAsync(string? database = null, CancellationToken token = default)
	{
		var schemaSet = new DataSet(SchemaCollection.MetaDataCollections.Name());

		await using var connection = this.CreateDbConnection();
		await connection.OpenAsync(token);

		if (database is not null)
			await connection.ChangeDatabaseAsync(database, token);

		var table = await connection.GetSchemaAsync(schemaSet.DataSetName, token);
		foreach (var row in table.Select())
			schemaSet.Tables.Add(await connection.GetSchemaAsync(row[SchemaColumn.collectionName].ToString()!, token));

		return schemaSet;
	}

	public async Task<DataTable> GetDatabaseSchemaAsync(SchemaCollection collection, string? database = null, CancellationToken token = default)
	{
		await using var connection = this.CreateDbConnection();
		await connection.OpenAsync(token);

		if (database is not null)
			await connection.ChangeDatabaseAsync(database, token);

		return await connection.GetSchemaAsync(collection.Name(), token);
	}

	public DataSet GetDatabaseSchema(string? database = null)
	{
		var schemaSet = new DataSet(SchemaCollection.MetaDataCollections.Name());

		using var connection = this.CreateDbConnection();
		connection.Open();

		if (database is not null)
			connection.ChangeDatabase(database);

		var table = connection.GetSchema(schemaSet.DataSetName);
		table.Select().ForEach(row =>
			schemaSet.Tables.Add(connection.GetSchema(row[SchemaColumn.collectionName].ToString()!)));

		return schemaSet;
	}

	public DataTable GetDatabaseSchema(SchemaCollection collection, string? database = null)
	{
		using var connection = this.CreateDbConnection();
		connection.Open();

		if (database is not null)
			connection.ChangeDatabaseAsync(database);

		return connection.GetSchema(collection.Name());
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals(IDataSource? other)
		=> this.Name.Is(other?.Name);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as DataSource);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> this.Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override string ToString()
		=> this.Name;

	private string[] GetDatabases()
		=> this.GetDatabaseSchema(SchemaCollection.Databases, null)?
			.Select()?
			.Select(row => row[SchemaColumn.database_name].ToString()!).ToArray() ?? Array<string>.Empty;

	private IReadOnlyDictionary<DatabaseObject, ObjectSchema> GetObjectSchemas()
	{
		var objectSchemas = new Dictionary<DatabaseObject, ObjectSchema>();

		using var connection = this.CreateDbConnection();
		connection.Open();

		using var command = connection.CreateCommand();
		command.Connection = connection;
		command.CommandType = CommandType.Text;

		using var adapter = this.Factory.CreateDataAdapter()!;
		adapter.SelectCommand = command;
		adapter.MissingMappingAction = MissingMappingAction.Passthrough;
		adapter.MissingSchemaAction = MissingSchemaAction.Add;

		this.Databases.ToArray().ForEach(databaseName =>
		{
			connection.ChangeDatabase(databaseName);

			if (this.SupportedMetadataCollections.Contains(SchemaCollection.Tables))
			{
				var tables = connection.GetSchema(SchemaCollection.Tables.Name());
				var tablesRows = tables.Select(Invariant($"{SchemaColumn.table_type} = 'BASE TABLE'"), Invariant($"{SchemaColumn.table_name} ASC"));
				tablesRows.ForEach(tablesRow =>
				{
					var tableName = tablesRow[SchemaColumn.table_name].ToString()!;
					var tableSchema = tablesRow[SchemaColumn.table_schema].ToString()!;
					var name = this.CreateName(databaseName, tableSchema, tableName);

					command.CommandText = Invariant($"SELECT * FROM {name} WHERE 0 = 1;");
					var table = new DataTable();

					try
					{
						adapter.FillSchema(table, SchemaType.Source);

						var columns = table.Columns
							.OfType<DataColumn>()
							.Select(column => new ColumnSchema(
								column.ColumnName, column.AllowDBNull, table.PrimaryKey.Contains(column), column.ReadOnly, column.Unique, column.DataType.TypeHandle));

						var objectSchema = new ObjectSchema(this, DatabaseObjectType.Table, name, databaseName, tableSchema, tableName, columns);
						objectSchemas.Add(name, objectSchema);
					}
					catch (Exception) { }
				});
			}

			if (this.SupportedMetadataCollections.Contains(SchemaCollection.Views))
			{
				var views = connection.GetSchema(SchemaCollection.Views.Name());
				var viewsRows = views?.Select(null, Invariant($"{SchemaColumn.table_name} ASC"));
				viewsRows?.ForEach(viewsRow =>
				{
					var tableName = viewsRow[SchemaColumn.table_name].ToString()!;
					var tableSchema = viewsRow[SchemaColumn.table_schema].ToString()!;
					var name = this.CreateName(databaseName, tableSchema, tableName);

					command.CommandText = Invariant($"SELECT * FROM {name} WHERE 0 = 1;");
					var table = new DataTable();

					try
					{
						adapter.FillSchema(table, SchemaType.Source);

						var columns = table.Columns
							.OfType<DataColumn>()
							.Select(column => new ColumnSchema(
								column.ColumnName, column.AllowDBNull, table.PrimaryKey.Contains(column), column.ReadOnly, column.Unique, column.DataType.TypeHandle));

						var objectSchema = new ObjectSchema(this, DatabaseObjectType.View, name, databaseName, tableSchema, tableName, columns);
						objectSchemas.Add(name, objectSchema);
					}
					catch (Exception) { }
				});
			}

			if (this.SupportedMetadataCollections.Contains(SchemaCollection.Procedures))
			{
				var procedures = connection.GetSchema(SchemaCollection.Procedures.Name());
				var procedureParameters = connection.GetSchema(SchemaCollection.ProcedureParameters.Name());
				var proceduresRows = procedures?.Select(null, Invariant($"{SchemaColumn.routine_name} ASC"));
				proceduresRows?.ForEach(proceduresRow =>
				{
					var routineName = proceduresRow[SchemaColumn.routine_name].ToString()!;
					var routineSchema = proceduresRow[SchemaColumn.routine_schema].ToString()!;
					var routineType = proceduresRow[SchemaColumn.routine_type].ToString()!;

					var procedureParametersRows = procedureParameters?.Select(
						Invariant($"{SchemaColumn.specific_schema} = '{routineSchema}' AND {SchemaColumn.specific_name} = '{routineName}'")
						, Invariant($"{SchemaColumn.ordinal_position} ASC"));
					var parameters = procedureParametersRows?.Select(row => new ParameterSchema(row[SchemaColumn.parameter_name].ToString()!, row[SchemaColumn.parameter_mode].ToString() switch
					{
						string value when value.Is("OUT") => ParameterDirection.Output,
						string value when value.Is("INOUT") => ParameterDirection.InputOutput,
						_ => ParameterDirection.Input
					}));

					var name = this.CreateName(databaseName, routineSchema, routineName);
					var objectType = routineType switch
					{
						_ when routineType.Is("FUNCTION") => DatabaseObjectType.Function,
						_ => DatabaseObjectType.StoredProcedure
					};
					var objectSchema = new ObjectSchema(this, objectType, name, databaseName, routineSchema, routineName, parameters ?? Array<ParameterSchema>.Empty);
					objectSchemas.Add(name, objectSchema);
				});
			}
		});

		return objectSchemas.ToFrozenDictionary();
	}
}
