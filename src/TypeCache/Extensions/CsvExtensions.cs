// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using TypeCache.Collections;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class CsvExtensions
{
	private static readonly char[] ESCAPE_CHARS = ['"', ',', '\r', '\n'];

	/// <remarks>
	/// <c>=&gt; <see cref="ESCAPE_CHARS"/>.Contains(@<paramref name="this"/>) ? Invariant($"\"{@<paramref name="this"/>}\"") : @<paramref name="this"/>.ToString();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string EscapeCSV(this char @this)
		=> ESCAPE_CHARS.Contains(@this) ? Invariant($"\"{@this}\"") : @this.ToString();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ContainsAny(<see cref="ESCAPE_CHARS"/>) ? Invariant($"\"{@<paramref name="this"/>.Replace("\"", "\"\"")}\"") : @<paramref name="this"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string EscapeCSV(this string @this)
		=> @this.ContainsAny(ESCAPE_CHARS) ? Invariant($"\"{@this.Replace("\"", "\"\"")}\"") : @this;

	public static string EscapeCSV(this object? @this) => @this switch
	{
		null => string.Empty,
		bool => @this.ToString(),
		char character => character.EscapeCSV(),
		IFormattable formattable => @this switch
		{
			sbyte or byte => formattable.ToString("X", InvariantCulture),
			DateOnly or DateTime or DateTimeOffset or TimeOnly => formattable.ToString("O", InvariantCulture),
			TimeSpan => formattable.ToString("c", InvariantCulture),
			Guid => formattable.ToString("D", InvariantCulture),
			Enum => formattable.ToString("F", InvariantCulture),
			_ when @this.GetType().Implements(typeof(IBinaryInteger<>)) => formattable.ToString("D", InvariantCulture),
			_ when @this.GetType().Implements(typeof(IFloatingPoint<>)) => formattable.ToString("D", InvariantCulture),
			_ => @this.ToString()?.EscapeCSV()
		},
		string text => text.EscapeCSV(),
		_ => @this.ToString()?.EscapeCSV()
	} ?? string.Empty;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see cref="string"/>.Join(", ", @<paramref name="this"/>) : <see cref="string.Empty"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToCSV(this IEnumerable<string>? @this)
		=> @this is not null ? string.Join(", ", @this) : string.Empty;

	/// <summary>
	/// Creates a <c><see langword="string[]"/></c> result of formal (escaped) CSV data.
	/// </summary>
	/// <typeparam name="T">The model to convert to CSV.</typeparam>
	/// <returns>Formal (escaped) CSV data</returns>
	public static string[] ToCSV<T>(this IEnumerable<T> @this)
		where T : notnull
	{
		var propertyInfos = typeof(T).GetPublicProperties().Where(propertyInfo => propertyInfo.CanRead).ToArray();
		if (propertyInfos.Length > 0)
		{
			var funcs = propertyInfos.Select(_ => _.GetValueFunc()).ToArray();
			var headerRow = string.Join(',', propertyInfos.Select(propertyInfo => propertyInfo.Name.EscapeCSV()));
			var dataRows = @this.Select(row => string.Join(',', funcs.Select(_ => _.Invoke(row, null).EscapeCSV())));

			return [headerRow, ..dataRows];
		}

		var fieldInfos = typeof(T).GetPublicFields();
		if (fieldInfos.Length > 0)
		{
			var funcs = fieldInfos.Select(_ => _.GetValueFunc()).ToArray();
			var headerRow = string.Join(',', fieldInfos.Select(fieldInfo => fieldInfo.Name.EscapeCSV()));
			var dataRows = @this.Select(row => string.Join(',', funcs.Select(_ => _.Invoke(row).EscapeCSV())));

			return [headerRow, .. dataRows];
		}

		return Array<string>.Empty;
	}
}
