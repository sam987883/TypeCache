// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class CharExtensions
{
	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsControl(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsControl(this char @this)
		=> char.IsControl(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsDigit(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsDigit(this char @this)
		=> char.IsDigit(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsHighSurrogate(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsHighSurrogate(this char @this)
		=> char.IsHighSurrogate(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsLetter(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsLetter(this char @this)
		=> char.IsLetter(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsLetterOrDigit(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsLetterOrDigit(this char @this)
		=> char.IsLetterOrDigit(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsLower(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsLowercase(this char @this)
		=> char.IsLower(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsLowSurrogate(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsLowSurrogate(this char @this)
		=> char.IsLowSurrogate(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsNumber(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsNumber(this char @this)
		=> char.IsNumber(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsPunctuation(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsPunctuation(this char @this)
		=> char.IsPunctuation(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsSeparator(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsSeparator(this char @this)
		=> char.IsSeparator(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsSurrogate(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsSurrogate(this char @this)
		=> char.IsSurrogate(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsSymbol(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsSymbol(this char @this)
		=> char.IsSymbol(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsUpper(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsUppercase(this char @this)
		=> char.IsUpper(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.IsWhiteSpace(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsWhiteSpace(this char @this)
		=> char.IsWhiteSpace(@this);

	/// <summary>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Join(this char @this, params string[] values)
		=> string.Join(@this, values);

	/// <summary>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Join(this char @this, IEnumerable<string> values)
		=> string.Join(@this, values);

	/// <summary>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Join(this char @this, params object[] values)
		=> string.Join(@this, values);

	/// <summary>
	/// <c>=&gt; <see cref="string"/>.Join(@<paramref name="this"/>, <paramref name="values"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Join(this char @this, IEnumerable<object> values)
		=> string.Join(@this, values);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.ToLowerInvariant(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Char ToLower(this char @this)
		=> char.ToLowerInvariant(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.ToLower(@<paramref name="this"/>, <paramref name="culture"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static char ToLower(this char @this, CultureInfo culture)
		=> char.ToLower(@this, culture);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.GetNumericValue(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static double ToNumber(this char @this)
		=> char.GetNumericValue(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.ToUpperInvariant(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static char ToUpper(this char @this)
		=> char.ToUpperInvariant(@this);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.ToUpper(@<paramref name="this"/>, <paramref name="culture"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static char ToUpper(this char @this, CultureInfo culture)
		=> char.ToUpper(@this, culture);

	/// <summary>
	/// <c>=&gt; <see cref="char"/>.GetUnicodeCategory(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static UnicodeCategory ToUnicodeCategory(this char @this)
		=> char.GetUnicodeCategory(@this);
}
