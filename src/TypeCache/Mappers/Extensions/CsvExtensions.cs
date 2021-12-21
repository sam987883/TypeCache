// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using static System.FormattableString;

namespace TypeCache.Mappers.Extensions;

public static class CsvExtensions
{
	private static string EscapeValue(string value)
		=> value switch
		{
			_ when value.Contains('"') => Invariant($"\"{value.Replace("\"", "\"\"")}\""),
			_ when value.Contains(',') => Invariant($"\"{value}\""),
			_ => value
		};

	public static string[] ToCSV(this object?[][] @this, CsvOptions options = default)
		=> @this.To(row => row.To(value => value switch
		{
			null => options.NullText,
			true => options.TrueText,
			false => options.FalseText,
			sbyte number => number.ToString(options.ByteFormatSpecifier),
			byte number => number.ToString(options.ByteFormatSpecifier),
			short number => number.ToString(options.IntegerFormatSpecifier),
			int number => number.ToString(options.IntegerFormatSpecifier),
			nint number => number.ToString(options.IntegerFormatSpecifier),
			long number => number.ToString(options.IntegerFormatSpecifier),
			ushort number => number.ToString(options.IntegerFormatSpecifier),
			uint number => number.ToString(options.IntegerFormatSpecifier),
			nuint number => number.ToString(options.IntegerFormatSpecifier),
			ulong number => number.ToString(options.IntegerFormatSpecifier),
			float number => number.ToString(options.DecimalFormatSpecifier),
			double number => number.ToString(options.DecimalFormatSpecifier),
			Half number => number.ToString(options.DecimalFormatSpecifier),
			decimal number => number.ToString(options.DecimalFormatSpecifier),
			',' => "\",\"",
			'"' => "\"\"\"\"",
			DateOnly date => date.ToString(options.DateOnlyFormatSpecifier),
			DateTime dateTime => dateTime.ToString(options.DateTimeFormatSpecifier),
			DateTimeOffset dateTimeOffset => dateTimeOffset.ToString(options.DateTimeOffsetFormatSpecifier),
			TimeOnly time => time.ToString(options.TimeOnlyFormatSpecifier),
			TimeSpan time => time.ToString(options.TimeSpanFormatSpecifier),
			Guid guid => guid.ToString(options.GuidFormatSpecifier),
			Enum token => token.ToString(options.EnumFormatSpecifier),
			string text => EscapeValue(text),
			_ => EscapeValue(value.ToString() ?? string.Empty)
		}).Join(',')).ToArray();
}
