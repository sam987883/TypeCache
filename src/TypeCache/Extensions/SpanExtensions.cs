// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TypeCache.Collections;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class SpanExtensions
{
	/// <remarks>
	/// <c>=&gt; (<see cref="ReadOnlySpan{T}"/>)@<paramref name="this"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ReadOnlySpan<T> AsReadOnly<T>(this Span<T> @this)
		=> (ReadOnlySpan<T>)@this;

	/// <remarks>
	/// <code>
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/>@<paramref name="this"/>[i] = (@<paramref name="this"/>[i]);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static void Each<T>(this Span<T> @this, Func<T, T> edit)
	{
		edit.AssertNotNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			@this[i] = edit(@this[i]);
	}

	/// <remarks>
	/// <code>
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/>@<paramref name="this"/>[i] = (@<paramref name="this"/>[i]);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static void Each<T>(this Span<T> @this, Func<T, int, T> edit)
	{
		edit.AssertNotNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			@this[i] = edit(@this[i], i);
	}

	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T Read<T>(this Span<byte> @this)
		where T : struct
		=> MemoryMarshal.Read<T>(@this);

	/// <remarks>
	/// <code>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; @<paramref name="this"/>.Length; ++i)<br/>
	/// <see langword="    if"/> (@<paramref name="this"/>[i] == <paramref name="existing"/>)<br/>
	/// <see langword="        "/>@<paramref name="this"/>[i] = <paramref name="replacement"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>;
	/// </code>
	/// </remarks>
	public static Span<char> Replace(this Span<char> @this, char existing, char replacement)
	{
		for (var i = 0; i < @this.Length; ++i)
			if (@this[i] == existing)
				@this[i] = replacement;
		return @this;
	}

	/// <inheritdoc cref="MemoryMarshal.Cast{TFrom, TTo}(Span{TFrom})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Span<R> To<T, R>(this Span<T> @this)
		where T : struct
		where R : struct
		=> MemoryMarshal.Cast<T, R>(@this);

	/// <inheritdoc cref="MemoryMarshal.AsBytes{T}(Span{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Span<byte> ToBytes<T>(this Span<T> @this)
		where T : struct
		=> MemoryMarshal.AsBytes(@this);

	/// <inheritdoc cref="MemoryMarshal.AsRef{T}(Span{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="ref"/> <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ref readonly T ToRef<T>(this Span<byte> @this)
		where T : struct
		=> ref MemoryMarshal.AsRef<T>(@this);

	/// <inheritdoc cref="MemoryMarshal.TryRead{T}(ReadOnlySpan{byte}, out T)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.TryRead(@<paramref name="this"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool TryRead<T>(this Span<byte> @this, out T value)
		where T : struct
		=> MemoryMarshal.TryRead(@this, out value);

	/// <inheritdoc cref="MemoryMarshal.TryWrite{T}(Span{byte}, ref T)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.TryWrite(@<paramref name="this"/>, <see langword="ref"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool TryWrite<T>(this Span<byte> @this, T value)
		where T : struct
		=> MemoryMarshal.TryWrite(@this, ref value);

	/// <inheritdoc cref="MemoryMarshal.Write{T}(Span{byte}, ref T)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Write(@<paramref name="this"/>, <see langword="ref"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Write<T>(this Span<byte> @this, ref T value)
		where T : struct
		=> MemoryMarshal.Write(@this, ref value);
}
