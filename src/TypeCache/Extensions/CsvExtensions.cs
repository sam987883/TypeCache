// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using TypeCache.Collections;
using TypeCache.Extensions;
using static System.FormattableString;
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
			string text when text.Contains('"') || text.Contains(',') || text.Contains('\r') || text.Contains('\n') => Invariant($"\"{text.Replace("\"", "\"\"")}\""),
			string text => text,
			_ => @this.ToString()?.EscapeCSV() ?? string.Empty
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see cref="string.Empty"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> <paramref name="escape"/> =&gt; string.Join(',', @this.Select(text =&gt; text.Contains(',') ? Invariant($"\"{text.Replace("\"", "\"\"")}\"") : (text?.Replace("\"", "\"\"") ?? string.Empty))),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="string"/>.Join(", ", @<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static string ToCSV(this IEnumerable<string>? @this, bool escape = false)
		=> @this switch
		{
			null => string.Empty,
			_ when escape => string.Join(',', @this.Select(text => text.Contains(',') ? Invariant($"\"{text.Replace("\"", "\"\"")}\"") : (text?.Replace("\"", "\"\"") ?? string.Empty))),
			_ => string.Join(", ", @this)
		};

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> headerRow = <see cref="string.Empty"/>;<br/>
	/// <see langword="    var"/> dataRows = Array&lt;<see cref="string"/>&gt;.Empty;<br/>
	/// <br/>
	/// <see langword="    if"/> (<paramref name="options"/>.MemberNames.Any())<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        var"/> memberMap = <see langword="new"/> Dictionary&lt;<see cref="string"/>, Func&lt;<typeparamref name="T"/>, <see cref="object"/>&gt;&gt;(<paramref name="options"/>.MemberNames.Select(name =&gt;<br/>
	/// <see langword="        "/>{<br/>
	/// <see langword="            var"/> property = <see cref="TypeOf{T}.Properties"/>.FirstOrDefault(_ =&gt; _.Name.Is(name));<br/>
	/// <see langword="            if"/> (property <see langword="is not null"/>)<br/>
	/// <see langword="                return"/> <see cref="KeyValuePair"/>.Create(name, <see langword="new"/> Func&lt;<typeparamref name="T"/>, <see cref="object"/>&gt;(_ =&gt; property.GetValue(_)));<br/>
	/// <br/>
	/// <see langword="            var"/> field = <see cref="TypeOf{T}.Fields"/>.FirstOrDefault(_ =&gt; _.Name.Is(name));<br/>
	/// <see langword="            return"/> <see cref="KeyValuePair"/>.Create(name, <see langword="new"/> Func&lt;<typeparamref name="T"/>, <see cref="object"/>&gt;(_ =&gt; field.GetValue(_)));<br/>
	/// <see langword="        "/>}), <see cref="StringComparer.OrdinalIgnoreCase"/>);<br/>
	/// <see langword="        "/>headerRow = <see cref="string"/>.Join(',', <paramref name="options"/>.MemberNames.Where(memberMap.ContainsKey));<br/>
	/// <see langword="        "/>dataRows = @<paramref name="this"/>.Select(row =&gt; <see cref="string"/>.Join(',', <paramref name="options"/>.MemberNames.Select(name =&gt; memberMap[name](row).EscapeCSV(options)))).ToArray();<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    else if"/> (<see cref="TypeOf{T}.Properties"/>.Any())<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        "/>headerRow = <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Properties"/>.Select(property =&gt; property.Name));<br/>
	/// <see langword="        "/>dataRows = @<paramref name="this"/>.Select(row =&gt; <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Properties"/>.Select(property =&gt; property.GetValue(row).EscapeCSV(<paramref name="options"/>)))).ToArray();<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    else if"/> (<see cref="TypeOf{T}.Fields"/>.Any())<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        "/>headerRow = <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Fields"/>.Select(field =&gt; field.Name));<br/>
	/// <see langword="        "/>dataRows = @<paramref name="this"/>.Select(row =&gt; <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Fields"/>.Select(field =&gt; field.GetValue(row).EscapeCSV(<paramref name="options"/>)))).ToArray();<br/>
	/// <see langword="    "/>}<br/>
	/// <br/>
	/// <see langword="    return"/> dataRows.Prepend(headerRow).ToArray();<br/>
	/// }
	/// </code>
	/// </summary>
	public static string[] ToCSV<T>(this IEnumerable<T> @this, CsvOptions options = default)
	{
		var headerRow = string.Empty;
		var dataRows = Array<string>.Empty;

		if (options.MemberNames.Any())
		{
			var memberMap = new Dictionary<string, Func<T, object?>>(options.MemberNames.Select(name =>
			{
				var property = TypeOf<T>.Properties.FirstOrDefault(_ => _.Name.Is(name));
				if (property is not null)
					return KeyValuePair.Create<string, Func<T, object?>>(name, new Func<T, object?>(_ => property.GetValue(_!)));

				var field = TypeOf<T>.Fields.FirstOrDefault(_ => _.Name.Is(name));
				return KeyValuePair.Create<string, Func<T, object?>>(name, new Func<T, object?>(_ => field?.GetValue!(_!)));
			}), StringComparer.OrdinalIgnoreCase);
			headerRow = string.Join(',', options.MemberNames.Where(memberMap.ContainsKey));
			dataRows = @this.Select(row => string.Join(',', options.MemberNames.Select(name => memberMap[name]!(row).EscapeCSV(options)))).ToArray();
		}
		else if (TypeOf<T>.Properties.Any())
		{
			headerRow = string.Join(',', TypeOf<T>.Properties.Select(property => property.Name));
			dataRows = @this.Select(row => string.Join(',', TypeOf<T>.Properties.Select(property => property.GetValue(row!).EscapeCSV(options)))).ToArray();
		}
		else if (TypeOf<T>.Fields.Any())
		{
			headerRow = string.Join(',', TypeOf<T>.Fields.Select(field => field.Name));
			dataRows = @this.Select(row => string.Join(',', TypeOf<T>.Fields.Select(field => field.GetValue!(row!).EscapeCSV(options)))).ToArray();
		}

		return dataRows.Prepend(headerRow).ToArray();
	}
}
