// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
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
			string text when text.HasAny(new[] { '"', ',', '\r', '\n' }) => Invariant($"\"{text.Replace("\"", "\"\"")}\""),
			string text => text,
			_ => @this.ToString()?.EscapeCSV() ?? string.Empty
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see cref="string.Empty"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> <paramref name="escape"/> =&gt; @<paramref name="this"/><br/>
	/// <see langword="        "/>.Map(text =&gt; text.IsNotBlank() ? (text.Contains(',') ? Invariant($"\"{text.Replace("\"", "\"\"")}\"") : text.Replace("\"", "\"\"")) : <see cref="string.Empty"/>)<br/>
	/// <see langword="        "/>.Join(','),<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.Join(", ")<br/>
	/// };
	/// </code>
	/// </summary>
	public static string ToCSV(this IEnumerable<string>? @this, bool escape = false)
		=> @this switch
		{
			null => string.Empty,
			_ when escape => @this.Map(text => text.IsNotBlank() ? (text.Contains(',') ? Invariant($"\"{text.Replace("\"", "\"\"")}\"") : text.Replace("\"", "\"\"")) : string.Empty).Join(','),
			_ => @this.Join(", ")
		};

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> headerRow = <see cref="string.Empty"/>;<br/>
	/// <see langword="    var"/> dataRows = Array&lt;<see cref="string"/>&gt;.Empty;<br/>
	/// <br/>
	/// <see langword="    if"/> (<paramref name="options"/>.MemberNames.Any())<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        var"/> memberMap = <paramref name="options"/>.MemberNames.Map(name =&gt;<br/>
	/// <see langword="        "/>{<br/>
	/// <see langword="            var"/> property = <see cref="TypeOf{T}.Properties"/>.First(_ =&gt; _.Name.Is(name));<br/>
	/// <see langword="            if"/> (property <see langword="is not null"/>)<br/>
	/// <see langword="                return"/> <see cref="KeyValuePair"/>.Create(name, <see langword="new"/> Func&lt;<typeparamref name="T"/>, <see cref="object"/>&gt;(_ =&gt; property.GetValue(_)));<br/>
	/// <br/>
	/// <see langword="            var"/> field = <see cref="TypeOf{T}.Fields"/>.First(_ =&gt; _.Name.Is(name));<br/>
	/// <see langword="            return"/> <see cref="KeyValuePair"/>.Create(name, <see langword="new"/> Func&lt;<typeparamref name="T"/>, <see cref="object"/>&gt;(_ =&gt; field.GetValue(_)));<br/>
	/// <see langword="        "/>}).ToDictionary();<br/>
	/// <see langword="        "/>headerRow = <see cref="string"/>.Join(',', <paramref name="options"/>.MemberNames.If(memberMap.ContainsKey));<br/>
	/// <see langword="        "/>dataRows = @<paramref name="this"/>.Map(row =&gt; <see cref="string"/>.Join(',', <paramref name="options"/>.MemberNames.Map(name =&gt; memberMap[name](row).EscapeCSV(options))));<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    else if"/> (<see cref="TypeOf{T}.Properties"/>.Any())<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        "/>headerRow = <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Properties"/>.Map(property => property.Name));<br/>
	/// <see langword="        "/>dataRows = @<paramref name="this"/>.Map(row =&gt; <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Properties"/>.Map(property =&gt; property.GetValue(row).EscapeCSV(<paramref name="options"/>))));<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    else if"/> (<see cref="TypeOf{T}.Fields"/>.Any())<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        "/>headerRow = <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Fields"/>.Map(field => field.Name));<br/>
	/// <see langword="        "/>dataRows = @<paramref name="this"/>.Map(row =&gt; <see cref="string"/>.Join(',', <see cref="TypeOf{T}.Fields"/>.Map(field =&gt; field.GetValue(row).EscapeCSV(<paramref name="options"/>))));<br/>
	/// <see langword="    "/>}<br/>
	/// <br/>
	/// <see langword="    return new"/>[] { headerRow }.Append(dataRows).ToArray();<br/>
	/// }
	/// </code>
	/// </summary>
	public static string[] ToCSV<T>(this IEnumerable<T>? @this, CsvOptions options = default)
	{
		var headerRow = string.Empty;
		var dataRows = Array<string>.Empty;

		if (options.MemberNames.Any())
		{
			var memberMap = options.MemberNames.Map(name =>
			{
				var property = TypeOf<T>.Properties.First(_ => _.Name.Is(name));
				if (property is not null)
					return KeyValuePair.Create(name, new Func<T, object?>(_ => property.GetValue(_!)));

				var field = TypeOf<T>.Fields.First(_ => _.Name.Is(name));
				return KeyValuePair.Create(name, new Func<T, object?>(_ => field?.GetValue!(_!)));
			}).ToDictionary();

			headerRow = string.Join(',', options.MemberNames.If(memberMap.ContainsKey));
			dataRows = @this.Map(row => string.Join(',', options.MemberNames.Map(name => memberMap[name]!(row).EscapeCSV(options))));
		}
		else if (TypeOf<T>.Properties.Any())
		{
			headerRow = string.Join(',', TypeOf<T>.Properties.Map(property => property.Name));
			dataRows = @this.Map(row => string.Join(',', TypeOf<T>.Properties.Map(property => property.GetValue(row!).EscapeCSV(options))));
		}
		else if (TypeOf<T>.Fields.Any())
		{
			headerRow = string.Join(',', TypeOf<T>.Fields.Map(field => field.Name));
			dataRows = @this.Map(row => string.Join(',', TypeOf<T>.Fields.Map(field => field.GetValue!(row!).EscapeCSV(options))));
		}

		return new[] { headerRow }.Append(dataRows).ToArray();
	}
}
