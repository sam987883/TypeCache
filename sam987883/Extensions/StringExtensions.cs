// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace sam987883.Extensions
{
    public static class StringExtensions
    {
		public static T? Enum<T>(this string @this, bool isCaseSensitive = false) where T : struct, Enum =>
			System.Enum.TryParse(@this, !isCaseSensitive, out T result) ? (T?) result : null;

		public static bool Has(this string @this, string value, bool isCaseSensitive = false) =>
			@this.Contains(value, isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

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
