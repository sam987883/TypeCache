// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Primitives;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;
using static TypeCache.Data.DataSourceType;
using static TypeCache.Default;

namespace TypeCache.Data;

public sealed record ObjectSchema(IDataSource DataSource, ObjectType Type, DatabaseObject Name, string DatabaseName, string SchemaName, string ObjectName)
{
	public ObjectSchema(
		IDataSource dataSource
		, ObjectType type
		, DatabaseObject name
		, string databaseName
		, string schemaName
		, string objectName
		, IEnumerable<ColumnSchema> columns
		) : this(dataSource, type, name, databaseName, schemaName, objectName)
	{
		this.Columns = columns.ToImmutableArray();
	}

	public ObjectSchema(
		IDataSource dataSource
		, ObjectType type
		, DatabaseObject name
		, string databaseName
		, string schemaName
		, string objectName
		, IEnumerable<ParameterSchema> parameters
		) : this(dataSource, type, name, databaseName, schemaName, objectName)
	{
		this.Parameters = parameters.ToImmutableArray();
	}

	[DebuggerHidden]
	public IReadOnlyList<ColumnSchema> Columns { get; } = ImmutableArray<ColumnSchema>.Empty;

	[DebuggerHidden]
	public IReadOnlyList<ParameterSchema> Parameters { get; } = ImmutableArray<ParameterSchema>.Empty;

	public DataTable CreateDataTable()
	{
		var primaryKeys = this.Columns.If(column => column.PrimaryKey).Map(column => column.Name);
		var table = new DataTable(this.Name);
		this.Columns.Do(column => table.Columns.Add(new DataColumn(column.Name, column.DataTypeHandle.ToType())));
		table.PrimaryKey = table.Columns.If<DataColumn>().If(column => primaryKeys.Has(column.ColumnName)).ToArray();
		return table;
	}

	public string CreateCountSQL(string? distinctColumn, string where)
		=> new StringBuilder()
			.AppendLine(distinctColumn.IsNotBlank()
				? Invariant($"SELECT COUNT(DISTINCT {this.DataSource.EscapeIdentifier(distinctColumn)})")
				: "SELECT COUNT(*)")
			.AppendLine(this.DataSource.Type is SqlServer
				? Invariant($"FROM {this.Name} WITH(NOLOCK)")
				: Invariant($"FROM {this.Name}"))
			.AppendLineIf(where.IsNotBlank(), Invariant($"WHERE {where}"))
			.AppendStatementEndSQL()
			.ToString();

