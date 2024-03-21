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

	IReadOnlyDictionary<string, ObjectSchema> ObjectSchemas { get; }

	public string Server { get; }

	IReadOnlySet<SchemaCollection> SupportedMetadataCollections { get; }

	DataSourceType Type { get; }

	public string Version { get; }

	ObjectSchema? this[string objectName] { get; }

	DbConnection CreateDbConnection();

	/// <exception cref="ArgumentOutOfRangeException"/>
	string Escape(string databaseObject);

	SqlCommand CreateSqlCommand(string sql);

	Task<DataSet> GetDatabaseSchemaAsync(string? database = null, CancellationToken token = default);

	Task<DataTable> GetDatabaseSchemaAsync(SchemaCollection collection, string? database = null, CancellationToken token = default);

	DataSet GetDatabaseSchema(string? database = null);

	DataTable GetDatabaseSchema(SchemaCollection collection, string? database = null);
}
