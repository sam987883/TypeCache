// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Extensions;

public static class StringBuilderExtensions
{
	public static StringBuilder AppendColumnsSQL(this StringBuilder @this, string[] columns)
		=> @this.Append('(')
			.AppendJoin(", ", columns.Map(column => column.EscapeIdentifier()))
			.Append(')').AppendLine();

	public static StringBuilder AppendSQL(this StringBuilder @this, ReadOnlySpan<char> keyword, string? clause)
		=> clause.IsNotBlank() ? @this.Append(keyword).Append(' ').AppendLine(clause) : @this;

	public static StringBuilder AppendSQL(this StringBuilder @this, ReadOnlySpan<char> keyword, ReadOnlySpan<char> clause1, ReadOnlySpan<char> clause2)
		=> @this.Append(keyword).Append(' ').Append(clause1).Append(' ').Append(clause2).AppendLine();

	public static StringBuilder AppendSQL(this StringBuilder @this, ReadOnlySpan<char> keyword, IEnumerable<string>? parts)
	{
		if (!parts.Any())
			return @this;

		var separator = new[] { '\t', ',', ' ' };
		@this.Append(keyword).Append(' ');
		parts.Do(part => @this.Append(part), () => @this.AppendLine().Append(separator.AsSpan()));
		return @this.AppendLine();
	}

	public static StringBuilder AppendInsertSQL(this StringBuilder @this, string into, string[] columns)
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

	public static StringBuilder AppendValuesSQL<T>(this StringBuilder @this, string[] columns, T[] input)
		=> @this.AppendSQL("VALUES", input switch
		{
			bool[] or sbyte[] or byte[] or short[] or ushort[] or int[] or uint[] or long[] or ulong[]
			or bool?[] or sbyte?[] or byte?[] or short?[] or ushort?[] or int?[] or uint?[] or long?[] or ulong?[]
			or float[] or Half[] or double[] or decimal[] or float?[] or Half?[] or double?[] or decimal?[]
			or DateOnly[] or DateTime[] or DateTimeOffset[] or DateOnly?[] or DateTime?[] or DateTimeOffset?[]
			or TimeOnly[] or TimeSpan[] or TimeOnly?[] or TimeSpan?[]
			or Guid[] or Guid?[] or string[] => input.Map(value => Invariant($"({value.ToSQL()})")),
			object[][] rows => rows.Map(row => Invariant($"({row.Map(value => value.ToSQL()).ToCSV()})")),
			IDictionary<string, object?>[] rows => rows.Map(row => Invariant($"({columns.Map(column => row[column].ToSQL()).ToCSV()})")),
			_ when input[0] is ITuple => ((ITuple[])(object)input).Map(row => Invariant($"({(0..row.Length).Map(i => row[i].ToSQL()).ToCSV()})")),
			_ => input.Map(row => Invariant($"({columns.Map(column => TypeOf<T>.Properties.If(_ => _.Name.Is(column)).First()!.GetValue(row!).ToSQL()).ToCSV()})"))
		});
}
