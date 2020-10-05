// Copyright (c) 2020 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sam987883.Common.Extensions
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Enum<T>(this string? @this, bool compareCase = false) where T : struct, Enum
			=> System.Enum.TryParse(@this, !compareCase, out T result) ? (T?)result : null;

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
	}
}
