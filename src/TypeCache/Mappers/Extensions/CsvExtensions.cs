// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using static System.FormattableString;
using static System.Globalization.CultureInfo;

namespace TypeCache.Mappers.Extensions;

public static class CsvExtensions
{
	private static string EscapeCSV(this string @this)
		=> @this switch
		{
			null => string.Empty,
			_ when @this.Contains('"') => Invariant($"\"{@this.Replace("\"", "\"\"")}\""),
			_ when @this.Contains(',') || @this.Contains('\r') || @this.Contains('\n') => Invariant($"\"{@this}\""),
			_ => @this
		};

	public static string[] ToCSV(this object?[][] @this, CsvOptions options = default)
		=> @this.Map(row => row.Map(value => value switch
		{
			null => options.NullText,
			true => options.TrueText,
			false => options.FalseText,
			sbyte number => number.ToString(options.ByteFormatSpecifier, InvariantCulture),
			byte number => number.ToString(options.ByteFormatSpecifier, InvariantCulture),
			short number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			int number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			nint number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			long number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			ushort number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			uint number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			nuint number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			ulong number => number.ToString(options.IntegerFormatSpecifier, InvariantCulture),
			float number => number.ToString(options.DecimalFormatSpecifier, InvariantCulture),
			double number => number.ToString(options.DecimalFormatSpecifier, InvariantCulture),
			Half number => number.ToString(options.DecimalFormatSpecifier, InvariantCulture),
			decimal number => number.ToString(options.DecimalFormatSpecifier, InvariantCulture),
			',' => "\",\"",
			'"' => "\"\"\"\"",
			DateOnly date => date.ToString(options.DateOnlyFormatSpecifier, InvariantCulture),
			DateTime dateTime => dateTime.ToString(options.DateTimeFormatSpecifier, InvariantCulture),
			DateTimeOffset dateTimeOffset => dateTimeOffset.ToString(options.DateTimeOffsetFormatSpecifier, InvariantCulture),
			TimeOnly time => time.ToString(options.TimeOnlyFormatSpecifier, InvariantCulture),
			TimeSpan time => time.ToString(options.TimeSpanFormatSpecifier, InvariantCulture),
			Guid guid => guid.ToString(options.GuidFormatSpecifier, InvariantCulture),
			Enum token => token.ToString(options.EnumFormatSpecifier, InvariantCulture),
			string text => text.EscapeCSV(),
			_ => value.ToString()!.EscapeCSV()
		}).Join(',')).ToArray();
}
