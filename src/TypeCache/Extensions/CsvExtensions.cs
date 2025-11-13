// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class CsvExtensions
{
	private static readonly char[] ESCAPE_CHARS = ['"', ',', '\r', '\n'];

	extension(object? @this)
	{
		public string EscapeCSV()
			=> @this switch
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
	}

	extension(char @this)
	{
		/// <remarks>
		/// <c>=&gt; <see cref="ESCAPE_CHARS"/>.Contains(@this) ? Invariant($"\"{@this}\"") : @this.ToString();</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string EscapeCSV()
			=> ESCAPE_CHARS.Contains(@this) ? Invariant($"\"{@this}\"") : @this.ToString();
	}

	extension(string @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.ContainsAny(<see cref="ESCAPE_CHARS"/>) ? Invariant($"\"{@this.Replace("\"", "\"\"")}\"") : @this;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string EscapeCSV()
			=> @this.ContainsAny(ESCAPE_CHARS) ? Invariant($"\"{@this.Replace("\"", "\"\"")}\"") : @this;
	}

	extension(IEnumerable<string>? @this)
	{
		/// <remarks>
		/// <c>=&gt; @this <see langword="is not null"/> ? <see cref="string"/>.Join(", ", @this) : <see cref="string.Empty"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToCSV()
			=> @this is not null ? string.Join(", ", @this) : string.Empty;
	}

	extension<T>(IEnumerable<T> @this) where T : notnull
	{
		/// <summary>
		/// Creates a <c><see langword="string[]"/></c> result of formal (escaped) CSV data.
		/// </summary>
		/// <returns>Formal (escaped) CSV data</returns>
		public string[] ToCSV()
		{
			var properties = Type<T>.Properties.Values.Where(_ => _.CanRead).ToArray();
			if (properties.Length > 0)
			{
				var headerRow = string.Join(',', properties.Select(_ => _.Name.EscapeCSV()));
				var dataRows = @this.Select(row => string.Join(',', properties.Select(_ => _.GetValue(row).EscapeCSV())));

				return [headerRow, .. dataRows];
			}

			var fields = Type<T>.Fields.Values.Where(_ => _.IsPublic).ToArray();
			if (fields.Length > 0)
			{
				var headerRow = string.Join(',', fields.Select(_ => _.Name.EscapeCSV()));
				var dataRows = @this.Select(row => string.Join(',', fields.Select(_ => _.GetValue(row).EscapeCSV())));

				return [headerRow, .. dataRows];
			}

			return [];
		}
	}
}
