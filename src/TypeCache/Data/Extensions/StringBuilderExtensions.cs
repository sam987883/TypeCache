﻿// Copyright (c) 2021 Samuel Abraham

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
		var propertyInfos = typeof(T).GetProperties(Instance | Public);
		var propertyMap = propertyInfos
			.Where(_ => columns.ContainsIgnoreCase(_.Name))
			.ToDictionary(_ => _.Name, _ => _.GetValueFunc(), StringComparer.OrdinalIgnoreCase);
		var values = input.Select(row => Invariant($"({columns.Select(column =>
			propertyMap[column!].Invoke(row!, null).ToSQL()).ToCSV()})")).ToArray();

		@this.Append("VALUES ");
		values.ForEach(row => @this.Append(row), () => @this.AppendLine().Append("\t, "));

		return @this.AppendLine();
	}
}
