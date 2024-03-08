// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;

namespace TypeCache.Data;

public interface IDataSource : IEquatable<IDataSource>
{
	string ConnectionString { get; }

	IReadOnlySet<string> Databases { get; }

	string DefaultDatabase { get; }

	public string DefaultSchema { get; }

	DbProviderFactory Factory { get; }

	string Name { get; }

	IReadOnlyDictionary<DatabaseObject, ObjectSchema> ObjectSchemas { get; }

	public string Server { get; }

	IReadOnlySet<SchemaCollection> SupportedMetadataCollections { get; }

	DataSourceType Type { get; }

	public string Version { get; }

	ObjectSchema? this[string objectName] { get; }

	DbConnection CreateDbConnection();

	/// <exception cref="ArgumentOutOfRangeException"/>
	DatabaseObject CreateName(string databaseObject);

	/// <summary>
	/// <c>=&gt; <see langword="new"/>(<see cref="Invariant"/>($"{<see langword="this"/>.EscapeIdentifier(<see langword="this"/>.DefaultDatabase)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="schema"/>)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="objectName"/>)}"));</c>
	/// </summary>
	DatabaseObject CreateName(string schema, string objectName);

	/// <summary>
	/// <c>=&gt; <see langword="new"/>(<see cref="Invariant"/>($"{<see langword="this"/>.EscapeIdentifier(<paramref name="database"/>)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="schema"/>)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="objectName"/>)}"));</c>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	DatabaseObject CreateName(string database, string schema, string objectName);

	SqlCommand CreateSqlCommand(string sql);

	Task<DataSet> GetDatabaseSchemaAsync(string? database = null, CancellationToken token = default);

	Task<DataTable> GetDatabaseSchemaAsync(SchemaCollection collection, string? database = null, CancellationToken token = default);

	DataSet GetDatabaseSchema(string? database = null);

	DataTable GetDatabaseSchema(SchemaCollection collection, string? database = null);
}
