// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using static System.FormattableString;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class CsvExtensions
{
	private static string EscapeCSV(this object? @this, CsvOptions options = default)
		=> @this switch
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
			Enum token => token.ToString(options.EnumFormatSpecifier),
			string text when text.Contains('"') => Invariant($"\"{text.Replace("\"", "\"\"")}\""),
			string text when text.Contains(',') || text.Contains('\r') || text.Contains('\n') => Invariant($"\"{text.Replace("\"", "\"\"")}\""),
			string text => text,
			_ => @this.ToString()!.EscapeCSV()
		};

	public static string[] ToCSV<T>(this T[] @this, CsvOptions options = default)
	{
		if (options.MemberNames.Any())
		{
			var propertyMap = TypeOf<T>.Properties.ToDictionary(_ => _.Name, _ => _, options.MemberNameComparison);
			var fieldMap = TypeOf<T>.Fields.ToDictionary(_ => _.Name, _ => _, options.MemberNameComparison);

			var headerRow = string.Join(',', options.MemberNames.If(name => propertyMap.ContainsKey(name) || fieldMap.ContainsKey(name)));
			var dataRows = @this.Map(row => string.Join(',', options.MemberNames.Map(name => propertyMap.TryGetValue(name, out var propertyMember)
				? propertyMember.GetValue(row).EscapeCSV(options) : fieldMap[name].GetValue(row).EscapeCSV(options))));

			return new[] { headerRow }.Append(dataRows).ToArray();
		}
		else if (TypeOf<T>.Properties.Any())
		{
			var headerRow = string.Join(',', TypeOf<T>.Properties.Map(property => property.Name));
			var dataRows = @this.Map(row => string.Join(',', TypeOf<T>.Properties.Map(property => property.GetValue(row).EscapeCSV(options))));

			return new[] { headerRow }.Append(dataRows).ToArray();
		}
		else if (TypeOf<T>.Fields.Any())
		{
			var headerRow = string.Join(',', TypeOf<T>.Fields.Map(field => field.Name));
			var dataRows = @this.Map(row => string.Join(',', TypeOf<T>.Fields.Map(field => field.GetValue(row).EscapeCSV(options))));

			return new[] { headerRow }.Append(dataRows).ToArray();
		}
		return Array<string>.Empty;
	}
}
