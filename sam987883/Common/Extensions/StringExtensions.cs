// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace sam987883.Common.Extensions
{
	public static class StringExtensions
    {
		public static void Assert(this string? @this, string name, string? value)
		{
			name.AssertNotNull(nameof(name));

			if (!string.Equals(@this, value))
				throw new ArgumentException($"{nameof(Assert)}: [{(@this != null ? $"\"{@this}\"" : "null")}] <> {(value != null ? $"\"{value}\"" : "null")}.", name);
		}

		public static void Assert(this string? @this, string name, string? value, StringComparer comparer)
		{
			name.AssertNotNull(nameof(name));
			comparer.AssertNotNull(nameof(comparer));

			if (!comparer.Equals(@this, value))
				throw new ArgumentException($"{nameof(Assert)}: [{(@this != null ? $"\"{@this}\"" : "null")}] <> {(value != null ? $"\"{value}\"" : "null")}.", name);
		}

		public static T? Enum<T>(this string @this, bool caseSensitive = false) where T : struct, Enum =>
			System.Enum.TryParse(@this, !caseSensitive, out T result) ? (T?) result : null;

		public static bool Has(this string @this, string value, bool caseSensitive = false) =>
			@this.Contains(value, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

		public static bool Is(this string @this, string value, bool caseSensitive = false) =>
			caseSensitive ? StringComparer.Ordinal.Equals(@this, value) : StringComparer.OrdinalIgnoreCase.Equals(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlank([NotNullWhen(false)] this string? @this) =>
			string.IsNullOrWhiteSpace(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this string? @this, params string[] values) =>
			string.Join(@this, values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Left(this string @this, int length) =>
			@this.Substring(0, length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Queue<char> Queue(this string @this) =>
			new Queue<char>(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Reverse(this string @this) =>
			new string(@this.Stack().ToArray());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Stack<char> Stack(this string @this) =>
			new Stack<char>(@this);
	}
}
