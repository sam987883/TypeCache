// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using static TypeCache.Default;
using Unsafe = TypeCache.Reflection.Unsafe;

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
	public static void Do<T>(this ReadOnlySpan<T> @this, Action<T> action)
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
	public static void Do<T>(this ReadOnlySpan<T> @this, Action<T, int> action)
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
	public static void Do<T>(this ReadOnlySpan<T> @this, Action<T> action, Action between)
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
	public static void Do<T>(this ReadOnlySpan<T> @this, Action<T, int> action, Action between)
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
	/// <see langword="    return"/> <see cref="string.Empty"/>;<br/>
	/// <br/>
	/// <see langword="var"/> totalLength = (<see cref="int"/>)<paramref name="values"/>.Map(value =&gt; value.Length).Sum() + @<paramref name="this"/>.Length * (<paramref name="values"/>.Count() - 1);<br/>
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
	public static string Join(this ReadOnlySpan<char> @this, IEnumerable<string> values)
	{
		if (!values.Any())
			return string.Empty;

		var totalLength = (int)values.Map(value => value.Length).Sum() + @this.Length * (values.Count() - 1);
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
	public static string Join(this ReadOnlySpan<char> @this, params string[] values)
		=> @this.Join((IEnumerable<string>)values);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>[0] == <paramref name="text"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Left(this ReadOnlySpan<char> @this, char text)
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
	public static bool Left(this ReadOnlySpan<char> @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.StartsWith(text, comparison);

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
	/// };
	/// </code>
	/// </remarks>
	public static T? Parse<T>(this ReadOnlySpan<char> @this)
		where T : unmanaged
		=> TypeOf<T>.SystemType switch
		{
			_ when TypeOf<T>.Kind == Kind.Enum && Enum.TryParse<T>(@this, out var value) => value,
			SystemType.Boolean when bool.TryParse(@this, out var value) => Unsafe.Convert<bool, T>(value),
			SystemType.Char => @this.Length == 1 ? Unsafe.Convert<char, T>(@this[0]) : null,
			SystemType.SByte when sbyte.TryParse(@this, out var value) => Unsafe.Convert<sbyte, T>(value),
			SystemType.Int16 when short.TryParse(@this, out var value) => Unsafe.Convert<short, T>(value),
			SystemType.Int32 when int.TryParse(@this, out var value) => Unsafe.Convert<int, T>(value),
			SystemType.Int64 when long.TryParse(@this, out var value) => Unsafe.Convert<long, T>(value),
			SystemType.Byte when byte.TryParse(@this, out var value) => Unsafe.Convert<byte, T>(value),
			SystemType.UInt16 when ushort.TryParse(@this, out var value) => Unsafe.Convert<ushort, T>(value),
			SystemType.UInt32 when uint.TryParse(@this, out var value) => Unsafe.Convert<uint, T>(value),
			SystemType.UInt64 when ulong.TryParse(@this, out var value) => Unsafe.Convert<ulong, T>(value),
			SystemType.Single when float.TryParse(@this, out var value) => Unsafe.Convert<float, T>(value),
			SystemType.Double when double.TryParse(@this, out var value) => Unsafe.Convert<double, T>(value),
			SystemType.Decimal when decimal.TryParse(@this, out var value) => Unsafe.Convert<decimal, T>(value),
			_ => null
		};

	/// <inheritdoc cref="MemoryMarshal.Read{T}(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T To<T>(this ReadOnlySpan<byte> @this)
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
	public static string ToBase64(this ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> Convert.ToBase64String(@this, options);

	/// <remarks>
	/// <code>
	/// Span&lt;<see cref="char"/>&gt; chars = <see langword="stackalloc"/> <see cref="char"/>[@<paramref name="this"/>.Length * <see langword="sizeof"/>(<see cref="int"/>)];<br/>
	/// <see langword="return"/> Convert.TryToBase64Chars(@this, chars, out var length, options)<br/>
	/// <see langword="    "/>? chars.Slice(0, length).ToArray()<br/>
	/// <see langword="    "/>: Array&lt;<see cref="char"/>&gt;.Empty;
	/// </code>
	/// </remarks>
	public static char[] ToBase64Chars(this ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
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

	public static string ToHex(this ReadOnlySpan<byte> @this)
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
	public static string ToText(this ReadOnlySpan<byte> @this, Encoding encoding)
		=> encoding.GetString(@this);
}
