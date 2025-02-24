// Copyright (c) 2021 Samuel Abraham

using System.Runtime.InteropServices;

namespace TypeCache.Extensions;

public static class SpanExtensions
{
	/// <inheritdoc cref="MemoryMarshal.AsBytes{T}(Span{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Span<byte> AsBytes<T>(this Span<T> @this)
		where T : struct
		=> MemoryMarshal.AsBytes(@this);

	/// <remarks>
	/// <c>=&gt; (<see cref="ReadOnlySpan{T}"/>)@<paramref name="this"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ReadOnlySpan<T> AsReadOnly<T>(this Span<T> @this)
		=> (ReadOnlySpan<T>)@this;

	/// <inheritdoc cref="MemoryMarshal.AsRef{T}(Span{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="ref"/> <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ref readonly T AsRef<T>(this Span<byte> @this)
		where T : struct
		=> ref MemoryMarshal.AsRef<T>(@this);

	/// <inheritdoc cref="MemoryMarshal.Cast{TFrom, TTo}(Span{TFrom})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Span<R> Cast<T, R>(this Span<T> @this)
		where T : struct
		where R : struct
		=> MemoryMarshal.Cast<T, R>(@this);

	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Read<T>(this scoped Span<byte> @this)
		where T : struct
		=> MemoryMarshal.Read<T>(@this);

	public static Span<char> Replace(this Span<char> @this, char existing, char replacement)
	{
		for (var i = 0; i < @this.Length; ++i)
			if (@this[i] == existing)
				@this[i] = replacement;
		return @this;
	}

	/// <inheritdoc cref="MemoryMarshal.TryRead{T}(ReadOnlySpan{byte}, out T)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.TryRead(@<paramref name="this"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryRead<T>(this Span<byte> @this, out T value)
		where T : struct
		=> MemoryMarshal.TryRead(@this, out value);

	/// <inheritdoc cref="MemoryMarshal.TryWrite{T}(Span{byte}, in T)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.TryWrite(@<paramref name="this"/>, <see langword="ref"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryWrite<T>(this Span<byte> @this, in T value)
		where T : struct
		=> MemoryMarshal.TryWrite(@this, in value);

	/// <inheritdoc cref="MemoryMarshal.Write{T}(Span{byte}, in T)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Write(@<paramref name="this"/>, <see langword="ref"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Write<T>(this Span<byte> @this, in T value)
		where T : struct
		=> MemoryMarshal.Write(@this, in value);
}
