﻿// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using TypeCache.Utilities;
using static System.Globalization.CultureInfo;
using static System.StringSplitOptions;

namespace TypeCache.Extensions;

public static class StringExtensions
{
	private const char FROM_END = '^';
	private const string RANGE_OPERATOR = "..";

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

	/// <inheritdoc cref="string.Contains(char, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsIgnoreCase(this string @this, char value)
		=> @this.Contains(value, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="string.Contains(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsIgnoreCase(this string @this, string value)
		=> @this.Contains(value, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="string.Contains(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see cref="StringComparison.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsOrdinal(this string @this, string value)
		=> @this.Contains(value, StringComparison.Ordinal);

	/// <inheritdoc cref="string.EndsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.EndsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EndsWithIgnoreCase(this string @this, string text)
		=> @this.EndsWith(text, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="string.EndsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.EndsWith(<paramref name="text"/>, <see cref="StringComparison.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EndsWithOrdinal(this string @this, string text)
		=> @this.EndsWith(text, StringComparison.Ordinal);

	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Equals(@<paramref name="this"/>, <paramref name="value"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualsIgnoreCase([NotNullWhen(true)] this string? @this, [NotNullWhen(true)] string? value)
		=> string.Equals(@this, value, StringComparison.OrdinalIgnoreCase);

	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Equals(@<paramref name="this"/>, <paramref name="value"/>, <see cref="StringComparison.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualsOrdinal([NotNullWhen(true)] this string? @this, [NotNullWhen(true)] string? value)
		=> string.Equals(@this, value, StringComparison.Ordinal);

	/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
	public static string FromBase64(this string @this, Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;
		return encoding.GetString(Convert.FromBase64String(@this));
	}

	/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
	public static string FromBase64Url(this string @this, Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;
		return encoding.GetString(Base64Url.DecodeFromChars(@this));
	}

	/// <inheritdoc cref="string.GetHashCode(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetHashCode(<see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int GetHashCodeIgnoreCase(this string @this)
		=> @this.GetHashCode(StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="string.GetHashCode(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetHashCode(<see cref="StringComparison.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int GetHashCodeOrdinal(this string @this)
		=> @this.GetHashCode(StringComparison.Ordinal);

	/// <inheritdoc cref="Base64.IsValid(ReadOnlySpan{char})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Base64"/>.IsValid(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsBase64([NotNullWhen(false)] this string? @this)
		=> Base64.IsValid(@this);

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
		=> @this is not "" ? @this : null;

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

	/// <inheritdoc cref="string.Split(char, StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separator"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, char separator)
		=> @this.Split(separator, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.Split(char, int, StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separator"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, char separator, int count)
		=> @this.Split(separator, count, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.Split(char[], StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separators"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, char[] separators)
		=> @this.Split(separators, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.Split(char[], int, StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separators"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, char[] separators, int count)
		=> @this.Split(separators, count, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.Split(string, StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separator"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, string separator)
		=> @this.Split(separator, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.Split(string, int, StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separator"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, string separator, int count)
		=> @this.Split(separator, count, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.Split(string[], StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separators"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, string[] separators)
		=> @this.Split(separators, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.Split(string[], int, StringSplitOptions)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Split(<paramref name="separators"/>, <paramref name="count"/>, <see cref="RemoveEmptyEntries"/> | <see cref="TrimEntries"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] SplitEx(this string @this, string[] separators, int count)
		=> @this.Split(separators, count, RemoveEmptyEntries | TrimEntries);

	/// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool StartsWithIgnoreCase(this string @this, string text)
		=> @this.StartsWith(text, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <see cref="StringComparison.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool StartsWithOrdinal(this string @this, string text)
		=> @this.StartsWith(text, StringComparison.Ordinal);

	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="string"/>(@<paramref name="this"/>.Take(<paramref name="range"/>).ToArray());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Substring(this string @this, Range range)
		=> new string(@this.Take(range).ToArray());

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfBlank([NotNull] this string? @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
	{
		if (@this.IsBlank())
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: argument,
				actualValue: @this,
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfBlank)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
	public static string ToBase64(this string @this, Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;
		var length = encoding.GetMaxByteCount(@this.Length) - 1;
		Span<byte> bytes = stackalloc byte[length];

		return encoding.TryGetBytes(@this, bytes, out length)
			? bytes.Slice(0, length).AsReadOnly().ToBase64()
			: string.Empty;
	}

	/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
	public static string ToBase64Url(this string @this, Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;
		var length = encoding.GetMaxByteCount(@this.Length) - 1;
		Span<byte> bytes = stackalloc byte[length];

		return encoding.TryGetBytes(@this, bytes, out length)
			? bytes.Slice(0, length).AsReadOnly().ToBase64Url()
			: string.Empty;
	}

	/// <inheritdoc cref="Enum.TryParse{TEnum}(string?, out TEnum)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>.TryParse(@<paramref name="this"/>, <see langword="out"/> <see langword="var"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
	/// </remarks>
	[DebuggerHidden]
	public static T? ToEnum<T>(this string? @this)
		where T : struct, Enum
		=> Enum.TryParse<T>(@this, out var result) ? (T?)result : null;

	/// <inheritdoc cref="Enum.TryParse{TEnum}(string?, bool, out TEnum)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>.TryParse(@<paramref name="this"/>, <see langword="true"/>, <see langword="out"/> <see langword="var"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
	/// </remarks>
	[DebuggerHidden]
	public static T? ToEnumIgnoreCase<T>(this string? @this)
		where T : struct, Enum
		=> Enum.TryParse<T>(@this, true, out var result) ? (T?)result : null;

	/// <summary>
	/// <code>
	/// "^2" ---&gt; ^2
	/// "1"  ---&gt; 1
	/// </code>
	/// </summary>
	public static Index? ToIndex(this string @this)
		=> @this switch
		{
			null or "" => null,
			_ when @this.StartsWith(FROM_END) && int.TryParse(@this.Substring(1), out var index) => Index.FromEnd(index),
			_ when int.TryParse(@this, out var index) => Index.FromStart(index),
			_ => null
		};

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

	/// <summary>
	/// <code>
	/// "1..^2" ---&gt; 1..^2
	/// "..^1"  ---&gt; 0..^1
	/// "1.."   ---&gt; 1..^0
	/// "3"     ---&gt; 3..4
	/// </code>
	/// </summary>
	public static Range? ToRange(this string @this)
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
			var token = @this.Substring(0..^2);
			index = token.ToIndex();
			return index.HasValue ? Range.StartAt(index.Value) : null;
		}

		if (@this.StartsWithIgnoreCase(RANGE_OPERATOR))
		{
			var token = @this.Substring(RANGE_OPERATOR.Length);
			index = token.ToIndex();
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

	/// <remarks>
	/// <code>
	/// =&gt; RegexCache[(@<paramref name="this"/>, <paramref name="options"/>)]; // <see langword="new"/> Regex(@<paramref name="this"/>, <paramref name="options"/>, <see cref="TimeSpan"/>.FromMinutes(1));
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Regex ToRegex([StringSyntax(StringSyntaxAttribute.Regex)] this string @this,
		RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline)
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

	/// <inheritdoc cref="Uri.Uri(string)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Uri ToUri(this string @this)
		=> new(@this);

	/// <inheritdoc cref="Uri.Uri(string, UriKind)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>, <paramref name="kind"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Uri ToUri(this string @this, UriKind kind)
		=> new(@this, kind);

	public static string TrimEnd(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> @this.EndsWith(text, comparison) ? @this.Substring(0, @this.Length - text.Length) : @this;

	public static string TrimStart(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		=> @this.StartsWith(text, comparison) ? @this.Substring(text.Length) : @this;

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

	/// <inheritdoc cref="Convert.TryFromBase64String(string, Span{byte}, out int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.TryFromBase64String(@<paramref name="this"/>, <paramref name="bytes"/>, <see langword="out"/> <paramref name="bytesWritten"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryFromBase64(this string @this, Span<byte> bytes, out int bytesWritten)
		=> Convert.TryFromBase64Chars(@this, bytes, out bytesWritten);

	private static readonly IReadOnlyDictionary<(string Pattern, RegexOptions Options), Regex> RegexCache =
		new LazyDictionary<(string Pattern, RegexOptions Options), Regex>(_ => new(_.Pattern, _.Options, TimeSpan.FromMinutes(1)));
}
