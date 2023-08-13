// Copyright (c) 2021 Samuel Abraham

using System.Globalization;

namespace TypeCache.Extensions;

public static class CharExtensions
{
	/// <inheritdoc cref="char.GetUnicodeCategory(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.GetUnicodeCategory(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static UnicodeCategory GetUnicodeCategory(this char @this)
		=> char.GetUnicodeCategory(@this);

	/// <inheritdoc cref="char.IsAscii(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAscii(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAscii(this char @this)
		=> char.IsAscii(@this);

	/// <inheritdoc cref="char.IsAsciiDigit(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiDigit(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiDigit(this char @this)
		=> char.IsAsciiDigit(@this);

	/// <inheritdoc cref="char.IsAsciiHexDigit(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiHexDigit(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiHexDigit(this char @this)
		=> char.IsAsciiHexDigit(@this);

	/// <inheritdoc cref="char.IsAsciiHexDigitLower(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiHexDigitLower(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiHexDigitLower(this char @this)
		=> char.IsAsciiHexDigitLower(@this);

	/// <inheritdoc cref="char.IsAsciiHexDigitUpper(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiHexDigitUpper(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiHexDigitUpper(this char @this)
		=> char.IsAsciiHexDigitUpper(@this);

	/// <inheritdoc cref="char.IsAsciiLetter(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiLetter(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiLetter(this char @this)
		=> char.IsAsciiLetter(@this);

	/// <inheritdoc cref="char.IsAsciiLetterLower(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiLetterLower(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiLetterLower(this char @this)
		=> char.IsAsciiLetterLower(@this);

	/// <inheritdoc cref="char.IsAsciiLetterOrDigit(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiLetterOrDigit(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiLetterOrDigit(this char @this)
		=> char.IsAsciiLetterOrDigit(@this);

	/// <inheritdoc cref="char.IsAsciiLetterUpper(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsAsciiLetterUpper(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsAsciiLetterUpper(this char @this)
		=> char.IsAsciiLetterUpper(@this);

	/// <inheritdoc cref="char.IsControl(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsControl(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsControl(this char @this)
		=> char.IsControl(@this);

	/// <inheritdoc cref="char.IsDigit(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsDigit(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsDigit(this char @this)
		=> char.IsDigit(@this);

	/// <inheritdoc cref="char.IsHighSurrogate(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsHighSurrogate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsHighSurrogate(this char @this)
		=> char.IsHighSurrogate(@this);

	/// <inheritdoc cref="char.IsLetter(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsLetter(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsLetter(this char @this)
		=> char.IsLetter(@this);

	/// <inheritdoc cref="char.IsLetterOrDigit(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsLetterOrDigit(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsLetterOrDigit(this char @this)
		=> char.IsLetterOrDigit(@this);

	/// <inheritdoc cref="char.IsLower(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsLower(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsLower(this char @this)
		=> char.IsLower(@this);

	/// <inheritdoc cref="char.IsLowSurrogate(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsLowSurrogate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsLowSurrogate(this char @this)
		=> char.IsLowSurrogate(@this);

	/// <inheritdoc cref="char.IsNumber(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsNumber(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNumber(this char @this)
		=> char.IsNumber(@this);

	/// <inheritdoc cref="char.IsPunctuation(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsPunctuation(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsPunctuation(this char @this)
		=> char.IsPunctuation(@this);

	/// <inheritdoc cref="char.IsSeparator(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsSeparator(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsSeparator(this char @this)
		=> char.IsSeparator(@this);

	/// <inheritdoc cref="char.IsSurrogate(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsSurrogate(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsSurrogate(this char @this)
		=> char.IsSurrogate(@this);

	/// <inheritdoc cref="char.IsSymbol(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsSymbol(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsSymbol(this char @this)
		=> char.IsSymbol(@this);

	/// <inheritdoc cref="char.IsUpper(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsUpper(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsUpper(this char @this)
		=> char.IsUpper(@this);

	/// <inheritdoc cref="char.IsWhiteSpace(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.IsWhiteSpace(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsWhiteSpace(this char @this)
		=> char.IsWhiteSpace(@this);

	/// <inheritdoc cref="string.Join(char, string?[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Join(this char @this, params string[] values)
		=> string.Join(@this, values);

	/// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Join(this char @this, IEnumerable<string> values)
		=> string.Join(@this, values);

	/// <inheritdoc cref="string.Join(char, object?[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Join(this char @this, params object[] values)
		=> string.Join(@this, values);

	/// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Join(this char @this, IEnumerable<object> values)
		=> string.Join(@this, values);

	/// <inheritdoc cref="char.ToLower(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.ToLower(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToLower(this char @this)
		=> char.ToLower(@this);

	/// <inheritdoc cref="char.ToLower(char, CultureInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.ToLower(@<paramref name="this"/>, <paramref name="culture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToLower(this char @this, CultureInfo culture)
		=> char.ToLower(@this, culture);

	/// <inheritdoc cref="char.ToLowerInvariant(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.ToLowerInvariant(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToLowerInvariant(this char @this)
		=> char.ToLowerInvariant(@this);

	/// <inheritdoc cref="char.GetNumericValue(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.GetNumericValue(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double ToNumber(this char @this)
		=> char.GetNumericValue(@this);

	/// <inheritdoc cref="char.ToUpper(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.ToUpper(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToUpper(this char @this)
		=> char.ToUpper(@this);

	/// <inheritdoc cref="char.ToUpper(char, CultureInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.ToUpper(@<paramref name="this"/>, <paramref name="culture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToUpper(this char @this, CultureInfo culture)
		=> char.ToUpper(@this, culture);

	/// <inheritdoc cref="char.ToUpperInvariant(char)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="char"/>.ToUpperInvariant(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char ToUpperInvariant(this char @this)
		=> char.ToUpperInvariant(@this);
}
