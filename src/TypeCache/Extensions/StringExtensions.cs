// Copyright (c) 2021 Samuel Abraham

using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;
using TypeCache.Collections;
using static System.Globalization.CultureInfo;
using static System.StringSplitOptions;

namespace TypeCache.Extensions;

public static class StringExtensions
{
	private const string RANGE_OPERATOR = "..";

	extension(string @this)
	{
		/// <inheritdoc cref="char.IsDigit(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsDigit());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyDigit()
			=> @this.Any(c => c.IsDigit());

		/// <inheritdoc cref="char.IsLetter(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsLetter());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyLetter()
			=> @this.Any(c => c.IsLetter());

		/// <inheritdoc cref="char.IsLetterOrDigit(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsLetterOrDigit());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyLetterOrDigit()
			=> @this.Any(c => c.IsLetterOrDigit());

		/// <inheritdoc cref="char.IsLower(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsLower());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyLower()
			=> @this.Any(c => c.IsLower());

		/// <inheritdoc cref="char.IsNumber(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsNumber());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyNumber()
			=> @this.Any(c => c.IsNumber());

		/// <inheritdoc cref="char.IsPunctuation(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsPunctuation());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyPunctuation()
			=> @this.Any(c => c.IsPunctuation());

		/// <inheritdoc cref="char.IsSymbol(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsSymbol());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnySymbol()
			=> @this.Any(c => c.IsSymbol());

		/// <inheritdoc cref="char.IsUpper(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsUpper());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyUpper()
			=> @this.Any(c => c.IsUpper());

		/// <inheritdoc cref="char.IsWhiteSpace(char)"/>
		/// <remarks>
		/// <c>=&gt; @this.Any(c =&gt; c.IsWhiteSpace());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool AnyWhiteSpace()
			=> @this.Any(c => c.IsWhiteSpace());

		/// <remarks>
		/// <c>=&gt; <paramref name="chars"/>?.Any(@this.Contains) <see langword="is true"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsAny(char[]? chars = null)
			=> chars?.Any(@this.Contains) is true;

		/// <inheritdoc cref="string.Contains(char, StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.Contains(<paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsIgnoreCase(char value)
			=> @this.Contains(value, StringComparison.OrdinalIgnoreCase);

		/// <inheritdoc cref="string.Contains(string, StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.Contains(<paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsIgnoreCase(string value)
			=> @this.Contains(value, StringComparison.OrdinalIgnoreCase);

		/// <inheritdoc cref="string.Contains(string, StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.Contains(<paramref name="value"/>, <see cref="StringComparison.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsOrdinal(string value)
			=> @this.Contains(value, StringComparison.Ordinal);

		/// <inheritdoc cref="string.EndsWith(string, StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.EndsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EndsWithIgnoreCase(string text)
			=> @this.EndsWith(text, StringComparison.OrdinalIgnoreCase);

		/// <inheritdoc cref="string.EndsWith(string, StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.EndsWith(<paramref name="text"/>, <see cref="StringComparison.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EndsWithOrdinal(string text)
			=> @this.EndsWith(text, StringComparison.Ordinal);

		/// <inheritdoc cref="Regex.Escape(string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Regex"/>.Escape(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string EscapeRegex()
			=> Regex.Escape(@this);

		/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
		public string FromBase64(Encoding? encoding = null)
		{
			encoding ??= Encoding.UTF8;
			return encoding.GetString(Convert.FromBase64String(@this));
		}

		/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
		public string FromBase64Url(Encoding? encoding = null)
		{
			encoding ??= Encoding.UTF8;
			return encoding.GetString(Base64Url.DecodeFromChars(@this));
		}

		/// <inheritdoc cref="string.GetHashCode(StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.GetHashCode(<see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public int GetHashCodeIgnoreCase()
			=> @this.GetHashCode(StringComparison.OrdinalIgnoreCase);

		/// <inheritdoc cref="string.GetHashCode(StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.GetHashCode(<see cref="StringComparison.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public int GetHashCodeOrdinal()
			=> @this.GetHashCode(StringComparison.Ordinal);

		/// <remarks>
		/// <c>=&gt; @this.AsSpan().Join(<paramref name="values"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Join(IEnumerable<string> values)
			=> @this.AsSpan().Join(values);

