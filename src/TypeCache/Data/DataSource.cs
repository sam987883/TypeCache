// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Data;
using System.Data.Common;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.Utilities;
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

		var factoryName = dbProviderFactory.GetType().FullName!;
		this.Type = factoryName switch
		{
			_ when factoryName.ContainsIgnoreCase("Oracle") => Oracle,
			_ when factoryName.ContainsIgnoreCase("Npgsql") || factoryName.ContainsIgnoreCase("Postgre") => PostgreSql,
			_ when factoryName.ContainsIgnoreCase("MySql") => MySql,
			_ when factoryName.ContainsIgnoreCase("SqlClient") => SqlServer,
			_ => Unknown
		};

		if (this.Type is PostgreSql)
			this.DefaultSchema = "public";
		else if (this.Type is MySql)
			this.DefaultSchema = this.DefaultDatabase;
		else if (this.Type is Unknown)
			this.DefaultSchema = string.Empty;
		else
		{
			var sql = this.Type switch
			{
				SqlServer => "SELECT SCHEMA_NAME();",
				Oracle => "SELECT sys_context('USERENV', 'CURRENT_SCHEMA') FROM dual",
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
		=> this.ObjectSchemas.TryGetValue(this.Escape(objectName), out var objectSchema) ? objectSchema : null;

	public string ConnectionString { get; }

	public IReadOnlySet<string> Databases { get; }

	public string DefaultDatabase { get; }

	public string DefaultSchema { get; }

	public DbProviderFactory Factory { get; }

	public string Name { get; }

	public IReadOnlyDictionary<string, ObjectSchema> ObjectSchemas { get; }

	public string Server { get; }

	public IReadOnlySet<SchemaCollection> SupportedMetadataCollections { get; }

	public string Version { get; }

	public DataSourceType Type { get; }

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public DbConnection CreateDbConnection()
		=> this.Factory.CreateConnection(this.ConnectionString);

	public string Escape(string databaseObject)
	{
		databaseObject.ThrowIfBlank();

		var items = databaseObject.Split('.', RemoveEmptyEntries | TrimEntries);
		if (items.Length > 3 || (this.Type == MySql && items.Length > 2))
			throw new ArgumentOutOfRangeException(Invariant($"{nameof(DataSource)}.{nameof(Escape)}: Invalid name: {databaseObject}"), nameof(databaseObject));

		items = (items.Length, this.Type) switch
		{
			(1, MySql) => [this.DefaultSchema, items[0]],
			(1, _) => [this.DefaultDatabase, this.DefaultSchema, items[0]],
			(2, _) when databaseObject.Contains("..") => [items[0], this.DefaultSchema, items[1]],
			(2, _) => [this.DefaultDatabase, ..items],
			_ => items
		};

		return '.'.Join(items.Select(item => item.UnEscapeIdentifier(this.Type).EscapeIdentifier(this.Type)));
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public SqlCommand CreateSqlCommand(string sql)
		=> new(this, sql);

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
		=> this.Name.EqualsIgnoreCase(other?.Name);

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

	private IReadOnlyDictionary<string, ObjectSchema> GetObjectSchemas()
	{
		var objectSchemas = new Dictionary<string, ObjectSchema>();

		using var connection = this.CreateDbConnection();
		connection.Open();

		using var command = connection.CreateCommand();
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

					command.CommandText = Invariant($"SELECT * FROM {tableSchema.EscapeIdentifier(this.Type)}.{tableName.EscapeIdentifier(this.Type)} WHERE 0 = 1;");
					var table = new DataTable();

					try
					{
						adapter.FillSchema(table, SchemaType.Source);

						var columns = table.Columns
							.OfType<DataColumn>()
							.Select(column => new ColumnSchema(
								column.ColumnName, column.AllowDBNull, table.PrimaryKey.Contains(column), column.ReadOnly, column.Unique, column.DataType.TypeHandle));

						var objectSchema = new ObjectSchema(this, DatabaseObjectType.Table, databaseName, tableSchema, tableName, columns, null);
						objectSchemas.Add(objectSchema.Name, objectSchema);
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

					command.CommandText = Invariant($"SELECT * FROM {tableSchema.EscapeIdentifier(this.Type)}.{tableName.EscapeIdentifier(this.Type)} WHERE 0 = 1;");
					var table = new DataTable();

					try
					{
						adapter.FillSchema(table, SchemaType.Source);

						var columns = table.Columns
							.OfType<DataColumn>()
							.Select(column => new ColumnSchema(
								column.ColumnName, column.AllowDBNull, table.PrimaryKey.Contains(column), column.ReadOnly, column.Unique, column.DataType.TypeHandle));

						var objectSchema = new ObjectSchema(this, DatabaseObjectType.View, databaseName, tableSchema, tableName, columns, null);
						objectSchemas.Add(objectSchema.Name, objectSchema);
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
						string value when value.EqualsIgnoreCase("OUT") => ParameterDirection.Output,
						string value when value.EqualsIgnoreCase("INOUT") => ParameterDirection.InputOutput,
						_ => ParameterDirection.Input
					}));

					var objectType = routineType switch
					{
						_ when routineType.EqualsIgnoreCase("FUNCTION") => DatabaseObjectType.Function,
						_ => DatabaseObjectType.StoredProcedure
					};
					var objectSchema = new ObjectSchema(this, objectType, databaseName, routineSchema, routineName, null, parameters);
					objectSchemas.Add(objectSchema.Name, objectSchema);
				});
			}
		});

		return objectSchemas.ToFrozenDictionary();
	}
}
