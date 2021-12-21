// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class SpanExtensions
{
	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T Read<T>(this Span<byte> @this)
		where T : struct
		=> MemoryMarshal.Read<T>(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Span<R> To<T, R>(this Span<T> @this)
		where T : struct
		where R : struct
		=> MemoryMarshal.Cast<T, R>(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Span<byte> ToBytes<T>(this Span<T> @this)
		where T : struct
		=> MemoryMarshal.AsBytes(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ref readonly T ToRef<T>(this Span<byte> @this)
		where T : struct
		=> ref MemoryMarshal.AsRef<T>(@this);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.TryRead(@<paramref name="this"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryRead<T>(this Span<byte> @this, out T value)
		where T : struct
		=> MemoryMarshal.TryRead(@this, out value);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.TryWrite(@<paramref name="this"/>, <see langword="ref"/> <paramref name="value"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryWrite<T>(this Span<byte> @this, T value)
		where T : struct
		=> MemoryMarshal.TryWrite(@this, ref value);

	/// <summary>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Write(@<paramref name="this"/>, <see langword="ref"/> <paramref name="value"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static void Write<T>(this Span<byte> @this, ref T value)
		where T : struct
		=> MemoryMarshal.Write(@this, ref value);
}
