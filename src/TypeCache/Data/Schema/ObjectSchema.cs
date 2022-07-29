// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;
using static TypeCache.Default;

namespace TypeCache.Data.Schema;

public class ObjectSchema : IEquatable<ObjectSchema>
{
	public static string SQL { get; } = Invariant(@$"
SELECT o.[object_id] AS [Id]
, DB_NAME() AS [DatabaseName]
, o.[name] AS [ObjectName]
, CASE o.[type]
	WHEN 'U' THEN {ObjectType.Table.Number()}
	WHEN 'V' THEN {ObjectType.View.Number()}
	WHEN 'TF' THEN {ObjectType.Function.Number()}
	ELSE {ObjectType.StoredProcedure.Number()}
	END AS [Type]
, s.[name] AS [SchemaName]
FROM [sys].[objects] AS o
INNER JOIN [sys].[schemas] AS s ON s.[schema_id] = o.[schema_id]
WHERE o.[object_id] = @ObjectId AND o.[type] IN ('U', 'V', 'TF', 'P')
UNION ALL
SELECT tt.[type_table_object_id] AS [Id]
, DB_NAME() AS [DatabaseName]
, tt.[name] AS [ObjectName]
, {ObjectType.TableType.Number()} AS [Type]
, s.[name] AS [SchemaName]
FROM [sys].[table_types] AS tt
INNER JOIN [sys].[schemas] AS s ON s.[schema_id] = tt.[schema_id]
WHERE tt.[type_table_object_id] = @ObjectId;
");

	public ObjectSchema(string dataSource, ObjectSchemaModel objectSchema)
	{
		this.DataSource = dataSource;
		this.Id = objectSchema.Id;
		this.Type = objectSchema.Type;
		this.DatabaseName = objectSchema.DatabaseName;
		this.SchemaName = objectSchema.SchemaName;
		this.ObjectName = objectSchema.ObjectName;
		this.Name = Invariant($"[{objectSchema.DatabaseName}].[{objectSchema.SchemaName}].[{objectSchema.ObjectName}]");
		this.Columns = objectSchema.Columns is not null ? objectSchema.Columns.Map(_ => new ColumnSchema(_)).ToImmutableArray() : ImmutableArray<ColumnSchema>.Empty;
		this.Parameters = objectSchema.Parameters is not null ? objectSchema.Parameters.Map(_ => new ParameterSchema(_)).ToImmutableArray() : ImmutableArray<ParameterSchema>.Empty;
	}

	public int Id { get; }

	public ObjectType Type { get; }

	public string DataSource { get; }

	public string DatabaseName { get; }

	public string SchemaName { get; }

	public string ObjectName { get; }

	/// <summary>
	/// The fully qualified database object name.
	/// </summary>
	public string Name { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool HasColumn(string column) =>
		this.Columns.Map(_ => _.Name).Has(column);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool HasParameter(string parameter) =>
		this.Parameters.Map(_ => _.Name).Has(parameter);

	public IReadOnlyList<ColumnSchema> Columns { get; }

	public IReadOnlyList<ParameterSchema> Parameters { get; }

	public bool Equals(ObjectSchema? other)
		=> other is not null && this.DataSource.Is(other.DataSource) && this.Name.Is(other.Name);

	public override bool Equals([NotNullWhen(true)] object? item)
		=> item is not null && this.Equals((ObjectSchema)item);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> (this.DataSource, this.Name).GetHashCode();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override string ToString()
		=> this.Name;
}
