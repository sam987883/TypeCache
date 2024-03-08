// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text;
using Microsoft.Extensions.Primitives;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.Data.Extensions;

public static class StringBuilderExtensions
{
	public static StringBuilder AppendOutputSQL(this StringBuilder @this, DataSourceType dataSourceType, StringValues output)
		=> @this.AppendLine(dataSourceType switch
		{
			SqlServer or PostgreSql => Invariant($"OUTPUT {output.ToCSV()}"),
			Oracle => Invariant($"RETURNING {output.ToCSV()}"),
			_ => string.Empty
		});

	public static StringBuilder AppendStatementEndSQL(this StringBuilder @this)
	{
		while (@this[@this.Length - 1] == '\r' || @this[@this.Length - 1] == '\n')
			@this.Remove(@this.Length - 1, 1);

		return @this.Append(';').AppendLine();
	}

	public static StringBuilder AppendValuesSQL<T>(this StringBuilder @this, T[] input, StringValues columns)
	{ 
		var values = input.Select(row => Invariant($"({columns.Select<string, string>(column =>
			(typeof(T).GetProperties(Instance | Public).Where(_ => _.Name().Is(column)).First()?.GetPropertyValue(row!)).ToSQL()).ToCSV()})")).ToArray();
		@this.Append("VALUES ");
		values.ForEach(row => @this.Append(row), () => @this.AppendLine().Append("\t, "));
		return @this.AppendLine();
	}
}
