// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class CharExtensions
	{
		/// <summary>
		/// <see cref="char.IsControl(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsControl(this char @this)
			=> char.IsControl(@this);

		/// <summary>
		/// <see cref="char.IsDigit(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigit(this char @this)
			=> char.IsDigit(@this);

		/// <summary>
		/// <see cref="char.IsHighSurrogate(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsHighSurrogate(this char @this)
			=> char.IsHighSurrogate(@this);

		/// <summary>
		/// <see cref="char.IsLetter(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLetter(this char @this)
			=> char.IsLetter(@this);

		/// <summary>
		/// <see cref="char.IsLetterOrDigit(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLetterOrDigit(this char @this)
			=> char.IsLetterOrDigit(@this);

		/// <summary>
		/// <see cref="char.IsLower(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLower(this char @this)
			=> char.IsLower(@this);

		/// <summary>
		/// <see cref="char.IsLowSurrogate(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLowSurrogate(this char @this)
			=> char.IsLowSurrogate(@this);

		/// <summary>
		/// <see cref="char.IsNumber(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumber(this char @this)
			=> char.IsNumber(@this);

		/// <summary>
		/// <see cref="char.IsPunctuation(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPunctuation(this char @this)
			=> char.IsPunctuation(@this);

		/// <summary>
		/// <see cref="char.IsSeparator(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSeparator(this char @this)
			=> char.IsSeparator(@this);

		/// <summary>
		/// <see cref="char.IsSurrogate(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSurrogate(this char @this)
			=> char.IsSurrogate(@this);

		/// <summary>
		/// <see cref="char.IsSymbol(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSymbol(this char @this)
			=> char.IsSymbol(@this);

		/// <summary>
		/// <see cref="char.IsUpper(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUpper(this char @this)
			=> char.IsUpper(@this);

		/// <summary>
		/// <see cref="char.IsWhiteSpace(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWhiteSpace(this char @this)
			=> char.IsWhiteSpace(@this);

		/// <summary>
		/// <see cref="string.Join(char, string?[])"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params string[] values)
			=> string.Join(@this, values);

		/// <summary>
		/// <see cref="string.Join{T}(char, IEnumerable{T})"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<string> values)
			=> string.Join(@this, values);

		/// <summary>
		/// <see cref="string.Join(char, object?[])"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params object[] values)
			=> string.Join(@this, values);

		/// <summary>
		/// <see cref="string.Join{T}(char, IEnumerable{T})"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<object> values)
			=> string.Join(@this, values);

		/// <summary>
		/// <see cref="char.ToLowerInvariant(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToLower(this char @this)
			=> char.ToLowerInvariant(@this);

		/// <summary>
		/// <see cref="char.ToLower(char, CultureInfo)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToLower(this char @this, CultureInfo culture)
			=> char.ToLower(@this, culture);

		/// <summary>
		/// <see cref="char.GetNumericValue(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToNumber(this char @this)
			=> char.GetNumericValue(@this);

		/// <summary>
		/// <see cref="char.ToUpperInvariant(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToUpper(this char @this)
			=> char.ToUpperInvariant(@this);

		/// <summary>
		/// <see cref="char.ToUpper(char, CultureInfo)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToUpper(this char @this, CultureInfo culture)
			=> char.ToUpper(@this, culture);

		/// <summary>
		/// <see cref="char.GetUnicodeCategory(char)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnicodeCategory ToUnicodeCategory(this char @this)
			=> char.GetUnicodeCategory(@this);
	}
}
