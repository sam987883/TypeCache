// Copyright (c) 2021 Samuel Abraham

using System.Text;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Domain;

public class DeleteDataCommand<T> : Command
{
	/// <summary>
	/// Primary Key values of the records to delete.
	/// </summary>
	public T[] Input { get; set; } = Array<T>.Empty;

	/// <summary>
	/// JSON: <c>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" ]</c><br/>
	/// SQL: <c>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</c>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;

	internal string[] PrimaryKeys { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <c>"From": "Table1"</c><br/>
	/// SQL: <c>DELETE FROM [Database1]..[Table1]</c>
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <inheritdoc/>
	public override string ToSQL()
		=> new StringBuilder()
			.AppendSQL("DELETE", "FROM", "_")
			.AppendSQL("OUTPUT", this.Output)
			.AppendSQL("FROM", this.Table, "_")
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.AppendValuesSQL(this.PrimaryKeys, this.Input)
			.Append(')').Append(" AS pk ").AppendColumnsSQL(this.PrimaryKeys)
			.AppendSQL("ON", this.PrimaryKeys.Each(column => Invariant($"pk.{column.EscapeIdentifier()} = _.{column.EscapeIdentifier()}")).Join(" AND "))
			.AppendStatementEndSQL()
			.ToString();
}
