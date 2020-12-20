// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace TypeCache.Extensions
{
	public static class StringExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assert(this string? @this, string name, string? value, bool compareCase = false)
			=> @this.Assert(name, value, compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

		public static void Assert(this string? @this, string name, string? value, StringComparer comparer)
		{
			name.AssertNotNull(nameof(name));
			comparer.AssertNotNull(nameof(comparer));

			if (!comparer.Equals(@this, value))
				throw new ArgumentException($"{nameof(Assert)}: [{(@this != null ? $"\"{@this}\"" : "null")}] <> {(value != null ? $"\"{value}\"" : "null")}.", name);
		}

		public static void AssertNotBlank([AllowNull] this string @this, string name)
		{
			if (@this == null)
				throw new ArgumentNullException($"{nameof(AssertNotBlank)}: [{name}] is blank.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Enum<T>(this string? @this, bool compareCase = false) where T : struct, Enum
			=> System.Enum.TryParse(@this, !compareCase, out T result) ? (T?)result : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FromBase64(this string @this, Encoding encoding)
			=> encoding.GetString(Convert.FromBase64String(@this));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(this string @this, string value, bool compareCase = false)
			=> @this.Contains(value, compareCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(this string @this, string value, bool compareCase = false)
			=> compareCase ? StringComparer.Ordinal.Equals(@this, value) : StringComparer.OrdinalIgnoreCase.Equals(@this, value);

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
		public static string Reverse(this string @this)
			=> new string(@this.ToStack().ToArray());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToBase64(this string @this, Encoding encoding)
			=> Convert.ToBase64String(@this.ToBytes(encoding));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this string @this, Encoding encoding)
			=> encoding.GetBytes(@this);
	}
}