	public string CreateDeleteSQL(string where, params string[] output)
		=> new StringBuilder()
			.AppendLine(Invariant($"DELETE {this.Name}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLineIf(where.IsNotBlank(), Invariant($"WHERE {where}"))
			.AppendStatementEndSQL()
			.ToString();

	public string CreateDeleteSQL(DataTable data, params string[] output)
	{
		var columns = data.PrimaryKey.If<DataColumn>().Map(column => this.DataSource.EscapeIdentifier(column.ColumnName));

		return new StringBuilder()
			.AppendLine(Invariant($"DELETE {this.Name}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.Append(data.ToSQL())
			.AppendLine(Invariant($") AS data ({data.Columns.If<DataColumn>().Map(column => this.DataSource.EscapeIdentifier(column.ColumnName)).ToCSV()})"))
			.AppendLine(Invariant($"ON {columns.Each(column => Invariant($"data.{column} = _.{column}")).Join(" AND ")}"))
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateDeleteSQL(JsonArray data, StringValues output)
	{
		var columns = this.Columns.If(column => column.PrimaryKey).Map(column => this.DataSource.EscapeIdentifier(column.Name));

		return new StringBuilder()
			.AppendLine(Invariant($"DELETE {this.Name}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.Append(data.ToSQL())
			.AppendLine(Invariant($") AS data ({data[0]!.AsObject().Map(pair => this.DataSource.EscapeIdentifier(pair.Key)).ToCSV()})"))
			.AppendLine(Invariant($"ON {columns.Each(column => Invariant($"data.{column} = _.{column}")).Join(" AND ")}"))
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateDeleteSQL<T>(T[] data, StringValues output)
	{
		var primaryKeys = this.Columns
			.If(column => column.PrimaryKey)
			.Map(column => column.Name);
		var escapedPrimaryKeys = primaryKeys.Each(this.DataSource.EscapeIdentifier).ToArray();

		return new StringBuilder()
			.AppendLine(Invariant($"DELETE {this.Name}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.AppendValuesSQL(data, primaryKeys)
			.AppendLine(Invariant($") AS data ({escapedPrimaryKeys.ToCSV()})"))
			.AppendLine(Invariant($"ON {escapedPrimaryKeys.Each(column => Invariant($"data.{column} = _.{column}")).Join(" AND ")}"))
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateInsertSQL(string[] columns, SelectQuery selectQuery, params string[] output)
		=> new StringBuilder()
			.AppendLine(Invariant($"INSERT INTO {this.Name}"))
			.AppendLine(Invariant($"({columns.Map(this.DataSource.EscapeIdentifier).ToCSV()})"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append(this.CreateSelectSQL(selectQuery))
			.ToString();

	public string CreateInsertSQL(JsonArray data, params string[] output)
		=> new StringBuilder()
			.AppendLine(Invariant($"INSERT INTO {this.Name}"))
			.AppendLine(Invariant($"({data[0]!.AsObject().Map(pair => this.DataSource.EscapeIdentifier(pair.Key)).ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append(data.ToSQL())
			.AppendStatementEndSQL()
			.ToString();

	public string CreateInsertSQL(DataTable data, params string[] output)
		=> new StringBuilder()
			.AppendLine(Invariant($"INSERT INTO {this.Name}"))
			.AppendLine(Invariant($"({data.Columns.If<DataColumn>().Map(column => this.DataSource.EscapeIdentifier(column.ColumnName)).ToCSV()})"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.Append(data.ToSQL())
			.AppendStatementEndSQL()
			.ToString();

	public string CreateInsertSQL<T>(string[] columns, T[] data, params string[] output)
		=> new StringBuilder()
			.AppendLine(Invariant($"INSERT INTO {this.Name}"))
			.AppendLine(Invariant($"({columns.Each(this.DataSource.EscapeIdentifier).ToCSV()})"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendValuesSQL(data, columns)
			.AppendStatementEndSQL()
			.ToString();

	public string CreateSelectSQL(SelectQuery select)
	{
		var sqlBuilder = new StringBuilder("SELECT");

		if (this.DataSource.Type is PostgreSql && select.DistinctOn.IsNotBlank())
			sqlBuilder.Append(Invariant($" DISTINCT ON {select.DistinctOn}"));
		else
			sqlBuilder.AppendIf(select.Distinct, " DISTINCT");

		if (this.DataSource.Type is SqlServer && select.Top.IsNotBlank())
			sqlBuilder.Append(Invariant($" TOP {select.Top}"));

		return sqlBuilder
			.Append(' ')
			.AppendLine(select.Select.Any() ? select.Select.ToCSV() : "*")
			.AppendLine(Invariant($"FROM {select.From} {select.TableHints}"))
			.AppendLineIf(select.Where.IsNotBlank(), Invariant($"WHERE {select.Where}"))
			.AppendLineIf(select.GroupBy.Any(), select.GroupByOption switch
			{
				GroupBy.Cube => Invariant($"GROUP BY CUBE({select.GroupBy.ToCSV()})"),
				GroupBy.Rollup => Invariant($"GROUP BY ROLLUP({select.GroupBy.ToCSV()})"),
				_ => Invariant($"GROUP BY {select.GroupBy.ToCSV()}")
			})
			.AppendLineIf(select.Having.IsNotBlank(), Invariant($"HAVING {select.Having}"))
			.AppendLineIf(select.OrderBy.Any(), Invariant($"ORDER BY {select.OrderBy.ToCSV()}"))
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

	public string CreateUpdateSQL(string[] set, string where, params string[] output)
		=> new StringBuilder()
			.AppendLine(this.DataSource.Type is SqlServer
				? Invariant($"UPDATE {this.Name} WITH(UPDLOCK)")
				: Invariant($"UPDATE {this.Name}"))
			.AppendLine(Invariant($"SET {set.ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLineIf(where.IsNotBlank(), Invariant($"WHERE {where}"))
			.AppendStatementEndSQL()
			.ToString();

	public string CreateUpdateSQL(JsonArray data, params string[] output)
	{
		var primaryKeys = this.Columns
			.If(column => column.PrimaryKey)
			.Map(column => this.DataSource.EscapeIdentifier(column.Name));
		var setColumns = data[0]!.AsObject()
			.If(pair => this.Columns.Any(column => column.PrimaryKey && column.Name.Is(pair.Key)))
			.Map(pair => this.DataSource.EscapeIdentifier(pair.Key));
		var columns = data[0]!.AsObject()
			.Map(pair => this.DataSource.EscapeIdentifier(pair.Key));

		return new StringBuilder()
			.AppendLine(this.DataSource.Type is SqlServer
				? Invariant($"UPDATE {this.Name} WITH(UPDLOCK)")
				: Invariant($"UPDATE {this.Name}"))
			.AppendLine(Invariant($"SET {setColumns.Each(column => Invariant($"{column} = data.{column}")).ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.Append(Invariant($"VALUES {data.ToSQL()}"))
			.AppendLine(Invariant($") AS data ({columns.ToCSV()})"))
			.AppendLine(Invariant($"ON {primaryKeys.Each(column => Invariant($"data.{column} = _.{column}")).Join(" AND ")}"))
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateUpdateSQL(DataTable data, params string[] output)
	{
		var primaryKeys = data.PrimaryKey
			.Map(column => this.DataSource.EscapeIdentifier(column.ColumnName));
		var setColumns = data.Columns.If<DataColumn>()
			.If(dataColumn => this.Columns.Any(column => column.PrimaryKey && column.Name.Is(dataColumn.ColumnName)))
			.Map(dataColumn => this.DataSource.EscapeIdentifier(dataColumn.ColumnName));
		var columns = data.Columns.If<DataColumn>()
			.Map(dataColumn => this.DataSource.EscapeIdentifier(dataColumn.ColumnName));

		return new StringBuilder()
			.AppendLine(this.DataSource.Type is SqlServer
				? Invariant($"UPDATE {this.Name} WITH(UPDLOCK)")
				: Invariant($"UPDATE {this.Name}"))
			.AppendLine(Invariant($"SET {setColumns.Each(column => Invariant($"{column} = data.{column}")).ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.Append(data.ToSQL())
			.AppendLine(Invariant($") AS data ({columns.ToCSV()})"))
			.AppendLine(Invariant($"ON {primaryKeys.Each(column => Invariant($"data.{column} = _.{column}")).Join(" AND ")}"))
			.AppendStatementEndSQL()
			.ToString();
	}

	public string CreateUpdateSQL<T>(string[] columns, T[] data, params string[] output)
	{
		var primaryKeys = this.Columns
			.If(column => column.PrimaryKey)
			.Map(column => this.DataSource.EscapeIdentifier(column.Name));
		var escapedColumns = columns.Each(this.DataSource.EscapeIdentifier);

		return new StringBuilder()
			.AppendLine(this.DataSource.Type is SqlServer
				? Invariant($"UPDATE {this.Name} WITH(UPDLOCK)")
				: Invariant($"UPDATE {this.Name}"))
			.AppendLine(Invariant($"SET {escapedColumns.Each(column => Invariant($"{column} = data.{column}")).ToCSV()}"))
			.AppendOutputSQL(this.DataSource.Type, output)
			.AppendLine(Invariant($"FROM {this.Name} AS _"))
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.AppendValuesSQL(data, columns)
			.AppendLine(Invariant($") AS data ({escapedColumns.ToCSV()})"))
			.AppendLine(Invariant($"ON {primaryKeys.Each(column => Invariant($"data.{column} = _.{column}")).Join(" AND ")}"))
			.AppendStatementEndSQL()
			.ToString();
	}

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool HasColumn(string column) =>
		this.Columns.Map(_ => _.Name).Has(column);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool HasParameter(string parameter) =>
		this.Parameters.Map(_ => _.Name).Has(parameter);

	[DebuggerHidden]
	public bool Equals([NotNullWhen(true)] ObjectSchema? other)
		=> other?.DataSource.Equals(this.DataSource) is true
			&& other?.Name.Equals(this.Name) is true;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> HashCode.Combine(this.DataSource, this.Name);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override string ToString()
		=> this.Name;
}
