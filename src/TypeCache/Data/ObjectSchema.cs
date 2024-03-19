// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Data;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Primitives;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.Data;

public sealed class ObjectSchema(
	IDataSource dataSource
		, DatabaseObjectType type
		, string databaseName
		, string schemaName
		, string objectName
		, IEnumerable<ColumnSchema>? columns
		, IEnumerable<ParameterSchema>? parameters
		) : IEquatable<ObjectSchema>
{
	public IDataSource DataSource { get; } = dataSource;

	public DatabaseObjectType Type { get; } = type;

	public string Name { get; } = dataSource.Type switch
	{
		MySql => Invariant($"{schemaName.EscapeIdentifier(dataSource.Type)}.{objectName.EscapeIdentifier(dataSource.Type)}"),
		_ => Invariant($"{databaseName.EscapeIdentifier(dataSource.Type)}.{schemaName.EscapeIdentifier(dataSource.Type)}.{objectName.EscapeIdentifier(dataSource.Type)}")
	};

	public string DatabaseName { get; } = databaseName;

	public string SchemaName { get; } = schemaName;

	public string ObjectName { get; } = objectName;

	public IReadOnlySet<ColumnSchema> Columns { get; } = columns?.ToFrozenSet() ?? FrozenSet<ColumnSchema>.Empty;

	public IReadOnlySet<ParameterSchema> Parameters { get; } = parameters?.ToFrozenSet() ?? FrozenSet<ParameterSchema>.Empty;

	public DataTable CreateDataTable()
	{
		var primaryKeys = this.Columns.Where(column => column.PrimaryKey).Select(column => column.Name).ToArray();
		var table = new DataTable(this.Name);
		var columns = this.Columns.Select(column => new DataColumn(column.Name, column.DataTypeHandle.ToType())).ToArray();
		table.Columns.AddRange(columns);
		table.PrimaryKey = columns.Where(column => primaryKeys.Contains(column.ColumnName, StringComparer.OrdinalIgnoreCase)).ToArray();
		return table;
	}

	public string CreateCountSQL(string? distinctColumn, string where)
		=> new StringBuilder("SELECT ")
			.AppendLineIf(distinctColumn.IsBlank(), "COUNT(*)", Invariant($"COUNT(DISTINCT {(distinctColumn.IsNotBlank() ? distinctColumn?.EscapeIdentifier(this.DataSource.Type!) : null)})"))
			.Append("FROM ").Append(this.Name).AppendIf(this.DataSource.Type is SqlServer, " WITH(NOLOCK)").AppendLine()
			.AppendLineIf(where.IsNotBlank(), Invariant($"WHERE {where}"))
			.AppendStatementEndSQL()
			.ToString();

	public string CreateDeleteSQL(string where, string[] output)
		=> new StringBuilder("DELETE ").AppendLine(this.Name)
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLineIf(where.IsNotBlank(), Invariant($"WHERE {where}"))
			.AppendStatementEndSQL()
			.ToString();

	/// <exception cref="ArgumentOutOfRangeException"/>
	public string CreateDeleteSQL(DataTable data, string[] output)
	{
		var primaryKeys = this.Columns.Where(column => column.PrimaryKey).ToArray();

		(primaryKeys.Length > 0).AssertTrue();

		if (primaryKeys.Length == 1)
		{
			var values = data.Rows.Cast<DataRow>().Select(row => row[primaryKeys[0].Name].ToSQL()).ToCSV();
			var column = primaryKeys[0].Name.EscapeIdentifier(this.DataSource.Type);
			return new StringBuilder("DELETE FROM ").AppendLine(this.Name)
				.AppendOutputSQL(this.DataSource.Type, output)
				.Append("WHERE ").Append(column).Append(" IN (").Append(values).Append(')')
				.AppendStatementEndSQL()
				.ToString();
		}

		var conditions = data.Rows.Cast<DataRow>().Select(row => Invariant($"({string.Join(" AND ", primaryKeys.Select(column =>
			Invariant($"{column.Name.EscapeIdentifier(this.DataSource.Type)} = {row[column.Name].ToSQL()}")))})"));

		return new StringBuilder("DELETE FROM ").AppendLine(this.Name)
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append("WHERE ").AppendJoin(" OR ", conditions)
			.AppendStatementEndSQL()
			.ToString();
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public string CreateDeleteSQL(JsonArray data, StringValues output)
	{
		var primaryKeys = this.Columns.Where(column => column.PrimaryKey).ToArray();

		(primaryKeys.Length > 0).AssertTrue();

		if (primaryKeys.Length == 1)
		{
			var values = data.Select(row => row![primaryKeys[0].Name].ToSQL()).ToCSV();
			var column = primaryKeys[0].Name.EscapeIdentifier(this.DataSource.Type);
			return new StringBuilder("DELETE FROM ").AppendLine(this.Name)
				.AppendOutputSQL(this.DataSource.Type, output)
				.Append("WHERE ").Append(column).Append(" IN (").Append(values).Append(')')
				.AppendStatementEndSQL()
				.ToString();
		}

		var conditions = data.Select(row => Invariant($"({string.Join(" AND ", primaryKeys.Select(column =>
			Invariant($"{column.Name.EscapeIdentifier(this.DataSource.Type)} = {row![column.Name].ToSQL()}")))})"));

		return new StringBuilder("DELETE FROM ").AppendLine(this.Name)
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append("WHERE ").AppendJoin(" OR ", conditions)
			.AppendStatementEndSQL()
			.ToString();
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public string CreateDeleteSQL<T>(T[] data, StringValues output)
	{
		var primaryKeys = this.Columns.Where(column => column.PrimaryKey).ToArray();

		(primaryKeys.Length > 0).AssertTrue();

		var type = typeof(T);

		if (primaryKeys.Length == 1)
		{
			var values = data.Select(row => type.GetPropertyValue(primaryKeys[0].Name, row!).ToSQL()).ToCSV();
			var column = primaryKeys[0].Name.EscapeIdentifier(this.DataSource.Type);
			return new StringBuilder("DELETE ").AppendLine(this.Name)
				.AppendOutputSQL(this.DataSource.Type, output)
				.Append("WHERE ").Append(column).Append(" IN (").Append(values).Append(')')
				.AppendStatementEndSQL()
				.ToString();
		}

		var conditions = data.Select(row => Invariant($"({string.Join(" AND ", primaryKeys.Select(column =>
			Invariant($"{column.Name.EscapeIdentifier(this.DataSource.Type)} = {type.GetPropertyValue(column.Name, row!).ToSQL()}")))})"));

		return new StringBuilder("DELETE ").AppendLine(this.Name)
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append("WHERE ").AppendJoin(" OR ", conditions)
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateInsertSQL(string[] columns, SelectQuery selectQuery, string[] output)
		=> new StringBuilder("INSERT INTO ").AppendLine(this.Name)
			.AppendLine(Invariant($"({columns.Select(column => column.EscapeIdentifier(this.DataSource.Type)).ToCSV()})"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append(this.CreateSelectSQL(selectQuery))
			.ToString();

	public string CreateInsertSQL(JsonArray data, string[] output)
		=> new StringBuilder("INSERT INTO ").AppendLine(this.Name)
			.AppendLine(Invariant($"({data[0]!.AsObject().Select(pair => pair.Key.EscapeIdentifier(this.DataSource.Type)).ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append(data.ToSQL())
			.AppendStatementEndSQL()
			.ToString();

	public string CreateInsertSQL(DataTable data, string[] output)
		=> new StringBuilder("INSERT INTO ").AppendLine(this.Name)
			.AppendLine(Invariant($"({data.Columns.OfType<DataColumn>().Select(column => column.ColumnName.EscapeIdentifier(this.DataSource.Type)).ToCSV()})"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append(data.ToSQL())
			.AppendStatementEndSQL()
			.ToString();

	public string CreateInsertSQL<T>(string[] columns, T[] data, string[] output)
		=> new StringBuilder("INSERT INTO ").AppendLine(this.Name)
			.AppendLine(Invariant($"({columns.Select(column => column.EscapeIdentifier(this.DataSource.Type)).ToCSV()})"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendValuesSQL(data, columns)
			.AppendStatementEndSQL()
			.ToString();

	public string CreateSelectSQL(SelectQuery select)
	{
		var sqlBuilder = new StringBuilder("SELECT ");

		switch (this.DataSource.Type)
		{
			case SqlServer:
				sqlBuilder
					.AppendIf(select.Distinct, "DISTINCT ")
					.AppendIf(select.Top.HasValue, Invariant($"TOP ({select.Top}) "))
					.AppendIf(select.Top.HasValue && select.TopPercent, "PERCENT ");
				break;
			case Oracle:
			case MySql:
				sqlBuilder
					.AppendIf(select.TableHints.IsNotBlank(), Invariant($"/*+ {select.TableHints} */ "))
					.AppendIf(select.Distinct, "DISTINCT ");
				break;
			case PostgreSql:
				if (select.DistinctOn.IsNotBlank())
					sqlBuilder.Append("DISTINCT ON ").Append(select.DistinctOn);
				else
					sqlBuilder.AppendIf(select.Distinct, "DISTINCT ");

				break;
		}

		return sqlBuilder
			.AppendLineIf(select.Select?.Any() is true, select.Select.ToCSV(), "*")
			.Append("FROM ").Append(select.From)
			.AppendIf(select.TableHints.IsNotBlank(), _ => _
				.AppendIf(this.DataSource.Type is Oracle, Invariant($" /*+ {select.TableHints} */"))
				.AppendIf(this.DataSource.Type is SqlServer, Invariant($" WITH({select.TableHints})"))
				.AppendIf(this.DataSource.Type is PostgreSql, Invariant($" WITH({select.TableHints})")))
			.AppendLine()
			.AppendLineIf(select.Where.IsNotBlank(), Invariant($"WHERE {select.Where}"))
			.AppendLineIf(select.GroupBy?.Any() is true, select.GroupByOption switch
			{
				GroupBy.Cube when this.DataSource.Type is not MySql => Invariant($"GROUP BY CUBE({select.GroupBy.ToCSV()})"),
				GroupBy.Rollup => Invariant($"GROUP BY ROLLUP({select.GroupBy.ToCSV()})"),
				_ => Invariant($"GROUP BY {select.GroupBy.ToCSV()}")
			})
			.AppendLineIf(select.Having.IsNotBlank(), Invariant($"HAVING {select.Having}"))
			.AppendLineIf(select.OrderBy?.Any() is true, Invariant($"ORDER BY {select.OrderBy.ToCSV()}"))
			.AppendLineIf(select.Offset > 0, Invariant($"OFFSET {select.Offset} ROWS"))
			.AppendLineIf(select.Fetch > 0, Invariant($"FETCH NEXT {select.Fetch} ROWS ONLY"))
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateTruncateSQL(string partitions)
		=> this.DataSource.Type switch
		{
			PostgreSql => Invariant($"TRUNCATE {this.Name};"),
			_ when partitions.IsNotBlank() => Invariant($"TRUNCATE TABLE {this.Name} WITH (PARTITIONS ({partitions}));"),
			_ => Invariant($"TRUNCATE TABLE {this.Name};")
		};

	public string CreateUpdateSQL(string[] set, string where, string[] output)
		=> new StringBuilder("UPDATE ").Append(this.Name)
			.AppendIf(this.DataSource.Type is SqlServer, " WITH(UPDLOCK)").AppendLine()
			.AppendLine(Invariant($"SET {set.ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLineIf(where.IsNotBlank(), Invariant($"WHERE {where}"))
			.AppendStatementEndSQL()
			.ToString();

	public string CreateUpdateSQL(JsonArray data, string[] output)
	{
		var primaryKeys = this.Columns
			.Where(column => column.PrimaryKey)
			.Select(column => column.Name.EscapeIdentifier(this.DataSource.Type));
		var setColumns = data[0]!.AsObject()
			.Where(pair => this.Columns.Any(column => column.PrimaryKey && column.Name.Is(pair.Key)))
			.Select(pair => pair.Key.EscapeIdentifier(this.DataSource.Type));
		var columns = data[0]!.AsObject()
			.Select(pair => pair.Key.EscapeIdentifier(this.DataSource.Type));

		return new StringBuilder("UPDATE ").Append(this.Name)
			.AppendIf(this.DataSource.Type is SqlServer, " WITH(UPDLOCK)").AppendLine()
			.AppendLine(Invariant($"SET {setColumns.Select(column => Invariant($"{column} = data.{column}")).ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.Append(Invariant($"VALUES {data.ToSQL()}"))
			.AppendLine(Invariant($") AS data ({columns.ToCSV()})"))
			.Append("ON ").AppendJoin(" AND ", primaryKeys.Select(column => Invariant($"data.{column} = _.{column}"))).AppendLine()
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateUpdateSQL(DataTable data, string[] output)
	{
		var primaryKeys = data.PrimaryKey.Select(column => column.ColumnName.EscapeIdentifier(this.DataSource.Type));
		var setColumns = data.Columns.OfType<DataColumn>()
			.Where(dataColumn => this.Columns.Any(column => column.PrimaryKey && column.Name.Is(dataColumn.ColumnName)))
			.Select(dataColumn => dataColumn.ColumnName.EscapeIdentifier(this.DataSource.Type));
		var columns = data.Columns.OfType<DataColumn>()
			.Select(dataColumn => dataColumn.ColumnName.EscapeIdentifier(this.DataSource.Type));

		return new StringBuilder("UPDATE ").Append(this.Name)
			.AppendIf(this.DataSource.Type is SqlServer, " WITH(UPDLOCK)").AppendLine()
			.Append("SET ").AppendLine(setColumns.Select(column => Invariant($"{column} = data.{column}")).ToCSV())
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append("FROM ").Append(this.Name).AppendLine(" AS _")
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.Append(data.ToSQL())
			.Append(") AS data (").Append(columns.ToCSV()).Append(')').AppendLine()
			.Append("ON ").AppendJoin(" AND ", primaryKeys.Select(column => Invariant($"data.{column} = _.{column}")))
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateUpdateSQL<T>(string[] columns, T[] data, string[] output)
	{
		var primaryKeys = this.Columns
			.Where(column => column.PrimaryKey)
			.Select(column => column.Name.EscapeIdentifier(this.DataSource.Type));
		var escapedColumns = columns.Select(column => column.EscapeIdentifier(this.DataSource.Type));

		return new StringBuilder("UPDATE ").Append(this.Name)
			.AppendIf(this.DataSource.Type is SqlServer, " WITH(UPDLOCK)").AppendLine()
			.AppendLine(Invariant($"SET {escapedColumns.Select(column => Invariant($"{column} = data.{column}")).ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.AppendValuesSQL(data, columns)
			.AppendLine(Invariant($") AS data ({escapedColumns.ToCSV()})"))
			.Append("ON ").AppendJoin(" AND ", primaryKeys.Select(column => Invariant($"data.{column} = _.{column}"))).AppendLine()
			.AppendStatementEndSQL()
			.ToString();
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool HasColumn(string column) =>
		this.Columns.Any(_ => _.Name.Is(column));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool HasParameter(string parameter) =>
		this.Parameters.Any(_ => _.Name.Is(parameter));

	[DebuggerHidden]
	public bool Equals([NotNullWhen(true)] ObjectSchema? other)
		=> other?.DataSource.Equals(this.DataSource) is true
			&& other?.Name.Equals(this.Name) is true;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> HashCode.Combine(this.DataSource.Name, this.Name);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override string ToString()
		=> this.Name;
}
