// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Extensions;

public static class StringBuilderExtensions
{
	public static StringBuilder AppendColumnsSQL(this StringBuilder @this, string[] columns)
		=> @this.Append('(')
			.AppendJoin(", ", columns.To(column => column.EscapeIdentifier()))
			.Append(')').AppendLine();

	public static StringBuilder AppendSQL(this StringBuilder @this, string keyword, string? clause)
		=> clause.IsNotBlank() ? @this.Append(keyword).Append(' ').AppendLine(clause) : @this;

	public static StringBuilder AppendSQL(this StringBuilder @this, string keyword, string clause1, string clause2)
		=> @this.Append(keyword).Append(' ').Append(clause1).Append(' ').AppendLine(clause2);

	public static StringBuilder AppendSQL(this StringBuilder @this, string keyword, string clause1, string clause2, string clause3)
		=> @this.Append(keyword).Append(' ').Append(clause1).Append(' ').AppendLine(clause2).Append(' ').AppendLine(clause3);

	public static StringBuilder AppendSQL(this StringBuilder @this, string keyword, IEnumerable<string>? parts)
	{
		if (!parts.Any())
			return @this;

		var separator = new[] { '\t', ',', ' ' };
		@this.Append(keyword).Append(' ');
		parts.Do(part => @this.Append(part), () => @this.AppendLine().Append(separator.AsSpan()));
		return @this.AppendLine();
	}

	public static StringBuilder AppendInsertSQL(this StringBuilder @this, string into, params string[] columns)
	{
		into.AssertNotBlank();
		columns.AssertNotEmpty();

		return @this.AppendSQL("INSERT INTO", into).AppendColumnsSQL(columns);
	}

	public static StringBuilder AppendPagerSQL(this StringBuilder @this, Pager? pager)
	{
		if (pager is null)
			return @this;

		@this.AppendSQL("OFFSET", Invariant($"{pager.Value.After} ROWS"));
		return pager.Value.First > 0 ? @this.AppendSQL("FETCH NEXT", Invariant($"{pager.Value.First} ROWS ONLY")) : @this;
	}


	public static StringBuilder AppendStatementEndSQL(this StringBuilder @this)
	{
		while (@this[@this.Length - 1] == '\r' || @this[@this.Length - 1] == '\n')
			@this.Remove(@this.Length - 1, 1);

		return @this.Append(';').AppendLine();
	}

	public static StringBuilder AppendValuesSQL(this StringBuilder @this, object?[][] rows)
		=> @this.AppendSQL("VALUES", rows.To(row => Invariant($"({row.To(value => value.ToSQL()).Join(", ")})")));
}
