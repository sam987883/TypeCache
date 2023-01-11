// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class CsvExtensions
{
	private static string EscapeCSV(this object? @this, CsvOptions options = default)
		=> @this switch
		{
			null => options.NullText,
			bool value => value ? options.TrueText : options.FalseText,
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
			',' => Invariant($"\",\""),
			'"' => Invariant($"\"\"\""),
			char character => character.ToString(),
			DateOnly date => date.ToString(options.DateOnlyFormatSpecifier, InvariantCulture),
			DateTime dateTime => dateTime.ToString(options.DateTimeFormatSpecifier, InvariantCulture),
			DateTimeOffset dateTimeOffset => dateTimeOffset.ToString(options.DateTimeOffsetFormatSpecifier, InvariantCulture),
			TimeOnly time => time.ToString(options.TimeOnlyFormatSpecifier, InvariantCulture),
			TimeSpan time => time.ToString(options.TimeSpanFormatSpecifier, InvariantCulture),
			Guid guid => guid.ToString(options.GuidFormatSpecifier, InvariantCulture),
			Enum token => token.ToString(options.EnumFormatSpecifier),
			string text when text.ContainsAny('"', ',', '\r', '\n') => Invariant($"\"{text.Replace("\"", "\"\"")}\""),
			string text => text,
			_ => @this.ToString()?.EscapeCSV() ?? string.Empty
		};

	public static string ToCSV(this IEnumerable<string>? @this, bool escape = false)
		=> @this switch
		{
			null => string.Empty,
			_ when escape => string.Join(',', @this.Select(text => text.Contains(',') ? Invariant($"\"{text.Replace("\"", "\"\"")}\"") : (text?.Replace("\"", "\"\"") ?? string.Empty))),
			_ => string.Join(", ", @this)
		};

	public static string[] ToCSV<T>(this IEnumerable<T> @this, CsvOptions options = default)
		where T : notnull
	{
		var headerRow = string.Empty;
		var dataRows = Array<string>.Empty;

		if (options.MemberNames.Any())
		{
			var memberMap = new Dictionary<string, Func<T, object?>>(options.MemberNames.Select(name =>
			{
				var propertyInfo = TypeOf<T>.Properties.FirstOrDefault(_ => _.Name().Is(name));
				if (propertyInfo is not null)
					return KeyValuePair.Create<string, Func<T, object?>>(name, new Func<T, object?>(_ => propertyInfo.GetPropertyValue(_)));

				var fieldInfo = TypeOf<T>.Fields.FirstOrDefault(_ => _.Name().Is(name));
				return KeyValuePair.Create<string, Func<T, object?>>(name, new Func<T, object?>(_ => fieldInfo?.GetFieldValue(_)));
			}), StringComparer.OrdinalIgnoreCase);
			headerRow = string.Join(',', options.MemberNames.Where(memberMap.ContainsKey));
			dataRows = @this.Select(row => string.Join(',', options.MemberNames.Select(name => memberMap[name]!(row).EscapeCSV(options)))).ToArray();
		}
		else if (TypeOf<T>.Properties.Any())
		{
			headerRow = string.Join(',', TypeOf<T>.Properties.Select(property => property.Name()));
			dataRows = @this.Select(row => string.Join(',', TypeOf<T>.Properties.Select(property => property.GetValue(row).EscapeCSV(options)))).ToArray();
		}
		else if (TypeOf<T>.Fields.Any())
		{
			headerRow = string.Join(',', TypeOf<T>.Fields.Select(field => field.Name()));
			dataRows = @this.Select(row => string.Join(',', TypeOf<T>.Fields.Select(fieldInfo => fieldInfo.GetFieldValue(row).EscapeCSV(options)))).ToArray();
		}

		return dataRows.Prepend(headerRow).ToArray();
	}
}
