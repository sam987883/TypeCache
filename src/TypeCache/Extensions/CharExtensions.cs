// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Numerics;

namespace TypeCache.Extensions;

public static class CharExtensions
{
	extension(char @this)
	{
		/// <inheritdoc cref="char.GetUnicodeCategory(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.GetUnicodeCategory(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public UnicodeCategory GetUnicodeCategory()
			=> char.GetUnicodeCategory(@this);

		/// <inheritdoc cref="char.IsAscii(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAscii(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAscii()
			=> char.IsAscii(@this);

		/// <inheritdoc cref="char.IsAsciiDigit(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiDigit(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiDigit()
			=> char.IsAsciiDigit(@this);

		/// <inheritdoc cref="char.IsAsciiHexDigit(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiHexDigit(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiHexDigit()
			=> char.IsAsciiHexDigit(@this);

		/// <inheritdoc cref="char.IsAsciiHexDigitLower(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiHexDigitLower(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiHexDigitLower()
			=> char.IsAsciiHexDigitLower(@this);

		/// <inheritdoc cref="char.IsAsciiHexDigitUpper(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiHexDigitUpper(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiHexDigitUpper()
			=> char.IsAsciiHexDigitUpper(@this);

		/// <inheritdoc cref="char.IsAsciiLetter(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiLetter(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiLetter()
			=> char.IsAsciiLetter(@this);

		/// <inheritdoc cref="char.IsAsciiLetterLower(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiLetterLower(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiLetterLower()
			=> char.IsAsciiLetterLower(@this);

		/// <inheritdoc cref="char.IsAsciiLetterOrDigit(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiLetterOrDigit(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiLetterOrDigit()
			=> char.IsAsciiLetterOrDigit(@this);

		/// <inheritdoc cref="char.IsAsciiLetterUpper(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsAsciiLetterUpper(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsAsciiLetterUpper()
			=> char.IsAsciiLetterUpper(@this);

		/// <inheritdoc cref="char.IsControl(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsControl(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsControl()
			=> char.IsControl(@this);

		/// <inheritdoc cref="char.IsDigit(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsDigit(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsDigit()
			=> char.IsDigit(@this);

		/// <inheritdoc cref="char.IsHighSurrogate(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsHighSurrogate(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsHighSurrogate()
			=> char.IsHighSurrogate(@this);

		/// <inheritdoc cref="char.IsLetter(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsLetter(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsLetter()
			=> char.IsLetter(@this);

		/// <inheritdoc cref="char.IsLetterOrDigit(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsLetterOrDigit(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsLetterOrDigit()
			=> char.IsLetterOrDigit(@this);

		/// <inheritdoc cref="char.IsLower(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsLower(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsLower()
			=> char.IsLower(@this);

		/// <inheritdoc cref="char.IsLowSurrogate(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsLowSurrogate(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsLowSurrogate()
			=> char.IsLowSurrogate(@this);

		/// <inheritdoc cref="char.IsNumber(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsNumber(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsNumber()
			=> char.IsNumber(@this);

		/// <inheritdoc cref="char.IsPunctuation(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsPunctuation(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsPunctuation()
			=> char.IsPunctuation(@this);

		/// <inheritdoc cref="char.IsSeparator(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsSeparator(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsSeparator()
			=> char.IsSeparator(@this);

		/// <inheritdoc cref="char.IsSurrogate(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsSurrogate(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsSurrogate()
			=> char.IsSurrogate(@this);

		/// <inheritdoc cref="char.IsSymbol(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsSymbol(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsSymbol()
			=> char.IsSymbol(@this);

		/// <inheritdoc cref="char.IsUpper(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsUpper(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsUpper()
			=> char.IsUpper(@this);

		/// <inheritdoc cref="char.IsWhiteSpace(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.IsWhiteSpace(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsWhiteSpace()
			=> char.IsWhiteSpace(@this);

		/// <inheritdoc cref="string.Join(char, string?[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Join(@this, <paramref name="values"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Join(string[] values)
			=> string.Join(@this, values);

		/// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Join(@this, <paramref name="values"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Join(IEnumerable<string> values)
			=> string.Join(@this, values);

		/// <inheritdoc cref="string.Join(char, object?[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Join(@this, <paramref name="values"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Join(object[] values)
			=> string.Join(@this, values);

		/// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Join(@this, <paramref name="values"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Join(IEnumerable<object> values)
			=> string.Join(@this, values);

		/// <inheritdoc cref="char.ToLower(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.ToLower(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char ToLower()
			=> char.ToLower(@this);

		/// <inheritdoc cref="char.ToLower(char, CultureInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.ToLower(@this, <paramref name="culture"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char ToLower(CultureInfo culture)
			=> char.ToLower(@this, culture);

		/// <inheritdoc cref="char.ToLowerInvariant(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.ToLowerInvariant(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char ToLowerInvariant()
			=> char.ToLowerInvariant(@this);

		/// <inheritdoc cref="char.GetNumericValue(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.GetNumericValue(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public double ToNumber()
			=> char.GetNumericValue(@this);

		/// <inheritdoc cref="char.ToUpper(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.ToUpper(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char ToUpper()
			=> char.ToUpper(@this);

		/// <inheritdoc cref="char.ToUpper(char, CultureInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.ToUpper(@this, <paramref name="culture"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char ToUpper(CultureInfo culture)
			=> char.ToUpper(@this, culture);

		/// <inheritdoc cref="char.ToUpperInvariant(char)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="char"/>.ToUpperInvariant(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char ToUpperInvariant()
			=> char.ToUpperInvariant(@this);
	}
}
