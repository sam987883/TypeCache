// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Domain;

/// <summary>
/// SQL: <code>TRUNCATE TABLE [Database1]..[Table1];</code>
/// </summary>
public class TruncateTableCommand : Command
{
	/// <summary>
	/// The table name.
	/// </summary>
	public string Table { get; set; } = string.Empty;

	/// <inheritdoc/>
	public override string ToSQL()
		=> Invariant($"TRUNCATE TABLE {this.Table.EscapeIdentifier()};");
}
