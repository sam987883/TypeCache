// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.Data.Extensions;

public static class StringBuilderExtensions
{
	public static StringBuilder AppendOutputSQL(this StringBuilder @this, DataSourceType dataSourceType, StringValues output)
		=> @this.AppendLineIf(output.Count is not 0, dataSourceType switch
		{
			SqlServer or PostgreSql => Invariant($"OUTPUT {output.ToCSV()}"),
			Oracle => Invariant($"RETURNING {output.ToCSV()}"),
			_ => string.Empty
		});

	public static StringBuilder AppendStatementEndSQL(this StringBuilder @this)
	{
		while (@this[@this.Length - 1] is '\r' || @this[@this.Length - 1] is '\n')
			@this.Remove(@this.Length - 1, 1);

		return @this.Append(';').AppendLine();
	}

	public static StringBuilder AppendValuesSQL<T>(this StringBuilder @this, T[] input, StringValues columns)
	{
		var properties = Type<T>.Properties;
		var propertyMap = columns
			.Where(properties.ContainsKey!)
			.Select(_ => properties[_!])
			.ToDictionaryIgnoreCase(_ => _.Name, _ => new Func<T, object?>(instance => _.GetValue(instance!)));

		@this.Append("VALUES ");
		input.ForEach(row => @this.Append('(').Append(columns.Select(column => propertyMap[column!].Invoke(row!).ToSQL()).ToCSV()).Append(')'),
			() => @this.AppendLine().Append("\t, "));

		return @this.AppendLine();
	}
}
