// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TypeCache.Reflection;
using static TypeCache.Default;
using static TypeCache.Reflection.Unsafe;

namespace TypeCache.Extensions;

public static class ReadOnlySpanExtensions
{
	/// <summary>
	/// <code>
	/// =&gt; <see cref="TypeOf{T}.SystemType"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>_ <see langword="when"/> <see cref="TypeOf{T}.Kind"/> == <see cref="Kind.Enum"/> &amp;&amp; <see cref="Enum"/>.TryParse&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; value,<br/>
	/// <see langword="    "/><see cref="SystemType.Boolean"/> <see langword="when"/> <see cref="bool"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="bool"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Char"/> =&gt; Convert&lt;<see cref="char"/>, <typeparamref name="T"/>&gt;(@<paramref name="this"/>[0]),<br/>
	/// <see langword="    "/><see cref="SystemType.SByte"/> <see langword="when"/> <see cref="sbyte"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="sbyte"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Int16"/> <see langword="when"/> <see cref="short"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="short"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Int32"/> <see langword="when"/> <see cref="int"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="int"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Int64"/> <see langword="when"/> <see cref="long"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="long"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Byte"/> <see langword="when"/> <see cref="byte"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="byte"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.UInt16"/> <see langword="when"/> <see cref="ushort"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="ushort"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.UInt32"/> <see langword="when"/> <see cref="uint"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="uint"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.UInt64"/> <see langword="when"/> <see cref="ulong"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="ulong"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Single"/> <see langword="when"/> <see cref="float"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="float"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Double"/> <see langword="when"/> <see cref="double"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="double"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/><see cref="SystemType.Decimal"/> <see langword="when"/> <see cref="decimal"/>.TryParse(@<paramref name="this"/>, <see langword="out var"/> value) =&gt; Convert&lt;<see cref="decimal"/>, <typeparamref name="T"/>&gt;(<see langword="ref"/> value),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// };
	/// </code>
	/// </summary>
	public static T? Parse<T>(this ReadOnlySpan<char> @this)
		where T : unmanaged
		=> TypeOf<T>.SystemType switch
		{
			_ when TypeOf<T>.Kind == Kind.Enum && Enum.TryParse<T>(@this, out var value) => value,
			SystemType.Boolean when bool.TryParse(@this, out var value) => Convert<bool, T>(ref value),
			SystemType.Char => Convert<char, T>(@this[0]),
			SystemType.SByte when sbyte.TryParse(@this, out var value) => Convert<sbyte, T>(ref value),
			SystemType.Int16 when short.TryParse(@this, out var value) => Convert<short, T>(ref value),
			SystemType.Int32 when int.TryParse(@this, out var value) => Convert<int, T>(ref value),
			SystemType.Int64 when long.TryParse(@this, out var value) => Convert<long, T>(ref value),
			SystemType.Byte when byte.TryParse(@this, out var value) => Convert<byte, T>(ref value),
			SystemType.UInt16 when ushort.TryParse(@this, out var value) => Convert<ushort, T>(ref value),
			SystemType.UInt32 when uint.TryParse(@this, out var value) => Convert<uint, T>(ref value),
			SystemType.UInt64 when ulong.TryParse(@this, out var value) => Convert<ulong, T>(ref value),
			SystemType.Single when float.TryParse(@this, out var value) => Convert<float, T>(ref value),
			SystemType.Double when double.TryParse(@this, out var value) => Convert<double, T>(ref value),
			SystemType.Decimal when decimal.TryParse(@this, out var value) => Convert<decimal, T>(ref value),
			_ => null
		};

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T To<T>(this ReadOnlySpan<byte> @this)
		where T : struct
		=> MemoryMarshal.Read<T>(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ReadOnlySpan<R> To<T, R>(this ReadOnlySpan<T> @this)
		where T : struct
		where R : struct
		=> MemoryMarshal.Cast<T, R>(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ReadOnlySpan<byte> ToBytes<T>(this ReadOnlySpan<T> @this)
		where T : struct
		=> MemoryMarshal.AsBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.ToEnumerable(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> @this)
		where T : struct
		=> MemoryMarshal.ToEnumerable(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ref readonly T ToRef<T>(this ReadOnlySpan<byte> @this)
		where T : struct
		=> ref MemoryMarshal.AsRef<T>(@this);
}
