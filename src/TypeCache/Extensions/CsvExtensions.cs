// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class CsvExtensions
{
	private static string EscapeCSV(this string @this)
		=> @this switch
		{
			_ when @this.ContainsAny('"', ',', '\r', '\n') => Invariant($"\"{@this.Replace("\"", "\"\"")}\""),
			_ => @this,
		};

	private static string EscapeCSV(this object? @this, CsvOptions options = default)
		=> @this switch
		{
			null => options.NullText,
			true => options.TrueText,
			false => options.FalseText,
			',' or '"' => Invariant($"\"{@this}\""),
			char character => character.ToString(),
			sbyte or byte => ((IFormattable)@this).ToString(options.ByteFormatSpecifier, InvariantCulture),
			short or int or nint or long or Int128 or ushort or uint or nuint or ulong or UInt128 => ((IFormattable)@this).ToString(options.IntegerFormatSpecifier, InvariantCulture),
			float or double or Half or decimal => ((IFormattable)@this).ToString(options.DecimalFormatSpecifier, InvariantCulture),
			DateOnly => ((IFormattable)@this).ToString(options.DateOnlyFormatSpecifier, InvariantCulture),
			DateTime => ((IFormattable)@this).ToString(options.DateTimeFormatSpecifier, InvariantCulture),
			DateTimeOffset => ((IFormattable)@this).ToString(options.DateTimeOffsetFormatSpecifier, InvariantCulture),
			TimeOnly => ((IFormattable)@this).ToString(options.TimeOnlyFormatSpecifier, InvariantCulture),
			TimeSpan => ((IFormattable)@this).ToString(options.TimeSpanFormatSpecifier, InvariantCulture),
			Guid => ((IFormattable)@this).ToString(options.GuidFormatSpecifier, InvariantCulture),
			Enum => ((IFormattable)@this).ToString(options.EnumFormatSpecifier, InvariantCulture),
			string text => text.EscapeCSV(),
			_ => @this.ToString()?.EscapeCSV() ?? string.Empty
		};

	public static string ToCSV<T>(this IEnumerable<T>? @this, bool escape = false)
		where T : unmanaged
		=> @this is not null ? string.Join(", ", @this) : string.Empty;

	public static string ToCSV(this IEnumerable<string>? @this, bool escape = false)
		=> @this switch
		{
			null => string.Empty,
			_ when escape => string.Join(',', @this.EscapeCSV()),
			_ => string.Join(", ", @this)
		};

	public static string[] ToCSV<T>(this IEnumerable<T> @this, CsvOptions options = default)
		where T : notnull
	{
		var propertyInfos = typeof(T).GetPublicProperties();
		if (options.MemberNames.Any())
			propertyInfos = propertyInfos.IntersectBy(options.MemberNames, propertyInfo => propertyInfo.Name, options.MemberNameComparison.ToStringComparer()).ToArray();
		if (propertyInfos.Any())
		{
			var headerRow = string.Join(',', propertyInfos.Select(_ => _.Name().EscapeCSV()));
			var dataRows = @this.Select(row => string.Join(',', propertyInfos.Select(_ => _.GetPropertyValue(row).EscapeCSV(options))));
			return dataRows.Prepend(headerRow).ToArray();
		}

		var fieldInfos = typeof(T).GetPublicFields();
		if (options.MemberNames.Any())
			fieldInfos = fieldInfos.IntersectBy(options.MemberNames, fieldInfo => fieldInfo.Name, options.MemberNameComparison.ToStringComparer()).ToArray();
		if (fieldInfos.Any())
		{
			var headerRow = string.Join(',', fieldInfos.Select(_ => _.Name()));
			var dataRows = @this.Select(row => string.Join(',', fieldInfos.Select(_ => _.GetFieldValue(row).EscapeCSV(options))));
			return dataRows.Prepend(headerRow).ToArray();
		}

		return Array<string>.Empty;
	}
}
