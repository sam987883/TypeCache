// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using TypeCache.Utilities;

namespace TypeCache.Data;

public interface IDataSource : IName, IEquatable<IDataSource>
{
	ObjectSchema? this[string objectName] { get; }

	string ConnectionString { get; }

	IReadOnlyList<string> Databases { get; }

	string DefaultDatabase { get; }

	string DefaultSchema { get; }

	DbProviderFactory Factory { get; }

	DataSourceType Type { get; }

	IReadOnlyDictionary<DatabaseObject, ObjectSchema> ObjectSchemas { get; }

	DbConnection CreateDbConnection();

	DatabaseObject CreateName(string databaseObject);

	/// <summary>
	/// <c>=&gt; <see langword="new"/>(<see cref="Invariant"/>($"{<see langword="this"/>.EscapeIdentifier(<see langword="this"/>.DefaultDatabase)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="schema"/>)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="objectName"/>)}"));</c>
	/// </summary>
	DatabaseObject CreateName(string schema, string objectName);

	/// <summary>
	/// <c>=&gt; <see langword="new"/>(<see cref="Invariant"/>($"{<see langword="this"/>.EscapeIdentifier(<paramref name="database"/>)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="schema"/>)}.{<see langword="this"/>.EscapeIdentifier(<paramref name="objectName"/>)}"));</c>
	/// </summary>
	DatabaseObject CreateName(string database, string schema, string objectName);

	SqlCommand CreateSqlCommand(string sql);

	string EscapeIdentifier([NotNull] string identifier);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.EscapeValue(<paramref name="text"/>).Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");</c>
	/// </summary>
	string EscapeLikeValue([NotNull] string text);

	/// <summary>
	/// <c>=&gt; <paramref name="text"/>.Replace("'", "''");</c>
	/// </summary>
	string EscapeValue([NotNull] string text);

	ValueTask<DataSet> GetDatabaseSchemaAsync(string? database = null, CancellationToken token = default);

	ValueTask<DataTable> GetDatabaseSchemaAsync(SchemaCollection collection, string? database = null, CancellationToken token = default);

	DataSet GetDatabaseSchema(string? database = null);

	DataTable GetDatabaseSchema(SchemaCollection collection, string? database = null);
}
