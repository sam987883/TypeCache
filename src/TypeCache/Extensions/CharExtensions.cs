// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class CharExtensions
	{
		/// <summary>
		/// <c><see cref="char.IsControl(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsControl(this char @this)
			=> char.IsControl(@this);

		/// <summary>
		/// <c><see cref="char.IsDigit(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsDigit(this char @this)
			=> char.IsDigit(@this);

		/// <summary>
		/// <c><see cref="char.IsHighSurrogate(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsHighSurrogate(this char @this)
			=> char.IsHighSurrogate(@this);

		/// <summary>
		/// <c><see cref="char.IsLetter(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsLetter(this char @this)
			=> char.IsLetter(@this);

		/// <summary>
		/// <c><see cref="char.IsLetterOrDigit(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsLetterOrDigit(this char @this)
			=> char.IsLetterOrDigit(@this);

		/// <summary>
		/// <c><see cref="char.IsLower(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsLowercase(this char @this)
			=> char.IsLower(@this);

		/// <summary>
		/// <c><see cref="char.IsLowSurrogate(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsLowSurrogate(this char @this)
			=> char.IsLowSurrogate(@this);

		/// <summary>
		/// <c><see cref="char.IsNumber(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsNumber(this char @this)
			=> char.IsNumber(@this);

		/// <summary>
		/// <c><see cref="char.IsPunctuation(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsPunctuation(this char @this)
			=> char.IsPunctuation(@this);

		/// <summary>
		/// <c><see cref="char.IsSeparator(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsSeparator(this char @this)
			=> char.IsSeparator(@this);

		/// <summary>
		/// <c><see cref="char.IsSurrogate(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsSurrogate(this char @this)
			=> char.IsSurrogate(@this);

		/// <summary>
		/// <c><see cref="char.IsSymbol(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsSymbol(this char @this)
			=> char.IsSymbol(@this);

		/// <summary>
		/// <c><see cref="char.IsUpper(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsUppercase(this char @this)
			=> char.IsUpper(@this);

		/// <summary>
		/// <c><see cref="char.IsWhiteSpace(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsWhiteSpace(this char @this)
			=> char.IsWhiteSpace(@this);

		/// <summary>
		/// <c><see cref="string.Join(char, string?[])"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string Join(this char @this, params string[] values)
			=> string.Join(@this, values);

		/// <summary>
		/// <c><see cref="string.Join{T}(char, IEnumerable{T})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string Join(this char @this, IEnumerable<string> values)
			=> string.Join(@this, values);

		/// <summary>
		/// <c><see cref="string.Join(char, object?[])"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string Join(this char @this, params object[] values)
			=> string.Join(@this, values);

		/// <summary>
		/// <c><see cref="string.Join{T}(char, IEnumerable{T})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string Join(this char @this, IEnumerable<object> values)
			=> string.Join(@this, values);

		/// <summary>
		/// <c><see cref="char.ToLowerInvariant(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Char ToLower(this char @this)
			=> char.ToLowerInvariant(@this);

		/// <summary>
		/// <c><see cref="char.ToLower(char, CultureInfo)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static char ToLower(this char @this, CultureInfo culture)
			=> char.ToLower(@this, culture);

		/// <summary>
		/// <c><see cref="char.GetNumericValue(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static double ToNumber(this char @this)
			=> char.GetNumericValue(@this);

		/// <summary>
		/// <c><see cref="char.ToUpperInvariant(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static char ToUpper(this char @this)
			=> char.ToUpperInvariant(@this);

		/// <summary>
		/// <c><see cref="char.ToUpper(char, CultureInfo)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static char ToUpper(this char @this, CultureInfo culture)
			=> char.ToUpper(@this, culture);

		/// <summary>
		/// <c><see cref="char.GetUnicodeCategory(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static UnicodeCategory ToUnicodeCategory(this char @this)
			=> char.GetUnicodeCategory(@this);
	}
}
