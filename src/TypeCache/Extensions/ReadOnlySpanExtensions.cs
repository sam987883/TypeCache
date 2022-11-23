// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using TypeCache.Collections;
using static System.Globalization.CultureInfo;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class ReadOnlySpanExtensions
{
	/// <remarks>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/><paramref name="action"/>(@<paramref name="this"/>[i]);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T> action)
	{
		action.AssertNotNull();

		if (@this.IsEmpty)
			return;

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			action(@this[i]);
	}

	/// <remarks>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/><paramref name="action"/>(@<paramref name="this"/>[i], i);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T, int> action)
	{
		action.AssertNotNull();

		if (@this.IsEmpty)
			return;

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			action(@this[i], i);
	}

	/// <remarks>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <paramref name="between"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IsEmpty)<br/>
	/// <see langword="    return"/>;<br/>
	/// <br/>
	/// <paramref name="action"/>(@<paramref name="this"/>[0])<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="between"/>();<br/>
	/// <see langword="    "/><paramref name="action"/>(@<paramref name="this"/>[i]);<br/>
	/// }
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T> action, Action between)
	{
		action.AssertNotNull();
		between.AssertNotNull();

		if (@this.IsEmpty)
			return;

		action(@this[0]);

		var count = @this.Length;
		for (var i = 1; i < count; ++i)
		{
			between();
			action(@this[i]);
		}
	}

	/// <remarks>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <paramref name="between"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IsEmpty)<br/>
	/// <see langword="    return"/>;<br/>
	/// <br/>
	/// <paramref name="action"/>(@<paramref name="this"/>[0])<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="between"/>();<br/>
	/// <see langword="    "/><paramref name="action"/>(@<paramref name="this"/>[i], i);<br/>
	/// }
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T, int> action, Action between)
	{
		action.AssertNotNull();
		between.AssertNotNull();

		if (@this.IsEmpty)
			return;

		action(@this[0], 0);

		var count = @this.Length;
		for (var i = 1; i < count; ++i)
		{
			between();
			action(@this[i], i);
		}
	}

	/// <remarks>
	/// <code>
	/// <see langword="if"/> (!<paramref name="values"/>.Any())<br/>
	/// <see langword="    return new"/> <see cref="string"/>(@<paramref name="this"/>);<br/>
	/// <br/>
	/// <see langword="var"/> totalLength = (<see cref="int"/>)<paramref name="values"/>.Select(value =&gt; value.Length).Sum() + @<paramref name="this"/>.Length * (<paramref name="values"/>.Count() - 1);<br/>
	/// Span&lt;<see cref="char"/>&gt; result = <see langword="stackalloc"/> <see cref="char"/>[totalLength];<br/>
	/// <br/>
	/// <see langword="var"/> offset = 0;<br/>
	/// <see langword="foreach"/> (<see langword="var"/> value <see langword="in"/> <paramref name="values"/>)<br/>
	/// {<br/>
	/// <see langword="    if"/> (offset &gt; 0)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        "/>@<paramref name="this"/>.CopyTo(result.Slice(offset, @<paramref name="this"/>.Length));<br/>
	/// <see langword="        "/>offset += @<paramref name="this"/>.Length;<br/>
	/// <see langword="    "/>}<br/>
	/// <br/>
	/// <see langword="    var"/> span = value.AsSpan();<br/>
	/// <see langword="    "/>span.CopyTo(result.Slice(offset, span.Length));<br/>
	/// <see langword="    "/>offset += span.Length;<br/>
	/// }<br/>
	/// <br/>
	/// <see langword="return new"/> <see cref="string"/>(result);
	/// </code>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Join(this scoped ReadOnlySpan<char> @this, IEnumerable<string> values)
	{
		if (!values.Any())
			return new string(@this);

		var totalLength = (int)values.Select(value => value.Length).Sum() + @this.Length * (values.Count() - 1);
		Span<char> result = stackalloc char[totalLength];

		var offset = 0;
		foreach (var value in values)
		{
			if (offset > 0)
			{
				@this.CopyTo(result.Slice(offset, @this.Length));
				offset += @this.Length;
			}

			var span = value.AsSpan();
			span.CopyTo(result.Slice(offset, span.Length));
			offset += span.Length;
		}

		return new string(result);
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Join((<see cref="IEnumerable{T}"/>)<paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Join(this scoped ReadOnlySpan<char> @this, params string[] values)
		=> @this.Join((IEnumerable<string>)values);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>[0] == <paramref name="text"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Left(this scoped ReadOnlySpan<char> @this, char text)
		=> @this[0] == text;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Slice(0, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ReadOnlySpan<char> Left(this ReadOnlySpan<char> @this, int length)
		=> @this.Slice(0, length);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Left(this scoped ReadOnlySpan<char> @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.StartsWith(text, comparison);

	/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T Parse<T>(this scoped ReadOnlySpan<char> @this, IFormatProvider? formatProvider)
		where T : ISpanParsable<T>
		=> T.Parse(@this, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T Parse<T>(this scoped ReadOnlySpan<char> @this, NumberStyles style, IFormatProvider? formatProvider = null)
		where T : INumberBase<T>
		=> T.Parse(@this, style, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="MemoryMarshal.Read{T}(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T To<T>(this scoped ReadOnlySpan<byte> @this)
		where T : struct
		=> MemoryMarshal.Read<T>(@this);

	/// <inheritdoc cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ReadOnlySpan<R> To<T, R>(this ReadOnlySpan<T> @this)
		where T : struct
		where R : struct
		=> MemoryMarshal.Cast<T, R>(@this);

	/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.ToBase64String(@<paramref name="this"/>, <paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToBase64(this scoped ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> Convert.ToBase64String(@this, options);

	/// <remarks>
	/// <code>
	/// Span&lt;<see cref="char"/>&gt; chars = <see langword="stackalloc"/> <see cref="char"/>[@<paramref name="this"/>.Length * <see langword="sizeof"/>(<see cref="int"/>)];<br/>
	/// <see langword="return"/> Convert.TryToBase64Chars(@this, chars, out var length, options)<br/>
	/// <see langword="    "/>? chars.Slice(0, length).ToArray()<br/>
	/// <see langword="    "/>: Array&lt;<see cref="char"/>&gt;.Empty;
	/// </code>
	/// </remarks>
	public static char[] ToBase64Chars(this scoped ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
	{
		Span<char> chars = stackalloc char[@this.Length * sizeof(int)];
		return Convert.TryToBase64Chars(@this, chars, out var length, options)
			? chars.Slice(0, length).ToArray()
			: Array<char>.Empty;
	}

	/// <inheritdoc cref="MemoryMarshal.AsBytes{T}(ReadOnlySpan{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ReadOnlySpan<byte> ToBytes<T>(this ReadOnlySpan<T> @this)
		where T : struct
		=> MemoryMarshal.AsBytes(@this);

	/// <inheritdoc cref="MemoryMarshal.ToEnumerable{T}(ReadOnlyMemory{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.ToEnumerable(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> @this)
		where T : struct
		=> MemoryMarshal.ToEnumerable(@this);

	public static string ToHex(this scoped ReadOnlySpan<byte> @this)
	{
		const string HEX_CHARS = "0123456789ABCDEF";

		Span<char> chars = stackalloc char[@this.Length * sizeof(char)];
		for (var i = 0; i < @this.Length; ++i)
		{
			var c = i * 2;
			chars[c] = HEX_CHARS[(@this[i] & 0xf0) >> 4];
			chars[c + 1] = HEX_CHARS[@this[i] & 0x0f];
		}
		return new string(chars);
	}

	/// <inheritdoc cref="MemoryMarshal.AsRef{T}(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="ref"/> <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ref readonly T ToRef<T>(this ReadOnlySpan<byte> @this)
		where T : struct
		=> ref MemoryMarshal.AsRef<T>(@this);

	/// <inheritdoc cref="Encoding.GetString(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetString(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToText(this scoped ReadOnlySpan<byte> @this, Encoding encoding)
		=> encoding.GetString(@this);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool TryParse<T>(this scoped ReadOnlySpan<char> @this, [MaybeNullWhen(false)] out T value)
		where T : ISpanParsable<T>
		=> T.TryParse(@this, InvariantCulture, out value);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool TryParse<T>(this scoped ReadOnlySpan<char> @this, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : ISpanParsable<T>
		=> T.TryParse(@this, formatProvider ?? InvariantCulture, out value);

	/// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, System.Globalization.NumberStyles, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool TryParse<T>(this scoped ReadOnlySpan<char> @this, NumberStyles style, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : INumberBase<T>
		=> T.TryParse(@this, style, formatProvider ?? InvariantCulture, out value);
}
