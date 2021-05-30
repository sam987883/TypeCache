// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class CharExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsControl(this char @this)
			=> char.IsControl(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigit(this char @this)
			=> char.IsDigit(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsHighSurrogate(this char @this)
			=> char.IsHighSurrogate(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLetter(this char @this)
			=> char.IsLetter(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLetterOrDigit(this char @this)
			=> char.IsLetterOrDigit(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLower(this char @this)
			=> char.IsLower(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLowSurrogate(this char @this)
			=> char.IsLowSurrogate(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumber(this char @this)
			=> char.IsNumber(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPunctuation(this char @this)
			=> char.IsPunctuation(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSeparator(this char @this)
			=> char.IsSeparator(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSurrogate(this char @this)
			=> char.IsSurrogate(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSymbol(this char @this)
			=> char.IsSymbol(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUpper(this char @this)
			=> char.IsUpper(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWhiteSpace(this char @this)
			=> char.IsWhiteSpace(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params string[] values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<string> values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, params object[] values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this char @this, IEnumerable<object> values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToLower(this char @this)
			=> char.ToLowerInvariant(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToLower(this char @this, CultureInfo culture)
			=> char.ToLower(@this, culture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToNumber(this char @this)
			=> char.GetNumericValue(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToUpper(this char @this)
			=> char.ToUpperInvariant(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Char ToUpper(this char @this, CultureInfo culture)
			=> char.ToUpper(@this, culture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnicodeCategory UnicodeCategory(this char @this)
			=> char.GetUnicodeCategory(@this);
	}
}