		/// <remarks>
		/// <c>=&gt; @this.AsSpan().Join(<paramref name="values"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Join(string[] values)
			=> @this.AsSpan().Join(values);

		/// <inheritdoc cref="string.Substring(int, int)"/>
		/// <remarks>
		/// <c>=&gt; @this.Substring(0, <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Left(int length)
			=> @this.Substring(0, length);

		/// <summary>
		/// Mask letter or numbers within a string.
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Mask(char mask = '*')
			=> @this.AsSpan().Mask(mask);

		/// <summary>
		/// Mask <paramref name="terms"/> within a string.
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Mask(char mask, string[] terms, StringComparison comparison)
			=> @this.AsSpan().Mask(mask, terms, comparison);

		/// <summary>
		/// Mask letter or numbers in a string.
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string MaskIgnoreCase(char mask, string[] terms)
			=> @this.AsSpan().MaskIgnoreCase(mask, terms);

		/// <summary>
		/// Mask letter or numbers in a string.
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string MaskOrdinal(char mask, string[] terms)
			=> @this.AsSpan().MaskOrdinal(mask, terms);

		/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.Parse(@this, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Parse<T>(IFormatProvider? formatProvider = null)
			where T : IParsable<T>
			=> T.Parse(@this, formatProvider ?? InvariantCulture);

		/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.Parse(@this, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Parse<T>(NumberStyles style, IFormatProvider? formatProvider = null)
			where T : INumberBase<T>
			=> T.Parse(@this, style, formatProvider ?? InvariantCulture);

		public string Reverse()
		{
			Span<char> span = stackalloc char[@this.Length];
			@this.AsSpan().CopyTo(span);
			span.Reverse();
			return new string(span);
		}

		/// <inheritdoc cref="string.Split(char, StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separator"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(char separator)
			=> @this.Split(separator, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.Split(char, int, StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separator"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(char separator, int count)
			=> @this.Split(separator, count, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.Split(char[], StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separators"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(char[] separators)
			=> @this.Split(separators, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.Split(char[], int, StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separators"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(char[] separators, int count)
			=> @this.Split(separators, count, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.Split(string, StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separator"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(string separator)
			=> @this.Split(separator, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.Split(string, int, StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separator"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(string separator, int count)
			=> @this.Split(separator, count, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.Split(string[], StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separators"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(string[] separators)
			=> @this.Split(separators, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.Split(string[], int, StringSplitOptions)"/>
		/// <remarks>
		/// <c>=&gt; @this.Split(<paramref name="separators"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] SplitEx(string[] separators, int count)
			=> @this.Split(separators, count, RemoveEmptyEntries | TrimEntries);

		/// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.StartsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool StartsWithIgnoreCase(string text)
			=> @this.StartsWith(text, StringComparison.OrdinalIgnoreCase);

		/// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; @this.StartsWith(<paramref name="text"/>, <see cref="StringComparison.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool StartsWithOrdinal(string text)
			=> @this.StartsWith(text, StringComparison.Ordinal);

		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="string"/>(@this.Take(<paramref name="range"/>).ToArray());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Substring(Range range)
			=> new string(@this.Take(range).ToArray());

		/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
		public string ToBase64(Encoding? encoding = null)
		{
			encoding ??= Encoding.UTF8;
			var length = encoding.GetMaxByteCount(@this.Length) - 1;
			Span<byte> bytes = stackalloc byte[length];

			return encoding.TryGetBytes(@this, bytes, out length)
				? bytes.Slice(0, length).AsReadOnly().ToBase64()
				: string.Empty;
		}

		/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
		public string ToBase64Url(Encoding? encoding = null)
		{
			encoding ??= Encoding.UTF8;
			var length = encoding.GetMaxByteCount(@this.Length) - 1;
			Span<byte> bytes = stackalloc byte[length];

			return encoding.TryGetBytes(@this, bytes, out length)
				? bytes.Slice(0, length).AsReadOnly().ToBase64Url()
				: string.Empty;
		}

		/// <summary>
		/// <code>
		/// "1..^2" ---&gt; 1..^2
		/// "..^1"  ---&gt; 0..^1
		/// "1.."   ---&gt; 1..^0
		/// "3"     ---&gt; 3..4
		/// </code>
		/// </summary>
		public Range? ToRange()
		{
			if (@this.EqualsIgnoreCase(RANGE_OPERATOR))
				return Range.All;

			var index = @this.ToIndex();
			if (index.HasValue)
				return new(index.Value.Value, index.Value.Value + 1);

			if (!@this.ContainsIgnoreCase(RANGE_OPERATOR))
				return null;

			if (@this.EndsWithIgnoreCase(RANGE_OPERATOR))
			{
				index = @this.AsSpan(0..^2).ToIndex();
				return index.HasValue ? Range.StartAt(index.Value) : null;
			}

			if (@this.StartsWithIgnoreCase(RANGE_OPERATOR))
			{
				index = @this.AsSpan(RANGE_OPERATOR.Length).ToIndex();
				return index.HasValue ? Range.EndAt(index.Value) : null;
			}

			var tokens = @this.SplitEx(RANGE_OPERATOR);
			if (tokens.Length is 2)
			{
				var startIndex = tokens[0].ToIndex();
				if (!startIndex.HasValue)
					return null;

				var endIndex = tokens[1].ToIndex();
				if (!endIndex.HasValue)
					return null;

				return new(startIndex.Value, endIndex.Value);
			}

			return null;
		}

		/// <inheritdoc cref="StringSegment.StringSegment(string, int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="StringSegment"/>(@this, <paramref name="offset"/>, <paramref name="count"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringSegment ToStringSegment(int offset, int count)
			=> new(@this, offset, count);

		/// <inheritdoc cref="Uri.Uri(string)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this, @this[0] is '/' ? <see cref="UriKind.Relative"/> : <see cref="UriKind.Absolute"/>);</c>
		/// </remarks>
		/// <exception cref="IndexOutOfRangeException"/>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Uri ToUri()
			=> new(@this, @this[0] is '/' ? UriKind.Relative : UriKind.Absolute);

		/// <inheritdoc cref="Uri.Uri(string, UriKind)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="Uri"/>(@this, <paramref name="kind"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Uri ToUri(UriKind kind)
			=> new(@this, kind);

		public string TrimEndOrdinal(string text)
			=> @this.EndsWith(text, StringComparison.Ordinal) ? @this.Substring(0, @this.Length - text.Length) : @this;

		public string TrimEndIgnoreCase(string text)
			=> @this.EndsWith(text, StringComparison.OrdinalIgnoreCase) ? @this.Substring(0, @this.Length - text.Length) : @this;

		public string TrimStartOrdinal(string text)
			=> @this.StartsWith(text, StringComparison.Ordinal) ? @this.Substring(text.Length) : @this;

		public string TrimStartIgnoreCase(string text)
			=> @this.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? @this.Substring(text.Length) : @this;

		/// <inheritdoc cref="Convert.TryFromBase64String(string, Span{byte}, out int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.TryFromBase64String(@this, <paramref name="bytes"/>, <see langword="out"/> <paramref name="bytesWritten"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryFromBase64(Span<byte> bytes, out int bytesWritten)
			=> Convert.TryFromBase64Chars(@this, bytes, out bytesWritten);

		/// <inheritdoc cref="Regex.Unescape(string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Regex"/>.Unescape(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string UnescapeRegex()
			=> Regex.Unescape(@this);
	}

	extension([NotNullWhen(true)] string? @this)
	{
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Equals(@this, <paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EqualsIgnoreCase([NotNullWhen(true)] string? value)
			=> string.Equals(@this, value, StringComparison.OrdinalIgnoreCase);

		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Equals(@this, <paramref name="value"/>, <see cref="StringComparison.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EqualsOrdinal([NotNullWhen(true)] string? value)
			=> string.Equals(@this, value, StringComparison.Ordinal);

		/// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
		/// <remarks>
		/// <c>=&gt; !<see cref="string"/>.IsNullOrWhiteSpace(@this);</c>
		/// </remarks>
		[DebuggerHidden]
		public bool IsNotBlank => !string.IsNullOrWhiteSpace(@this);

		/// <inheritdoc cref="string.IsNullOrEmpty(string?)"/>
		/// <remarks>
		/// <c>=&gt; !<see cref="string"/>.IsNullOrEmpty(@this);</c>
		/// </remarks>
		[DebuggerHidden]
		public bool IsNotNullOrEmpty => !string.IsNullOrEmpty(@this);

		/// <remarks>
		/// <c>=&gt; @this.IsNotBlank() ? @this : <paramref name="value"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		[return: NotNullIfNotNull(nameof(@this))]
		[return: NotNullIfNotNull(nameof(value))]
		public string? IfBlank(string? value)
			=> @this.IsNotBlank ? @this : value;

		/// <remarks>
		/// <c>=&gt; @this != <see cref="string.Empty"/> ? @this : <paramref name="value"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		[return: NotNullIfNotNull(nameof(@this))]
		[return: NotNullIfNotNull(nameof(value))]
		public string? IfEmpty(string? value)
			=> @this is not "" ? @this : value;

		/// <inheritdoc cref="Enum.TryParse{TEnum}(string?, out TEnum)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Enum"/>.TryParse(@this, <see langword="out var"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
		/// </remarks>
		[DebuggerHidden]
		public T? ToEnum<T>()
			where T : struct, Enum
			=> Enum.TryParse<T>(@this, out var result) ? (T?)result : null;

		/// <inheritdoc cref="Enum.TryParse{TEnum}(string?, bool, out TEnum)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Enum"/>.TryParse(@this, <see langword="true"/>, <see langword="out var"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
		/// </remarks>
		[DebuggerHidden]
		public T? ToEnumIgnoreCase<T>()
			where T : struct, Enum
			=> Enum.TryParse<T>(@this, true, out var result) ? (T?)result : null;

		/// <summary>
		/// <code>
		/// "^2" ---&gt; ^2
		/// "1"  ---&gt; 1
		/// </code>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Index? ToIndex()
			=> @this?.AsSpan().ToIndex();

		/// <inheritdoc cref="StringSegment.StringSegment(string?)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="StringSegment"/>(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringSegment ToStringSegment()
			=> new(@this);

		/// <inheritdoc cref="IParsable{TSelf}.TryParse(string, IFormatProvider, out TSelf)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.TryParse(@this, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
		/// </remarks>
		/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryParse<T>([MaybeNullWhen(false)] out T value, IFormatProvider? formatProvider = null)
			where T : IParsable<T>
			=> T.TryParse(@this, formatProvider ?? InvariantCulture, out value);

		/// <inheritdoc cref="INumberBase{TSelf}.TryParse(string, NumberStyles, IFormatProvider, out TSelf)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.TryParse(@this, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
		/// </remarks>
		/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryParse<T>(NumberStyles style, [MaybeNullWhen(false)] out T value, IFormatProvider? formatProvider = null)
			where T : INumberBase<T>
			=> T.TryParse(@this, style, formatProvider ?? InvariantCulture, out value);
	}

	extension([NotNullWhen(false)] string? @this)
	{
		/// <inheritdoc cref="Base64.IsValid(ReadOnlySpan{char})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Base64"/>.IsValid(@this);</c>
		/// </remarks>
		[DebuggerHidden]
		public bool IsBase64 => Base64.IsValid(@this);

		/// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.IsNullOrWhiteSpace(@this);</c>
		/// </remarks>
		[DebuggerHidden]
		public bool IsBlank => string.IsNullOrWhiteSpace(@this);

		/// <inheritdoc cref="string.IsNullOrEmpty(string?)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.IsNullOrEmpty(@this);</c>
		/// </remarks>
		[DebuggerHidden]
		public bool IsNullOrEmpty => string.IsNullOrEmpty(@this);
	}

	extension([StringSyntax(StringSyntaxAttribute.Regex)] string @this)
	{
		/// <remarks>
		/// <code>
		/// =&gt; RegexCache[(@this, <paramref name="options"/>)]; // <see langword="new"/> Regex(@this, <paramref name="options"/>, <see cref="TimeSpan"/>.FromMinutes(1));
		/// </code>
		/// </remarks>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Regex ToRegex(RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline)
			=> RegexCache[(@this, options)];
	}

	private static readonly IReadOnlyDictionary<(string Pattern, RegexOptions Options), Regex> RegexCache =
		LazyDictionary.Create<(string Pattern, RegexOptions Options), Regex>(_ => new(_.Pattern, _.Options, TimeSpan.FromMinutes(1)));
}
