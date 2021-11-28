// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsDigit(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyDigit(this string @this)
			=> @this.Any(c => c.IsDigit());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsLetter(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyLetter(this string @this)
			=> @this.Any(c => c.IsLetter());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsLetterOrDigit(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyLetterOrDigit(this string @this)
			=> @this.Any(c => c.IsLetterOrDigit());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsLower(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyLowercase(this string @this)
			=> @this.Any(c => c.IsLowercase());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsNumber(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyNumber(this string @this)
			=> @this.Any(c => c.IsNumber());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsPunctuation(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyPunctuation(this string @this)
			=> @this.Any(c => c.IsPunctuation());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsSymbol(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnySymbol(this string @this)
			=> @this.Any(c => c.IsSymbol());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsUpper(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyUppercase(this string @this)
			=> @this.Any(c => c.IsUppercase());

		/// <summary>
		/// <c>@<paramref name="this"/>.Any(<see cref="char.IsWhiteSpace(char)"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool AnyWhiteSpace(this string @this)
			=> @this.Any(c => c.IsWhiteSpace());

		/// <summary>
		/// <code>
		/// <see cref="Convert.TryFromBase64String(string, Span{byte}, out int)"/><br/>
		/// <see cref="Encoding.GetString(ReadOnlySpan{byte})"/>
		/// </code>
		/// </summary>
		public static string FromBase64(this string @this, Encoding encoding)
		{
			Span<byte> span = stackalloc byte[@this.Length];
			return Convert.TryFromBase64String(@this, span, out var count) ? encoding.GetString(span.TrimEnd((byte)0)) : @this;
		}

		/// <summary>
		/// <c><see cref="string.Contains(string, StringComparison)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Has(this string @this, string value, StringComparison comparison = STRING_COMPARISON)
			=> @this.Contains(value, comparison);

		/// <summary>
		/// <c><see cref="StringComparer.Equals(string?, string?)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Is([NotNullWhen(true)] this string? @this, string? value, StringComparison comparison = STRING_COMPARISON)
			=> comparison.ToStringComparer().Equals(@this, value);

		/// <summary>
		/// <c><see cref="string.IsNullOrWhiteSpace(string?)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool IsBlank([NotNullWhen(false)] this string? @this)
			=> string.IsNullOrWhiteSpace(@this);

		/// <summary>
		/// <c><see cref="string.Join(string?, string?[])"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string Join(this string? @this, params string[] values)
			=> string.Join(@this, values);

		/// <summary>
		/// <c><see cref="string.StartsWith(char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Left(this string @this, char text)
			=> @this.StartsWith(text);

		/// <summary>
		/// <c><see cref="string.Substring(int, int)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string Left(this string @this, int length)
			=> @this.Substring(0, length);

		/// <summary>
		/// <c><see cref="string.StartsWith(string, StringComparison)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Left(this string @this, string text, StringComparison comparison = STRING_COMPARISON)
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

		public static string MaskHide(this string @this, char mask = '*', StringComparison comparison = STRING_COMPARISON, params string[] hideTerms)
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

		public static string MaskShow(this string @this, char mask = '*', StringComparison comparison = STRING_COMPARISON, params string[] showTerms)
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

		public static T Parse<T>(this string @this)
			where T : unmanaged
			=> (T)(TypeOf<T>.SystemType switch
			{
				_ when TypeOf<T>.Kind == Kind.Enum => (object)Enum.Parse<T>(@this),
				SystemType.Boolean => (object)bool.Parse(@this),
				SystemType.Char => (object)char.Parse(@this),
				SystemType.SByte => (object)sbyte.Parse(@this),
				SystemType.Int16 => (object)short.Parse(@this),
				SystemType.Int32 => (object)int.Parse(@this),
				SystemType.Int64 => (object)long.Parse(@this),
				SystemType.Byte => (object)sbyte.Parse(@this),
				SystemType.UInt16 => (object)ushort.Parse(@this),
				SystemType.UInt32 => (object)uint.Parse(@this),
				SystemType.UInt64 => (object)ulong.Parse(@this),
				SystemType.Single => (object)float.Parse(@this),
				SystemType.Double => (object)double.Parse(@this),
				SystemType.Decimal => (object)decimal.Parse(@this),
				_ => (object)default(T)
			});

		/// <summary>
		/// <c><see cref="Regex"/>.Escape(@<paramref name="this"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string RegexEscape(this string @this)
			=> Regex.Escape(@this);

		/// <summary>
		/// <c><see cref="Regex"/>.IsMatch(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool RegexIsMatch(this string @this, string pattern, RegexOptions options = REGEX_OPTIONS)
			=> Regex.IsMatch(@this, pattern, options);

		/// <summary>
		/// <c><see cref="Regex"/>.IsMatch(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>, <paramref name="timeout"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool RegexIsMatch(this string @this, string pattern, TimeSpan timeout, RegexOptions options = REGEX_OPTIONS)
			=> Regex.IsMatch(@this, pattern, options, timeout);

		/// <summary>
		/// <c><see cref="Regex"/>.Match(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Match RegexMatch(this string @this, string pattern, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Match(@this, pattern, options);

		/// <summary>
		/// <c><see cref="Regex"/>.Match(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>, <paramref name="timeout"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Match RegexMatch(this string @this, string pattern, TimeSpan timeout, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Match(@this, pattern, options, timeout);

		/// <summary>
		/// <c><see cref="Regex"/>.Matches(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static MatchCollection RegexMatches(this string @this, string pattern, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Matches(@this, pattern, options);

		/// <summary>
		/// <c><see cref="Regex"/>.Matches(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>, <paramref name="timeout"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static MatchCollection RegexMatches(this string @this, string pattern, TimeSpan timeout, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Matches(@this, pattern, options, timeout);

		/// <summary>
		/// <c><see cref="Regex"/>.Replace(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="evaluator"/>, <paramref name="options"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string RegexReplace(this string @this, string pattern, MatchEvaluator evaluator, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Replace(@this, pattern, evaluator, options);

		/// <summary>
		/// <c><see cref="Regex"/>.Replace(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="evaluator"/>, <paramref name="options"/>, <paramref name="timeout"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string RegexReplace(this string @this, string pattern, MatchEvaluator evaluator, TimeSpan timeout, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Replace(@this, pattern, evaluator, options, timeout);

		/// <summary>
		/// <c><see cref="Regex"/>.Replace(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="replacement"/>, <paramref name="options"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string RegexReplace(this string @this, string pattern, string replacement, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Replace(@this, pattern, replacement, options);

		/// <summary>
		/// <c><see cref="Regex"/>.Replace(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="replacement"/>, <paramref name="options"/>, <paramref name="timeout"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string RegexReplace(this string @this, string pattern, string replacement, TimeSpan timeout, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Replace(@this, pattern, replacement, options, timeout);

		/// <summary>
		/// <c><see cref="Regex"/>.Split(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string[] RegexSplit(this string @this, string pattern, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Split(@this, pattern, options);

		/// <summary>
		/// <c><see cref="Regex"/>.Split(@<paramref name="this"/>, <paramref name="pattern"/>, <paramref name="options"/>, <paramref name="timeout"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string[] RegexSplit(this string @this, string pattern, TimeSpan timeout, RegexOptions options = REGEX_OPTIONS)
			=> Regex.Split(@this, pattern, options, timeout);

		/// <summary>
		/// <c><see cref="Regex"/>.Unescape(@<paramref name="this"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string RegexUnescape(this string @this)
			=> Regex.Unescape(@this);

		/// <summary>
		/// <code>
		/// Span&lt;<see cref="char"/>&gt; span = stackalloc <see cref="char"/>[@<paramref name="this"/>.Length];<br/>
		/// @<paramref name="this"/>.AsSpan().CopyTo(span);<br/>
		/// span.Reverse();<br/>
		/// return new <see cref="string"/>(span);
		/// </code>
		/// </summary>
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
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Right(this string @this, char text)
			=> @this.EndsWith(text);

		/// <summary>
		/// <c><see cref="string.EndsWith(string, StringComparison)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Right(this string @this, string text, StringComparison comparison = STRING_COMPARISON)
			=> @this.EndsWith(text, comparison);

		/// <summary>
		/// <code>
		/// <see cref="Encoding.GetBytes(ReadOnlySpan{char}, Span{byte})"/><br/>
		/// <see cref="Convert.TryToBase64Chars(ReadOnlySpan{byte}, Span{char}, out int, Base64FormattingOptions)"/>
		/// </code>
		/// </summary>
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
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static byte[] ToBytes(this string @this, Encoding encoding)
			=> encoding.GetBytes(@this);

		/// <summary>
		/// <c><see cref="Encoding.GetBytes(ReadOnlySpan{char}, Span{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static int ToBytes(this string @this, Encoding encoding, Span<byte> bytes)
			=> encoding.GetBytes(@this, bytes);

		/// <summary>
		/// <c><see cref="Enum.TryParse{TEnum}(string?, bool, out TEnum)"/></c>
		/// </summary>
		public static T? ToEnum<T>(this string? @this)
			where T : struct, Enum
			=> Enum.TryParse(@this, true, out T result) ? (T?)result : null;

		/// <summary>
		/// <c><see cref="Encoding.GetString(byte[])"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string ToText(this byte[] @this, Encoding encoding)
			=> encoding.GetString(@this);

		/// <summary>
		/// <c><see cref="Encoding.GetString(byte[], int, int)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string ToText(this byte[] @this, Encoding encoding, int index, int count)
			=> encoding.GetString(@this, index, count);

		/// <summary>
		/// <c><see cref="Encoding.GetString(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static string ToText(this ReadOnlySpan<byte> @this, Encoding encoding)
			=> encoding.GetString(@this);

		/// <summary>
		/// <code>
		/// !<paramref name="text"/>.IsBlank() &amp;&amp; @<paramref name="this"/>?.Right(<paramref name="text"/>, <paramref name="comparison"/>) <see langword="is true"/><br/>
		/// ? @<paramref name="this"/>.Sunstring(0, @<paramref name="this"/>.Length - <paramref name="text"/>.Length)<br/>
		/// : (@<paramref name="this"/> ?? <see cref="string.Empty"/>)
		/// </code>
		/// </summary>
		public static string TrimEnd(this string @this, string text, StringComparison comparison = STRING_COMPARISON)
			=> !text.IsBlank() && @this?.Right(text, comparison) is true ? @this.Substring(0, @this.Length - text.Length) : (@this ?? string.Empty);

		/// <summary>
		/// <code>
		/// !<paramref name="text"/>.IsBlank() &amp;&amp; @<paramref name="this"/>?.Left(<paramref name="text"/>, <paramref name="comparison"/>) <see langword="is true"/><br/>
		/// ? @<paramref name="this"/>.Sunstring(<paramref name="text"/>.Length)<br/>
		/// : (@<paramref name="this"/> ?? <see cref="string.Empty"/>)
		/// </code>
		/// </summary>
		public static string TrimStart(this string @this, string text, StringComparison comparison = STRING_COMPARISON)
			=> !text.IsBlank() && @this?.Left(text, comparison) is true ? @this.Substring(text.Length) : (@this ?? string.Empty);

		public static T? TryParse<T>(this string @this)
			where T : unmanaged
			=> TypeOf<T>.SystemType switch
			{
				_ when TypeOf<T>.Kind == Kind.Enum => Enum.TryParse<T>(@this, out var value) ? (object)value : null,
				SystemType.Boolean => bool.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Char => char.TryParse(@this, out var value) ? (object)value : null,
				SystemType.SByte => sbyte.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Int16 => short.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Int32 => int.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Int64 => long.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Byte => sbyte.TryParse(@this, out var value) ? (object)value : null,
				SystemType.UInt16 => ushort.TryParse(@this, out var value) ? (object)value : null,
				SystemType.UInt32 => uint.TryParse(@this, out var value) ? (object)value : null,
				SystemType.UInt64 => ulong.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Single => float.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Double => double.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Decimal => decimal.TryParse(@this, out var value) ? (object)value : null,
				_ => null
			} as T?;
	}
}
