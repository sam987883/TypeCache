// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;
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
	public static bool AnyUpper(this string @this)
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
	public static bool ContainsAny(this string @this, char[]? chars = null)
		=> chars?.Any(@this.Contains) is true;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsIgnoreCase(this string @this, string value)
		=> @this.Contains(value, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="string.EndsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.EndsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EndsWithIgnoreCase(this string @this, string text)
		=> @this.EndsWith(text, StringComparison.OrdinalIgnoreCase);

	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Equals(@<paramref name="this"/>, <paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualsIgnoreCase([NotNullWhen(true)] this string? @this, [NotNullWhen(true)] string? value)
		=> string.Equals(@this, value, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="Convert.FromBase64String(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.FromBase64String(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] FromBase64(this string @this)
		=> Convert.FromBase64String(@this);

	/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
	public static string FromBase64(this string @this, Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;
		Span<byte> span = stackalloc byte[@this.Length * sizeof(char)];
		return Convert.TryFromBase64String(@this, span, out var count) ? encoding.GetString(span.Slice(0, count)) : @this;
	}

	/// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.IsNullOrWhiteSpace(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsBlank([NotNullWhen(false)] this string? @this)
		=> string.IsNullOrWhiteSpace(@this);

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

	/// <inheritdoc cref="string.IsNullOrEmpty(string?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.IsNullOrEmpty(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? @this)
		=> string.IsNullOrEmpty(@this);

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
	public static string Join(this string? @this, string[] values)
		=> @this.AsSpan().Join(values);

	/// <inheritdoc cref="string.Substring(int, int)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Substring(0, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Left(this string @this, int length)
		=> @this.Substring(0, length);

	/// <summary>
	/// Mask letter or numbers in a string.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Mask(this string @this, char mask = '*', string[]? terms = null)
		=> @this.AsSpan().Mask(mask, terms);

	/// <summary>
	/// Mask letter or numbers in a string.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string MaskIgnoreCase(this string @this, char mask = '*', string[]? terms = null)
		=> @this.AsSpan().MaskIgnoreCase(mask, terms);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsNotBlank() ? @<paramref name="this"/> : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	[return:NotNullIfNotNull("this")]
	public static string? NullIfBlank(this string? @this)
		=> @this.IsNotBlank() ? @this : null;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> != <see cref="string.Empty"/> ? @<paramref name="this"/> : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	[return: NotNullIfNotNull("this")]
	public static string? NullIfEmpty(this string? @this)
		=> @this != string.Empty ? @this : null;

	/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Parse<T>(this string @this, IFormatProvider? formatProvider = null)
		where T : IParsable<T>
		=> T.Parse(@this, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
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

	/// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool StartsWithIgnoreCase(this string @this, string text)
		=> @this.StartsWith(text, StringComparison.OrdinalIgnoreCase);

	/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
	public static string ToBase64(this string @this, Encoding? encoding = null, bool stripPadding = false)
	{
		encoding ??= Encoding.UTF8;
		var length = encoding.GetMaxByteCount(@this.Length) - 1;
		Span<byte> bytes = stackalloc byte[length];
		encoding.GetBytes(@this, bytes);

		Span<char> chars = stackalloc char[length * sizeof(char)];
		return Convert.TryToBase64Chars(bytes, chars, out var count)
			? new(chars.Slice(0, stripPadding ? count - 2 : count))
			: string.Empty;
	}

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

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(<see langword="typeof"/>(<typeparamref name="T"/>), @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ParameterExpression ToParameterExpression<T>(this string @this)
		=> Expression.Parameter(typeof(T), @this);

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(<paramref name="type"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ParameterExpression ToParameterExpression(this string @this, Type type)
		=> Expression.Parameter(type, @this);

	/// <remarks>
	/// <code>
	/// =&gt; RegexCache[(@<paramref name="this"/>, <paramref name="options"/>)];
	/// // _ =&gt; new Regex(@<paramref name="this"/>, <paramref name="options"/>, <see cref="TimeSpan"/>.FromMinutes(1));
	/// </code>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Regex ToRegex([StringSyntax(StringSyntaxAttribute.Regex)] this string @this, RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline)
		=> RegexCache[(@this, options)];

	/// <inheritdoc cref="StringSegment.StringSegment(string?)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="StringSegment"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringSegment ToStringSegment(this string? @this)
		=> new(@this);

	/// <inheritdoc cref="StringSegment.StringSegment(string, int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="StringSegment"/>(@<paramref name="this"/>, <paramref name="offset"/>, <paramref name="count"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringSegment ToStringSegment(this string @this, int offset, int count)
		=> new(@this, offset, count);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsNotBlank() ? <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>) : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Uri? ToUri(this string? @this)
		=> @this.IsNotBlank() ? new(@this) : null;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsNotBlank() ? <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>, <paramref name="kind"/>) : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Uri? ToUri(this string? @this, UriKind kind)
		=> @this.IsNotBlank() ? new(@this, kind) : null;

	public static string TrimEnd(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> text.IsNotNullOrEmpty() && @this.EndsWith(text, comparison) ? @this.Substring(0, @this.Length - text.Length) : @this;

	public static string TrimStart(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> text.IsNotNullOrEmpty() && @this.StartsWith(text, comparison) ? @this.Substring(text.Length) : @this;

	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string, IFormatProvider, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>([NotNullWhen(true)] this string? @this, [MaybeNullWhen(false)] out T value, IFormatProvider? formatProvider = null)
		where T : IParsable<T>
		=> T.TryParse(@this, formatProvider ?? InvariantCulture, out value);

	/// <inheritdoc cref="INumberBase{TSelf}.TryParse(string, NumberStyles, IFormatProvider, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	/// <param name="formatProvider">Defaults to <see cref="InvariantCulture"/></param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>([NotNullWhen(true)] this string? @this, NumberStyles style, [MaybeNullWhen(false)] out T value, IFormatProvider? formatProvider = null)
		where T : INumberBase<T>
		=> T.TryParse(@this, style, formatProvider ?? InvariantCulture, out value);

	private static readonly IReadOnlyDictionary<(string Pattern, RegexOptions Options), Regex> RegexCache =
		new LazyDictionary<(string Pattern, RegexOptions Options), Regex>(_ => new(_.Pattern, _.Options, TimeSpan.FromMinutes(1)));
}
