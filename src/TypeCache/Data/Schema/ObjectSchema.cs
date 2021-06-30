// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Schema
{
	public sealed class ObjectSchema : IEquatable<ObjectSchema>
	{
		public const string OBJECT_NAME = "ObjectName";
		public const string SCHEMA_NAME = "SchemaName";

		static ObjectSchema()
		{
			Cache = new LazyDictionary<string, ConcurrentDictionary<string, ObjectSchema>>(server =>
				new(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
		}

		internal static IReadOnlyDictionary<string, ConcurrentDictionary<string, ObjectSchema>> Cache { get; }

		public static string SQL { get; } = @$"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

DECLARE @ObjectId AS INTEGER =
(
	SELECT o.[object_id]
	FROM [sys].[objects] AS o
	INNER JOIN [sys].[schemas] AS s ON s.[schema_id] = o.[schema_id]
	WHERE o.[name] = @{OBJECT_NAME}
		AND s.[name] = ISNULL(@{SCHEMA_NAME}, SCHEMA_NAME())
		AND o.[type] IN ('U', 'V', 'TF', 'P')
);
DECLARE @SqlDbTypes AS TABLE
(
	[ID] INTEGER NOT NULL PRIMARY KEY
    , [Type] NVARCHAR(200) NOT NULL UNIQUE
);

{InsertSqlDbTypes()}

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
WHERE o.[object_id] = @ObjectId;

{ColumnSchema.SQL}

{ParameterSchema.SQL}
";

		private static string InsertSqlDbTypes()
		{
			var sql = new StringBuilder()
				.AppendLine("INSERT INTO @SqlDbTypes ([ID], [Type])")
				.Append("VALUES ");

			EnumOf<SqlDbType>.Tokens.Values.Do(token => sql.AppendLine($"({token.Number}, '{token.Name}')"), () => sql.AppendLine().Append(", "));

			return sql.AppendLine(";").ToString();
		}

		public ObjectSchema(RowSet rowSet, ColumnSchema[] columns, ParameterSchema[] parameters)
		{
			this.Id = (int)rowSet[0, nameof(ObjectSchema.Id)]!;
			this.Type = (ObjectType)rowSet[0, nameof(ObjectSchema.Type)]!;
			this.DatabaseName = rowSet[0, nameof(ObjectSchema.DatabaseName)]!.ToString()!;
			this.SchemaName = rowSet[0, nameof(ObjectSchema.SchemaName)]!.ToString()!;
			this.ObjectName = rowSet[0, nameof(ObjectSchema.ObjectName)]!.ToString()!;
			this.Name = $"[{this.DatabaseName}].[{this.SchemaName}].[{this.ObjectName}]";
			this.Columns = columns.ToImmutableArray();
			this.Parameters = parameters.ToImmutableArray();
		}

		public int Id { get; init; }

		public ObjectType Type { get; init; }

		public string DatabaseName { get; init; }

		public string SchemaName { get; init; }

		public string ObjectName { get; init; }

		public IImmutableList<ColumnSchema> Columns { get; init; }

		public IImmutableList<ParameterSchema> Parameters { get; init; }

		/// <summary>
		/// The fully qualified database object name.
		/// </summary>
		public string Name { get; init; }

		public bool HasColumn(string column) =>
			this.Columns.To(_ => _.Name).Has(column);

		public bool HasParameter(string parameter) =>
			this.Parameters.To(_ => _.Name).Has(parameter);

		public bool Equals(ObjectSchema? other)
			=> this.Id == other?.Id && this.Name.Is(other.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> HashCode.Combine(this.Id, this.Name);
	}
}
