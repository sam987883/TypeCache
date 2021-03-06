﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;

namespace TypeCache.Extensions
{
	public static class StringExtensions
	{
		public static string FromBase64(this string @this, Encoding encoding)
		{
			Span<byte> span = stackalloc byte[@this.Length];
			return Convert.TryFromBase64String(@this, span, out var count) ? encoding.GetString(span.TrimEnd((byte)0)) : @this;
		}

		/// <summary>
		/// <c><see cref="string.Contains(string, StringComparison)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(this string @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Contains(value, comparison);

		/// <summary>
		/// <c><see cref="StringComparer.Equals(string?, string?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is([NotNullWhen(true)] this string? @this, string? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> comparison.ToStringComparer().Equals(@this, value);

		/// <summary>
		/// <c><see cref="string.IsNullOrWhiteSpace(string?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlank([NotNullWhen(false)] this string? @this)
			=> string.IsNullOrWhiteSpace(@this);

		/// <summary>
		/// <c><see cref="string.Join(string?, string?[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join(this string? @this, params string[] values)
			=> string.Join(@this, values);

		/// <summary>
		/// <c><see cref="string.Substring(int, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Left(this string @this, int length)
			=> @this.Substring(0, length);

		/// <summary>
		/// <c><see cref="string.StartsWith(char)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Left(this string @this, char text)
			=> @this.StartsWith(text);

		/// <summary>
		/// <c><see cref="string.StartsWith(string, StringComparison)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Left(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.StartsWith(text, comparison);

		public static string Mask(this string @this, char mask = '*')
		{
			if (@this.IsBlank())
				return string.Empty;

			var span = @this.ToSpan();
			var i = -1;
			while (++i < span.Length)
			{
				if (span[i].IsLetterOrDigit())
					span[i] = mask;
			}

			return new string(span);
		}

		public static string MaskHide(this string @this, char mask = '*', StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] hideTerms)
		{
			if (@this.IsBlank() || !hideTerms.Any())
				return @this;

			var count = 0;
			var span = @this.ToSpan();
			var i = -1;
			while (++i < span.Length)
			{
				foreach (var term in hideTerms)
				{
					if (term.Length > count && ((ReadOnlySpan<char>)span[i..]).StartsWith(term.AsSpan(), comparison))
						count = term.Length;
				}

				if (count > 0)
				{
					if (span[i].IsLetterOrDigit())
						span[i] = mask;
					--count;
				}
			}

			return new string(span);
		}

		public static string MaskShow(this string @this, char mask = '*', StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] showTerms)
		{
			if (@this.IsBlank())
				return @this;

			showTerms ??= Array<string>.Empty;

			var count = 0;
			var span = @this.ToSpan();
			var i = -1;
			while (++i < span.Length)
			{
				foreach (var term in showTerms)
				{
					if (term.Length > count && ((ReadOnlySpan<char>)span[i..]).StartsWith(term.AsSpan(), comparison))
						count = term.Length;
				}

				if (count > 0)
					--count;
				else if (span[i].IsLetterOrDigit())
					span[i] = mask;
			}

			return new string(span);
		}

		public static string Reverse(this string @this)
		{
			Span<char> span = stackalloc char[@this.Length];
			@this.AsSpan().CopyTo(span);
			span.Reverse();
			return new string(span);
		}

		/// <summary>
		/// <c><see cref="string.EndsWith(char)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Right(this string @this, char text)
			=> @this.EndsWith(text);

		/// <summary>
		/// <c><see cref="string.EndsWith(string, StringComparison)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Right(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.EndsWith(text, comparison);

		public static string ToBase64(this string @this, Encoding encoding)
		{
			Span<byte> bytes = stackalloc byte[@this.Length * sizeof(int)];
			@this.ToBytes(encoding, bytes);
			Span<char> chars = stackalloc char[bytes.Length * sizeof(int)];
			return Convert.TryToBase64Chars(bytes, chars, out var count) ? new string(chars.Slice(0, count)) : @this;
		}

		/// <summary>
		/// <c><see cref="Encoding.GetBytes(string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToBytes(this string @this, Encoding encoding)
			=> encoding.GetBytes(@this);

		/// <summary>
		/// <c><see cref="Encoding.GetBytes(ReadOnlySpan{char}, Span{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToBytes(this string @this, Encoding encoding, Span<byte> bytes)
			=> encoding.GetBytes(@this, bytes);

		/// <summary>
		/// <c><see cref="Encoding.GetString(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToText(this ReadOnlySpan<byte> @this, Encoding encoding)
			=> encoding.GetString(@this);

		/// <summary>
		/// <c><see cref="Encoding.GetString(byte[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToText(this byte[] @this, Encoding encoding)
			=> encoding.GetString(@this);

		/// <summary>
		/// <c><see cref="Encoding.GetString(byte[], int, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToText(this byte[] @this, Encoding encoding, int index, int count)
			=> encoding.GetString(@this, index, count);

		/// <summary>
		/// <c><see cref="Enum.TryParse{TEnum}(string?, bool, out TEnum)"/></c>
		/// </summary>
		public static T? ToEnum<T>(this string? @this)
			where T : struct, Enum
			=> Enum.TryParse(@this, true, out T result) ? (T?)result : null;

		public static string? TrimEnd(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> !text.IsBlank() && @this?.Right(text, comparison) is true ? @this.Substring(0, @this.Length - text.Length) : @this;

		public static string? TrimStart(this string @this, string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> !text.IsBlank() && @this?.Left(text, comparison) is true ? @this.Substring(text.Length) : @this;
	}
}
