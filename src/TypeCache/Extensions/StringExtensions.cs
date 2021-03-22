// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.Collections.Extensions;

namespace TypeCache.Extensions
{
	public static class StringExtensions
	{
		private static StringComparer GetStringComparer(bool compareCase)
			=> compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

		private static StringComparison GetStringComparison(bool compareCase)
			=> compareCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

		public static void Assert(this string? @this, string name, string? value, bool compareCase = false, [CallerMemberName] string caller = null)
		{
			name.AssertNotNull(nameof(name), caller);

			if (!GetStringComparer(compareCase).Equals(@this, value))
				throw new ArgumentException($"{nameof(Assert)}: [{(@this is not null ? $"\"{@this}\"" : "null")}] <> {(value is not null ? $"\"{value}\"" : "null")}.", name);
		}

		public static void AssertNotBlank([AllowNull] this string @this, string name, [CallerMemberName] string caller = null)
		{
			if (@this is null)
				throw new ArgumentNullException($"{caller} -> {nameof(AssertNotBlank)}: [{name}] is blank.");
		}

		public static T? Enum<T>(this string? @this, bool compareCase = false) where T : struct, Enum
			=> System.Enum.TryParse(@this, !compareCase, out T result) ? (T?)result : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FromBase64(this string @this, Encoding encoding)
			=> encoding.GetString(Convert.FromBase64String(@this));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(this string @this, string value, bool compareCase = false)
			=> @this.Contains(value, GetStringComparison(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(this string? @this, string value, bool compareCase = false)
			=> GetStringComparer(compareCase).Equals(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlank([NotNullWhen(false)] this string? @this)
			=> string.IsNullOrWhiteSpace(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this string? @this, params string[] values)
			=> string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Left(this string @this, int length)
			=> @this.Substring(0, length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Left(this string @this, string text, bool compareCase = false)
			=> @this.StartsWith(text, GetStringComparison(compareCase));

		public static string Reverse(this string @this)
		{
			var span = @this.ToCharArray().AsSpan();
			span.Reverse();
			return new string(span);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Right(this string @this, string text, bool compareCase = false)
			=> @this.EndsWith(text, GetStringComparison(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToBase64(this string @this, Encoding encoding)
			=> Convert.ToBase64String(@this.ToBytes(encoding));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this string @this, Encoding encoding)
			=> encoding.GetBytes(@this);
	}
}
