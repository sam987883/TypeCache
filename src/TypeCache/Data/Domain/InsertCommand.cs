// Copyright (c) 2021 Samuel Abraham

using System.Text;
using System.Threading;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Domain;

/// <summary>
/// JSON: <c>{ "Into": ..., "Insert": [...]. "Output": [ ... ], ... }</c><br/>
/// SQL: <c>INSERT INTO ... (...) OUPUT ... SELECT ...;</c>
/// </summary>
public class InsertCommand : SelectCommand
{
	/// <summary>
	/// JSON: <c>"Columns": [ "Column1", "Column2", "Column3" ]</c><br/>
	/// SQL: <c>INSERT INTO ... ([Column1], Column2, [Column 3], ...)</c>
	/// </summary>
	public string[] Columns { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <c>"Output": [ "NULLIF([Column1], 22) AS [Alias 1]", "INSERTED.ColumnName [Alias 2]", "DELETED.ColumnName" ]</c><br/>
	/// SQL: <c>OUTPUT NULLIF([Column1], 22) AS [Alias 1], INSERTED.[ColumnName] [Alias 2], DELETED.[ColumnName]</c>
	/// </summary>
	public string[] Output { get; set; } = Array<string>.Empty;

	/// <summary>
	/// JSON: <c>"Table": "Table"</c><br/>
	/// SQL: <c>INSERT INTO [Database]..[Table]</c>
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <inheritdoc/>
	public override string ToSQL()
	{
		var sqlBuilder = new StringBuilder();
		sqlBuilder.AppendInsertSQL(this.Table, this.Columns)
			.AppendSQL("OUTPUT", this.Output)
			.Append("SELECT");

		if (this.Distinct)
			sqlBuilder.Append(" DISTINCT");

		if (this.Top > 0)
		{
			sqlBuilder.Append(Invariant($" TOP ({this.Top})"));
			if (this.Percent)
				sqlBuilder.Append(" PERCENT");
			if (this.WithTies)
				sqlBuilder.Append(" WITH TIES");
		}

		return sqlBuilder
			.AppendSQL(string.Empty, this.Select.Any() ? this.Select : new[] { "*" })
			.AppendSQL("FROM", this.From, this.TableHints ?? "WITH(NOLOCK)")
			.AppendSQL("WHERE", this.Where)
			.AppendSQL("GROUP BY", this.GroupBy)
			.AppendSQL("HAVING", this.Having)
			.AppendSQL("ORDER BY", this.OrderBy)
			.AppendPagerSQL(this.Pager)
			.AppendStatementEndSQL()
			.ToString();
	}
}
