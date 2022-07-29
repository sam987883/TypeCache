// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using static TypeCache.Default;
using Unsafe = TypeCache.Reflection.Unsafe;

namespace TypeCache.Extensions;

public static class StringExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsDigit());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyDigit(this string @this)
		=> @this.Any(c => c.IsDigit());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsLetter());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyLetter(this string @this)
		=> @this.Any(c => c.IsLetter());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsLetterOrDigit());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyLetterOrDigit(this string @this)
		=> @this.Any(c => c.IsLetterOrDigit());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsLowercase());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyLowercase(this string @this)
		=> @this.Any(c => c.IsLowercase());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsNumber());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyNumber(this string @this)
		=> @this.Any(c => c.IsNumber());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsPunctuation());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyPunctuation(this string @this)
		=> @this.Any(c => c.IsPunctuation());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsSymbol());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnySymbol(this string @this)
		=> @this.Any(c => c.IsSymbol());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsUppercase());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyUppercase(this string @this)
		=> @this.Any(c => c.IsUppercase());

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(c =&gt; c.IsWhiteSpace());</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyWhiteSpace(this string @this)
		=> @this.Any(c => c.IsWhiteSpace());

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Edit(this string @this, StringEditor edit)
		=> edit(@this);

	/// <inheritdoc cref="Convert.FromBase64String(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.FromBase64String(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] FromBase64(this string @this)
		=> Convert.FromBase64String(@this);

	/// <remarks>
	/// <code>
	/// Span&lt;<see cref="byte"/>&gt; span = <see langword="stackalloc"/> <see cref="byte"/>[@<paramref name="this"/>.Length * 4];<br/>
	/// <see langword="return"/> <see cref="Convert"/>.TryFromBase64String(@<paramref name="this"/>, span, <see langword="out var"/> count) ? <paramref name="encoding"/>.GetString(span.Slice(0, count)) : @<paramref name="this"/>;
	/// </code>
	/// </remarks>
	public static string FromBase64(this string @this, Encoding encoding)
	{
		Span<byte> span = stackalloc byte[@this.Length * sizeof(char)];
		return Convert.TryFromBase64String(@this, span, out var count) ? encoding.GetString(span.Slice(0, count)) : @this;
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Has(this string @this, string value, StringComparison comparison = STRING_COMPARISON)
		=> @this.Contains(value, comparison);

	/// <remarks>
	/// <c>=&gt; <paramref name="comparison"/>.ToStringComparer().Equals(@<paramref name="this"/>, <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Is(this string? @this, string? value, StringComparison comparison = STRING_COMPARISON)
		=> comparison.ToStringComparer().Equals(@this, value);

	/// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.IsNullOrWhiteSpace(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsBlank([NotNullWhen(false)] this string? @this)
		=> string.IsNullOrWhiteSpace(@this);

	/// <inheritdoc cref="string.IsNullOrEmpty(string?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.IsNullOrEmpty(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsEmpty([NotNullWhen(false)] this string? @this)
		=> string.IsNullOrEmpty(@this);

	/// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
	/// <remarks>
	/// <c>=&gt; !<see cref="string"/>.IsNullOrWhiteSpace(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsNotBlank([NotNullWhen(true)] this string? @this)
		=> !string.IsNullOrWhiteSpace(@this);

	/// <inheritdoc cref="string.IsNullOrEmpty(string?)"/>
	/// <remarks>
	/// <c>=&gt; !<see cref="string"/>.IsNullOrEmpty(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsNotEmpty([NotNullWhen(true)] this string? @this)
		=> !string.IsNullOrEmpty(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.AsSpan().Join(<paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Join(this string? @this, IEnumerable<string> values)
		=> @this.AsSpan().Join(values);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.AsSpan().Join(<paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Join(this string? @this, params string[] values)
		=> @this.AsSpan().Join(values);

	/// <inheritdoc cref="string.StartsWith(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Left(this string @this, char text)
		=> @this.StartsWith(text);

	/// <inheritdoc cref="string.Substring(int, int)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Substring(0, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Left(this string @this, int length)
		=> @this.Substring(0, length);

	/// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
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

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsNotBlank() ? @<paramref name="this"/> : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	[return:NotNullIfNotNull("this")]
	public static string? NullIfBlank(this string? @this)
		=> @this.IsNotBlank() ? @this : null;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsNotEmpty() ? @<paramref name="this"/> : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	[return: NotNullIfNotNull("this")]
	public static string? NullIfEmpty(this string? @this)
		=> @this.IsNotEmpty() ? @this : null;

	/// <remarks>
	/// <code>
	/// =&gt; <see cref="TypeOf{T}.SystemType"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>_ <see langword="when"/> <see cref="TypeOf{T}.Kind"/> == <see cref="Kind.Enum"/> &amp;&amp; <see cref="Enum"/>.TryParse&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; value,<br/>
	/// <see langword="    "/><see cref="SystemType.Boolean"/> <see langword="when"/> <see cref="bool"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="   "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="bool"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Char"/> <see langword="   when"/> <see cref="char"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="   "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="char"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.SByte"/> <see langword="  when"/> <see cref="sbyte"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="  "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="sbyte"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Int16"/> <see langword="  when"/> <see cref="short"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="  "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="short"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Int32"/> <see langword="  when"/> <see cref="int"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="    "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="int"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Int64"/> <see langword="  when"/> <see cref="long"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="   "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="long"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Byte"/> <see langword="   when"/> <see cref="byte"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="   "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="byte"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.UInt16"/> <see langword=" when"/> <see cref="ushort"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword=" "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="ushort"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.UInt32"/> <see langword=" when"/> <see cref="uint"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="   "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="uint"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.UInt64"/> <see langword=" when"/> <see cref="ulong"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="  "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="ulong"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Single"/> <see langword=" when"/> <see cref="float"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword="  "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="float"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Double"/> <see langword=" when"/> <see cref="double"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) <see langword=" "/>=&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="double"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/><see cref="SystemType.Decimal"/> <see langword="when"/> <see cref="decimal"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; <see cref="Unsafe"/>.Convert&lt;<see cref="decimal"/>, <typeparamref name="T"/>&gt;(value),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// } <see langword="as"/> <typeparamref name="T"/>?;
	/// </code>
	/// </remarks>
	public static T? Parse<T>(this string @this)
		where T : unmanaged
		=> TypeOf<T>.SystemType switch
		{
			_ when TypeOf<T>.Kind == Kind.Enum && Enum.TryParse<T>(@this, out var value) => (T?)value,
			SystemType.Boolean when bool.TryParse(@this, out var value) => Unsafe.Convert<bool, T>(value),
			SystemType.Char when char.TryParse(@this, out var value) => Unsafe.Convert<char, T>(value),
			SystemType.SByte when sbyte.TryParse(@this, out var value) => Unsafe.Convert<sbyte, T>(value),
			SystemType.Int16 when short.TryParse(@this, out var value) => Unsafe.Convert<short, T>(value),
			SystemType.Int32 when int.TryParse(@this, out var value) => Unsafe.Convert<int, T>(value),
			SystemType.Int64 when long.TryParse(@this, out var value) => Unsafe.Convert<long, T>(value),
			SystemType.Byte when sbyte.TryParse(@this, out var value) => Unsafe.Convert<sbyte, T>(value),
			SystemType.UInt16 when ushort.TryParse(@this, out var value) => Unsafe.Convert<ushort, T>(value),
			SystemType.UInt32 when uint.TryParse(@this, out var value) => Unsafe.Convert<uint, T>(value),
			SystemType.UInt64 when ulong.TryParse(@this, out var value) => Unsafe.Convert<ulong, T>(value),
			SystemType.Single when float.TryParse(@this, out var value) => Unsafe.Convert<float, T>(value),
			SystemType.Double when double.TryParse(@this, out var value) => Unsafe.Convert<double, T>(value),
			SystemType.Decimal when decimal.TryParse(@this, out var value) => Unsafe.Convert<decimal, T>(value),
			_ => null
		};

	/// <inheritdoc cref="Regex.Escape(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Regex"/>.Escape(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string RegexEscape(this string @this)
		=> Regex.Escape(@this);

	/// <inheritdoc cref="Regex.Unescape(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Regex"/>.Unescape(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string RegexUnescape(this string @this)
		=> Regex.Unescape(@this);

	/// <remarks>
	/// <code>
	/// Span&lt;<see cref="char"/>&gt; span = <see langword="stackalloc"/> <see cref="char"/>[@<paramref name="this"/>.Length];<br/>
	/// @<paramref name="this"/>.AsSpan().CopyTo(span);<br/>
	/// span.Reverse();<br/>
	/// <see langword="return new"/> <see cref="string"/>(span);
	/// </code>
	/// </remarks>
	public static string Reverse(this string @this)
	{
		Span<char> span = stackalloc char[@this.Length];
		@this.AsSpan().CopyTo(span);
		span.Reverse();
		return new string(span);
	}

	/// <inheritdoc cref="string.EndsWith(char)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.EndsWith(<paramref name="text"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Right(this string @this, char text)
		=> @this.EndsWith(text);

	/// <inheritdoc cref="string.EndsWith(string, StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.EndsWith(<paramref name="text"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Right(this string @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.EndsWith(text, comparison);

	/// <remarks>
	/// <code>
	/// Span&lt;<see cref="byte"/>&gt; bytes = <see langword="stackalloc"/> <see cref="byte"/>[<paramref name="encoding"/>.GetMaxByteCount(@<paramref name="this"/>.Length) - 1];<br/>
	/// @<paramref name="this"/>.ToBytes(<paramref name="encoding"/>, bytes);<br/>
	/// <br/>
	/// Span&lt;<see cref="char"/>&gt; chars = <see langword="stackalloc"/> <see cref="char"/>[bytes.Length * <see langword="sizeof"/>(<see cref="char"/>)];<br/>
	/// <see langword="return"/> <see cref="Convert"/>.TryToBase64Chars(bytes, chars, <see langword="out var"/> count)<br/>
	/// <see langword="    "/>? <see langword="new"/> <see cref="string"/>(chars.Slice(0, <paramref name="stripPadding"/> ? count - 2 : count))<br/>
	/// <see langword="    "/>: <see cref="string.Empty"/>;
	/// </code>
	/// </remarks>
	public static string ToBase64(this string @this, Encoding encoding, bool stripPadding = false)
	{
		Span<byte> bytes = stackalloc byte[encoding.GetMaxByteCount(@this.Length) - 1];
		@this.ToBytes(encoding, bytes);

		Span<char> chars = stackalloc char[bytes.Length * sizeof(char)];
		return Convert.TryToBase64Chars(bytes, chars, out var count)
			? new string(chars.Slice(0, stripPadding ? count - 2 : count))
			: string.Empty;
	}

	/// <inheritdoc cref="Encoding.GetBytes(string)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static byte[] ToBytes(this string @this, Encoding encoding)
		=> encoding.GetBytes(@this);

	/// <inheritdoc cref="Encoding.GetBytes(ReadOnlySpan{char}, Span{byte})"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetBytes(@<paramref name="this"/>, <paramref name="bytes"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int ToBytes(this string @this, Encoding encoding, Span<byte> bytes)
		=> encoding.GetBytes(@this, bytes);

	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>.TryParse(@<paramref name="this"/>, <see langword="true"/>, <see langword="out"/> <typeparamref name="T"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
	/// </remarks>
	public static T? ToEnum<T>(this string? @this)
		where T : struct, Enum
		=> Enum.TryParse(@this, true, out T result) ? (T?)result : null;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>) : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Uri? ToUri(this string? @this)
		=> @this is not null ? new Uri(@this) : null;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see langword="new"/> <see cref="Uri"/>(@<paramref name="this"/>, <paramref name="kind"/>) : <see langword="null"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Uri? ToUri(this string? @this, UriKind kind)
		=> @this is not null ? new Uri(@this, kind) : null;

	/// <remarks>
	/// <c>=&gt; <paramref name="text"/>.IsNotBlank() &amp;&amp; @<paramref name="this"/>?.Right(<paramref name="text"/>, <paramref name="comparison"/>) <see langword="is true"/><br/>
	/// <see langword="    "/>? @<paramref name="this"/>.Sunstring(0, @<paramref name="this"/>.Length - <paramref name="text"/>.Length)<br/>
	/// <see langword="    "/>: (@<paramref name="this"/> ?? <see cref="string.Empty"/>);</c>
	/// </remarks>
	public static string TrimEnd(this string @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> text.IsNotBlank() && @this?.Right(text, comparison) is true ? @this.Substring(0, @this.Length - text.Length) : (@this ?? string.Empty);

	/// <remarks>
	/// <c>=&gt; <paramref name="text"/>.IsNotBlank() &amp;&amp; @<paramref name="this"/>?.Left(<paramref name="text"/>, <paramref name="comparison"/>) <see langword="is true"/><br/>
	/// <see langword="    "/>? @<paramref name="this"/>.Sunstring(<paramref name="text"/>.Length)<br/>
	/// <see langword="    "/>: (@<paramref name="this"/> ?? <see cref="string.Empty"/>);</c>
	/// </remarks>
	public static string TrimStart(this string @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> text.IsNotBlank() && @this?.Left(text, comparison) is true ? @this.Substring(text.Length) : (@this ?? string.Empty);
}
