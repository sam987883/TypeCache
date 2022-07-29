// Copyright (c) 2021 Samuel Abraham

using System.Text;
using System.Text.Json.Nodes;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Domain;

public class UpdateDataCommand<T> : Command
{
	/// <summary>
	/// JSON: <c>"Columns": { "[Column1] = ISNULL([Column2], N'')", "Column2 = 'aaa'", "[Column 3] = NULL" }</c><br/>
	/// SQL: <c>SET [Column1] = ISNULL([Column2], N''), Column2 = 'aaa', [Column 3] = NULL</c>
	/// </summary>
	public string[] Columns { get; set; } = Array<string>.Empty;

	/// <summary>
	/// Batch of records to update based on Primary Key(s).
	/// </summary>
	public T[] Input { get; set; } = Array<T>.Empty;

	/// <summary>
	/// JSON: <code>On: [ "ID1", "[ID2]" ]</code>
	/// SQL: <code>ON i.[ID1] = x.[ID1] AND i.[ID2] = x.[ID2]</code>
	/// </summary>
	public string[] On { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <c>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" ]</c><br/>
	/// SQL: <c>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</c>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <code>"Table1"</code>
	/// SQL: <code>UPDATE [Database1]..[Table1]</code>
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <summary>
	/// JSON: <code>"WITH(UPDLOCK)"</code>
	/// SQL: <code>UPDATE [Database1]..[Table1] WITH(UPDLOCK)</code>
	/// </summary>
	public string TableHints { get; set; } = "WITH(UPDLOCK)";

	public override string ToSQL()
		=> new StringBuilder()
			.AppendSQL("UPDATE", "_", this.TableHints ?? "WITH(UPDLOCK)")
			.AppendSQL("SET", this.Columns.Without(this.On).Each(column => Invariant($"{column.EscapeIdentifier()} = data.{column.EscapeIdentifier()}")))
			.AppendSQL("OUTPUT", this.Output)
			.AppendSQL("FROM", this.Table, "_")
			.AppendLine("INNER JOIN")
			.Append('(').AppendLine()
			.AppendValuesSQL(this.Columns, this.Input)
			.Append(") AS data (").AppendJoin(", ", this.Columns.Map(column => column.EscapeIdentifier())).Append(')').AppendLine()
			.AppendSQL("ON", this.On.Each(column => Invariant($"data.{column.EscapeIdentifier()} = _.{column.EscapeIdentifier()}")).Join(" AND "))
			.AppendStatementEndSQL()
			.ToString();
}
