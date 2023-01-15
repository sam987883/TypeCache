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
			',' or '"' => Invariant($"\"{@this}\""),
			true => options.TrueText,
			false => options.FalseText,
			sbyte or byte => ((IFormattable)@this).ToString(options.ByteFormatSpecifier, InvariantCulture),
			short or int or nint or long or Int128 or ushort or uint or nuint or ulong => ((IFormattable)@this).ToString(options.IntegerFormatSpecifier, InvariantCulture),
			float or double or Half or decimal => ((IFormattable)@this).ToString(options.DecimalFormatSpecifier, InvariantCulture),
			char character => character.ToString(),
			DateOnly => ((IFormattable)@this).ToString(options.DateOnlyFormatSpecifier, InvariantCulture),
			DateTime => ((IFormattable)@this).ToString(options.DateTimeFormatSpecifier, InvariantCulture),
			DateTimeOffset => ((IFormattable)@this).ToString(options.DateTimeOffsetFormatSpecifier, InvariantCulture),
			TimeOnly => ((IFormattable)@this).ToString(options.TimeOnlyFormatSpecifier, InvariantCulture),
			TimeSpan => ((IFormattable)@this).ToString(options.TimeSpanFormatSpecifier, InvariantCulture),
			Guid => ((IFormattable)@this).ToString(options.GuidFormatSpecifier, InvariantCulture),
			Enum => ((IFormattable)@this).ToString(options.EnumFormatSpecifier, InvariantCulture),
			string text when text.ContainsAny('"', ',', '\r', '\n') => Invariant($"\"{text.Replace("\"", "\"\"")}\""),
			string text => text,
			_ => @this.ToString()?.EscapeCSV() ?? string.Empty
		};

	public static string ToCSV<T>(this IEnumerable<T>? @this, bool escape = false)
		where T : unmanaged
		=> @this is not null ? string.Join(", ", @this) : string.Empty;

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
		var dataRows = Array<string>.Empty.AsEnumerable();
		var propertyInfos = TypeOf<T>.Properties;
		var fieldInfos = TypeOf<T>.Fields;

		if (options.MemberNames.Any())
		{
			var memberMap = new Dictionary<string, Func<T, object?>>(options.MemberNames.Length, options.MemberNameComparison.ToStringComparer());
			foreach (var name in options.MemberNames)
			{
				var propertyInfo = propertyInfos.FirstOrDefault(_ => _.Name().Is(name, options.MemberNameComparison));
				if (propertyInfo is not null)
					memberMap[name] = new Func<T, object?>(_ => propertyInfo.GetPropertyValue(_));

				var fieldInfo = fieldInfos.FirstOrDefault(_ => _.Name().Is(name, options.MemberNameComparison));
				if (fieldInfo is not null)
					memberMap[name] = new Func<T, object?>(_ => fieldInfo.GetFieldValue(_));
			}
			headerRow = string.Join(',', options.MemberNames.Where(memberMap.ContainsKey));
			dataRows = @this.Select(row => string.Join(',', options.MemberNames.Select(name => memberMap[name]!(row).EscapeCSV(options))));
		}
		else if (propertyInfos.Any())
		{
			headerRow = string.Join(',', propertyInfos.Select(_ => _.Name()));
			dataRows = @this.Select(row => string.Join(',', propertyInfos.Select(_ => _.GetValue(row).EscapeCSV(options))));
		}
		else if (fieldInfos.Any())
		{
			headerRow = string.Join(',', fieldInfos.Select(_ => _.Name()));
			dataRows = @this.Select(row => string.Join(',', fieldInfos.Select(_ => _.GetFieldValue(row).EscapeCSV(options))));
		}

		return dataRows.Prepend(headerRow).ToArray();
	}
}
