// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
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
	private const string collectionName = nameof(collectionName);
	private const string column_name = nameof(column_name);
	private const string database_name = nameof(database_name);
	private const string index_name = nameof(index_name);
	private const string is_nullable = nameof(is_nullable);
	private const string ordinal_position = nameof(ordinal_position);
	private const string parameter_mode = nameof(parameter_mode);
	private const string parameter_name = nameof(parameter_name);
	private const string routine_name = nameof(routine_name);
	private const string routine_schema = nameof(routine_schema);
	private const string routine_type = nameof(routine_type);
	private const string specific_name = nameof(specific_name);
	private const string specific_schema = nameof(specific_schema);
	private const string table_name = nameof(table_name);
	private const string table_schema = nameof(table_schema);
	private const string table_type = nameof(table_type);
	private const string type_desc = nameof(type_desc);

	public DataSource(string name, DbProviderFactory dbProviderFactory, string connectionString, DataSourceType type)
	{
		this.Factory = dbProviderFactory;
		this.Name = name;
		this.ConnectionString = connectionString;
		this.Type = type;

		using var connection = this.CreateDbConnection();
		connection.Open();
		this.DefaultDatabase = connection.Database;
		this.DefaultSchema = type switch
		{
			SqlServer => "dbo",
			PostgreSql => "public",
			_ => string.Empty
		};

		this.Databases = this.GetDatabases().ToImmutableArray();
		this.ObjectSchemas = this.GetObjectSchemas();
	}

	public ObjectSchema? this[string objectName]
		=> this.ObjectSchemas.TryGetValue(this.CreateName(objectName), out var objectSchema) ? objectSchema : null;

	public string ConnectionString { get; }

	public IReadOnlyList<string> Databases { get; }

	public string DefaultDatabase { get; }

	public string DefaultSchema { get; }

	public DbProviderFactory Factory { get; }

	public string Name { get; }

	public DataSourceType Type { get; }

	public IReadOnlyDictionary<DatabaseObject, ObjectSchema> ObjectSchemas { get; }

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public DbConnection CreateDbConnection()
		=> this.Factory.CreateConnection(this.ConnectionString);

	public DatabaseObject CreateName(string databaseObject)
	{
		databaseObject.AssertNotNull();

		var items = databaseObject.Split('.', RemoveEmptyEntries);
		if (items.Length > 3)
			throw new ArgumentException(Invariant($"{nameof(DataSource)}.{nameof(CreateName)}: Invalid name: {databaseObject}"), nameof(databaseObject));

		items = items.Select(item => this.Type switch
		{
			PostgreSql => item.Trim('"'),
			_ => item.TrimStart('[').TrimEnd(']')
		}).ToArray();

		return this.Type switch
		{
			SqlServer or PostgreSql => items.Length switch
			{
				1 => this.CreateName(this.DefaultSchema, items[0]),
				2 when databaseObject.Contains("..") => this.CreateName(items[0], this.DefaultSchema, items[1]),
				2 => this.CreateName(items[0], items[1]),
				_ => this.CreateName(items[0], items[1], items[2])
			},
			_ => new(databaseObject)
		};
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public DatabaseObject CreateName(string schema, string objectName)
		=> new(Invariant($"{this.EscapeIdentifier(this.DefaultDatabase)}.{this.EscapeIdentifier(schema)}.{this.EscapeIdentifier(objectName)}"));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public DatabaseObject CreateName(string database, string schema, string objectName)
		=> new(Invariant($"{this.EscapeIdentifier(database)}.{this.EscapeIdentifier(schema)}.{this.EscapeIdentifier(objectName)}"));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public SqlCommand CreateSqlCommand(string sql)
		=> new SqlCommand(this, sql);

	[DebuggerHidden]
	public string EscapeIdentifier([NotNull] string identifier)
		=> this.Type switch
		{
			PostgreSql => Invariant($"\"{identifier}\""),
			_ => Invariant($"[{identifier.Replace("]", "]]")}]")
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public string EscapeLikeValue([NotNull] string text)
		=> text.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public string EscapeValue([NotNull] string text)
		=> text.Replace("'", "''");

	public async ValueTask<DataSet> GetDatabaseSchemaAsync(string? database = null, CancellationToken token = default)
	{
		var schemaSet = new DataSet(SchemaCollection.MetaDataCollections.Name());

		await using var connection = this.CreateDbConnection();
		await connection.OpenAsync(token);

		if (database is not null)
			await connection.ChangeDatabaseAsync(database, token);

		var table = await connection.GetSchemaAsync(schemaSet.DataSetName, token);
		foreach (var row in table.Select())
			schemaSet.Tables.Add(await connection.GetSchemaAsync(row[collectionName].ToString()!, token));

		return schemaSet;
	}

	public async ValueTask<DataTable> GetDatabaseSchemaAsync(SchemaCollection collection, string? database = null, CancellationToken token = default)
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
			schemaSet.Tables.Add(connection.GetSchema(row[collectionName].ToString()!)));

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
	{
		var table = this.GetDatabaseSchema(SchemaCollection.Databases, null);
		var rows = this.Type switch
		{
			SqlServer => table?.Select(Invariant($"{database_name} NOT IN ('master', 'tempdb', 'model', 'msdb')")),
			_ => table?.Select()
		};

		return rows?.Select(row => row[database_name].ToString()!).ToArray() ?? Array<string>.Empty;
	}

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

			var tables = connection.GetSchema(SchemaCollection.Tables.Name());
			var tablesRows = tables.Select(Invariant($"{table_type} = 'BASE TABLE'"), Invariant($"{table_name} ASC"));
			tablesRows.ForEach(tablesRow =>
			{
				var tableName = tablesRow[table_name].ToString()!;
				var tableSchema = tablesRow[table_schema].ToString()!;
				var name = this.CreateName(databaseName, tableSchema, tableName);

				command.CommandText = Invariant($"SELECT TOP 1 * FROM {name};");
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

			var views = connection.GetSchema(SchemaCollection.Views.Name());
			var viewsRows = views?.Select(null, Invariant($"{table_name} ASC"));
			viewsRows?.ForEach(viewsRow =>
			{
				var tableName = viewsRow[table_name].ToString()!;
				var tableSchema = viewsRow[table_schema].ToString()!;
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

			var procedures = connection.GetSchema(SchemaCollection.Procedures.Name());
			var procedureParameters = connection.GetSchema(SchemaCollection.ProcedureParameters.Name());
			var proceduresRows = procedures?.Select(null, Invariant($"{routine_name} ASC"));
			proceduresRows?.ForEach(proceduresRow =>
			{
				var routineName = proceduresRow[routine_name].ToString()!;
				var routineSchema = proceduresRow[routine_schema].ToString()!;
				var routineType = proceduresRow[routine_type].ToString()!;

				var procedureParametersRows = procedureParameters?.Select(
					Invariant($"{specific_schema} = '{routineSchema}' AND {specific_name} = '{routineName}'")
					, Invariant($"{ordinal_position} ASC"));
				var parameters = procedureParametersRows?.Select(row => new ParameterSchema(row[parameter_name].ToString()!, row[parameter_mode].ToString() switch
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
		});

		return objectSchemas.ToImmutableDictionary();
	}
}
