// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using TypeCache.Collections;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class StringExtensions
{
	/// <inheritdoc cref="char.IsDigit(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsDigit());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyDigit(this string @this)
		=> @this.Any(c => c.IsDigit());

	/// <inheritdoc cref="char.IsLetter(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsLetter());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyLetter(this string @this)
		=> @this.Any(c => c.IsLetter());

	/// <inheritdoc cref="char.IsLetterOrDigit(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsLetterOrDigit());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyLetterOrDigit(this string @this)
		=> @this.Any(c => c.IsLetterOrDigit());

	/// <inheritdoc cref="char.IsLower(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsLower());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyLower(this string @this)
		=> @this.Any(c => c.IsLower());

	/// <inheritdoc cref="char.IsNumber(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsNumber());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyNumber(this string @this)
		=> @this.Any(c => c.IsNumber());

	/// <inheritdoc cref="char.IsPunctuation(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsPunctuation());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyPunctuation(this string @this)
		=> @this.Any(c => c.IsPunctuation());

	/// <inheritdoc cref="char.IsSymbol(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsSymbol());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnySymbol(this string @this)
		=> @this.Any(c => c.IsSymbol());

	/// <inheritdoc cref="char.IsUpper(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsUpper());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyUppercase(this string @this)
		=> @this.Any(c => c.IsUpper());

	/// <inheritdoc cref="char.IsWhiteSpace(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsWhiteSpace());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool AnyWhiteSpace(this string @this)
		=> @this.Any(c => c.IsWhiteSpace());

	/// <remarks>
	/// <c>=&gt; <paramref name="chars"/>?.Any(@<paramref name="this"/>.Contains) <see langword="is true"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsAny(this string @this, params char[] chars)
		=> chars?.Any(@this.Contains) is true;

	/// <inheritdoc cref="Convert.FromBase64String(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.FromBase64String(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] FromBase64(this string @this)
		=> Convert.FromBase64String(@this);

	public static string FromBase64(this string @this, Encoding encoding)
	{
		Span<byte> span = stackalloc byte[@this.Length * sizeof(char)];
		return Convert.TryFromBase64String(@this, span, out var count) ? encoding.GetString(span.Slice(0, count)) : @this;
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Has(this string @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> @this.Contains(value, comparison);

	/// <remarks>
	/// <c>=&gt; <paramref name="comparison"/>.ToStringComparer().Equals(@<paramref name="this"/>, <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is(this string? @this, string? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> comparison.ToStringComparer().Equals(@this, value);

	/// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.IsNullOrWhiteSpace(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsBlank([NotNullWhen(false)] this string? @this)
		=> string.IsNullOrWhiteSpace(@this);

	/// <inheritdoc cref="string.IsNullOrEmpty(string?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.IsNullOrEmpty(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? @this)
		=> string.IsNullOrEmpty(@this);

	/// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
	/// <remarks>
	/// <c>=&gt; !<see cref="string"/>.IsNullOrWhiteSpace(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNotBlank([NotNullWhen(true)] this string? @this)
		=> !string.IsNullOrWhiteSpace(@this);

	/// <inheritdoc cref="string.IsNullOrEmpty(string?)"/>
	/// <remarks>
	/// <c>=&gt; !<see cref="string"/>.IsNullOrEmpty(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? @this)
		=> !string.IsNullOrEmpty(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.AsSpan().Join(<paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Join(this string? @this, IEnumerable<string> values)
		=> @this.AsSpan().Join(values);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.AsSpan().Join(<paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Join(this string? @this, params string[] values)
		=> @this.AsSpan().Join(values);

	/// <inheritdoc cref="string.StartsWith(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Left(this string @this, char text)
		=> @this.StartsWith(text);

	/// <inheritdoc cref="string.Substring(int, int)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="length"/> &gt; -1 ? @<paramref name="this"/>.Substring(0, <paramref name="length"/>) : @<paramref name="this"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Left(this string @this, int length)
		=> length > -1 ? @this.Substring(0, length) : @this;

	/// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Left(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> @this.StartsWith(text, comparison);

	public static string Mask(this string @this, char mask = '*')
	{
		if (@this.IsBlank())
			return string.Empty;

		Span<char> span = stackalloc char[@this.Length];
		@this.AsSpan().CopyTo(span);
		var i = -1;
		while (++i < span.Length)
			if (span[i].IsLetterOrDigit())
				span[i] = mask;

		return new string(span);
	}

	public static string MaskHide(this string @this, char mask = '*', StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] hideTerms)
	{
		if (@this.IsBlank() || !hideTerms.Any())
			return @this;

		var count = 0;
		Span<char> span = stackalloc char[@this.Length];
		@this.AsSpan().CopyTo(span);
		var i = -1;
		while (++i < span.Length)
		{
			foreach (var term in hideTerms)
			{
				if (term.Length > count && ((ReadOnlySpan<char>)span[i..]).StartsWith(term.AsSpan(), comparison))
					count = term.Length;
			}

			if (count > 0)
			{
				if (span[i].IsLetterOrDigit())
					span[i] = mask;
				--count;
			}
		}

		return new string(span);
	}

	public static string MaskShow(this string @this, char mask = '*', StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] showTerms)
	{
		if (@this.IsBlank())
			return @this;

		showTerms ??= Array<string>.Empty;

		var count = 0;
		Span<char> span = stackalloc char[@this.Length];
		@this.AsSpan().CopyTo(span);
		var i = -1;
		while (++i < span.Length)
		{
			foreach (var term in showTerms)
			{
				if (term.Length > count && ((ReadOnlySpan<char>)span[i..]).StartsWith(term.AsSpan(), comparison))
					count = term.Length;
			}

			if (count > 0)
				--count;
			else if (span[i].IsLetterOrDigit())
				span[i] = mask;
		}

		return new string(span);
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsNotBlank() ? @<paramref name="this"/> : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	[return:NotNullIfNotNull("this")]
	public static string? NullIfBlank(this string? @this)
		=> @this.IsNotBlank() ? @this : null;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsNotEmpty() ? @<paramref name="this"/> : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	[return: NotNullIfNotNull("this")]
	public static string? NullIfEmpty(this string? @this)
		=> @this.IsNotNullOrEmpty() ? @this : null;

	/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Parse<T>(this string @this, IFormatProvider? formatProvider = null)
		where T : IParsable<T>
		=> T.Parse(@this, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Parse<T>(this string @this, NumberStyles style, IFormatProvider? formatProvider = null)
		where T : INumberBase<T>
		=> T.Parse(@this, style, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="Regex.Escape(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Regex"/>.Escape(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string RegexEscape(this string @this)
		=> Regex.Escape(@this);

	/// <inheritdoc cref="Regex.Unescape(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Regex"/>.Unescape(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string RegexUnescape(this string @this)
		=> Regex.Unescape(@this);

	public static string Reverse(this string @this)
	{
		Span<char> span = stackalloc char[@this.Length];
		@this.AsSpan().CopyTo(span);
		span.Reverse();
		return new string(span);
	}

	/// <inheritdoc cref="string.EndsWith(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.EndsWith(<paramref name="text"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Right(this string @this, char text)
		=> @this.EndsWith(text);

	/// <inheritdoc cref="string.EndsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.EndsWith(<paramref name="text"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Right(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> @this.EndsWith(text, comparison);

	public static string ToBase64(this string @this, Encoding encoding, bool stripPadding = false)
	{
		Span<byte> bytes = stackalloc byte[encoding.GetMaxByteCount(@this.Length) - 1];
		@this.ToBytes(encoding, bytes);

		Span<char> chars = stackalloc char[bytes.Length * sizeof(char)];
		return Convert.TryToBase64Chars(bytes, chars, out var count)
			? new string(chars.Slice(0, stripPadding ? count - 2 : count))
			: string.Empty;
	}

	/// <inheritdoc cref="Encoding.GetBytes(string)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] ToBytes(this string @this, Encoding encoding)
		=> encoding.GetBytes(@this);

	/// <inheritdoc cref="Encoding.GetBytes(ReadOnlySpan{char}, Span{byte})"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetBytes(@<paramref name="this"/>, <paramref name="bytes"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int ToBytes(this string @this, Encoding encoding, Span<byte> bytes)
		=> encoding.GetBytes(@this, bytes);

	/// <inheritdoc cref="Enum.TryParse{TEnum}(string?, bool, out TEnum)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>.TryParse(@<paramref name="this"/>, <paramref name="ignoreCase"/>, <see langword="out"/> <typeparamref name="T"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
	/// </remarks>
	[DebuggerHidden]
	public static T? ToEnum<T>(this string? @this, bool ignoreCase = true)
		where T : struct, Enum
		=> Enum.TryParse(@this, ignoreCase, out T result) ? (T?)result : null;

	/// <inheritdoc cref="Expression.Label(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LabelTarget ToLabelTarget(this string? @this)
		=> Expression.Label(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>) : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Uri? ToUri(this string? @this)
		=> @this is not null ? new Uri(@this) : null;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>, <paramref name="kind"/>) : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Uri? ToUri(this string? @this, UriKind kind)
		=> @this is not null ? new Uri(@this, kind) : null;

	public static string TrimEnd(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> text.IsNotBlank() && @this?.Right(text, comparison) is true ? @this.Substring(0, @this.Length - text.Length) : (@this ?? string.Empty);

	public static string TrimStart(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> text.IsNotBlank() && @this?.Left(text, comparison) is true ? @this.Substring(text.Length) : (@this ?? string.Empty);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>([NotNullWhen(true)] this string? @this, [MaybeNullWhen(false)] out T value)
		where T : ISpanParsable<T>
		=> T.TryParse(@this, InvariantCulture, out value);

	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string, IFormatProvider, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>([NotNullWhen(true)] this string? @this, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : IParsable<T>
		=> T.TryParse(@this, formatProvider ?? InvariantCulture, out value);

	/// <inheritdoc cref="INumberBase{TSelf}.TryParse(string, NumberStyles, IFormatProvider, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>([NotNullWhen(true)] this string? @this, NumberStyles style, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : INumberBase<T>
		=> T.TryParse(@this, style, formatProvider ?? InvariantCulture, out value);
}
