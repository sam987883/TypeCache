// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Converters;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	public sealed record ColumnSchema(int Id, string Name, SqlDbType Type, bool Nullable, bool ReadOnly, bool Hidden, bool Identity, bool PrimaryKey, int Length) : IEquatable<ColumnSchema>
	{
		public bool Equals(ColumnSchema? other)
			=> this.Id == other?.Id && this.Name.Is(other.Name) && this.Type == other.Type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> HashCode.Combine(this.Id, this.Name, this.Type);
	}

	/// <summary>
	/// JSON: <code>{ "Column": "N'Expression 1'" }</code>
	/// SQL: <code>[Column] = N'Expression 1'</code>
	/// </summary>
	[JsonConverter(typeof(ColumnSetJsonConverter))]
	public sealed record ColumnSet(string Column, object? Expression);

	/// <summary>
	/// JSON: <code>{ "Ascending": "SQL Expression" }</code>
	/// SQL: <code>[Column1] ASC</code>
	/// </summary>
	[JsonConverter(typeof(ColumnSortJsonConverter))]
	public sealed record ColumnSort(string Expression, Sort Sort);

	[JsonConverter(typeof(ExpressionJsonConverter))]
	public sealed record ComparisonExpression(string Field, ComparisonOperator Operator, object? Value);

	[JsonConverter(typeof(ExpressionSetJsonConverter))]
	public sealed record ExpressionSet(LogicalOperator Operator, ExpressionSet[] ExpressionSets, params ComparisonExpression[] Expressions);

	public sealed record ObjectSchema(int Id, ObjectType Type, string DatabaseName, string SchemaName, string ObjectName,
		IImmutableList<ColumnSchema> Columns, IImmutableList<ParameterSchema> Parameters) : IEquatable<ObjectSchema>
	{
		/// <summary>
		/// The fully qualified database object name.
		/// </summary>
		public string Name { get; init; } = $"[{DatabaseName}].[{SchemaName}].[{ObjectName}]";

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

	/// <summary>
	/// JSON: <code>
	/// {<br />
	///		"Deleted": { "Columns": [ "Column1", "Column2", "Column3", ... ], "Rows": [ [ "Data", 123, null, ... ], [ ... ], ... ] },<br />
	///		"Inserted": { "Columns": [ "Column1", "Column2", "Column3", ... ], "Rows": [ [ "Data", 123, null, ... ], [ ... ], ... ] }<br />
	/// }
	/// </code>
	/// SQL: <code>
	/// OUTPUT DELETED.[Column1], DELETED.[Column2], DELETED.[Column3] ...<br />
	/// OUTPUT INSERTED.[Column1], INSERTED.[Column2], INSERTED.[Column3] ...
	/// </code>
	/// </summary>
	public sealed record Output(RowSet Deleted, RowSet Inserted);

	/// <summary>
	/// JSON: <code>{ "Alias 1": "SQL Expression", "Alias 2": "ColumnName" }</code>
	/// SQL: <code>NULLIF([Column1], 22) AS [Alias 1]</code>
	/// </summary>
	[JsonConverter(typeof(OutputExpressionJsonConverter))]
	public sealed record OutputExpression(string Expression, string As);

	/// <summary>
	/// JSON: <code>{ "ParameterName": "ParameterValue" }</code>
	/// SQL: <code>SET @ParameterName = N'ParameterValue';</code>
	/// </summary>
	[JsonConverter(typeof(ParameterJsonConverter))]
	public sealed record Parameter(string Name, object Value);

	public sealed record ParameterSchema(int Id, string Name, SqlDbType Type, bool Output, bool Return);

	/// <summary>
	/// <code>
	/// {<br />
	///		"Columns": [ "Column1", "Column2", "Column3", ... ],<br />
	///		"Rows": [ [ "Data", 123, null ], [ ... ], ... ]<br />
	/// }
	/// </code>
	/// </summary>
	public sealed record RowSet(string[] Columns, object?[][] Rows)
	{
		public static RowSet Empty { get; } = new RowSet(Array.Empty<string>(), Array.Empty<object[]>());

		public object? this[int row, string column] => this.Rows[row][this.Columns.ToIndex(column).FirstValue()!.Value];
	}

	/// <summary>
	/// Use Parameters to take in user input to avoid SQL Injection.
	/// </summary>
	public sealed record SqlRequest(string SQL, params Parameter[]? Parameters);

	/// <summary>
	/// JSON: <code>{ "Procedure": "Procedure1", "Parameters": [ { "Parameter1": "Value1" }, { "Parameter2": null }, { "Parameter3": true } ] }</code>
	/// SQL: <code>EXECUTE [Database1]..[Procedure1] (@Parameter1 = N'Value1', @Parameter2 = NULL, @Parameter3 = 1);</code>
	/// </summary>
	public sealed record StoredProcedureRequest(string Procedure, params Parameter[]? Parameters);

	/// <summary>
	/// JSON: <code>
	/// {<br />
	///		"Parameters": [ ... ],<br />
	///		"Output":<br />
	///		{<br />
	///			"Columns": [ "Column1", "Column2", "Column3", ... ],<br />
	///			"Rows": [ [ "Data", 123, null ], [ ... ], ... ]<br />
	///		}<br />
	///	}
	/// </code>
	/// </summary>
	public sealed record StoredProcedureResponse(IEnumerable<RowSet> Output, params Parameter[]? Parameters);
}
