// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class SpanExtensions
	{
		/// <summary>
		/// <c><see cref="MemoryMarshal.Read{T}(Span{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T Read<T>(this Span<byte> @this)
			where T : struct
			=> MemoryMarshal.Read<T>(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.Cast{TFrom, TTo}(Span{TFrom})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Span<R> To<T, R>(this Span<T> @this)
			where T : struct
			where R : struct
			=> MemoryMarshal.Cast<T, R>(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.AsBytes{T}(Span{T})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Span<byte> ToBytes<T>(this Span<T> @this)
			where T : struct
			=> MemoryMarshal.AsBytes(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.AsRef{T}(Span{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ref readonly T ToRef<T>(this Span<byte> @this)
			where T : struct
			=> ref MemoryMarshal.AsRef<T>(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.TryRead{T}(ReadOnlySpan{byte}, out T)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryRead<T>(this Span<byte> @this, out T value)
			where T : struct
			=> MemoryMarshal.TryRead<T>(@this, out value);

		/// <summary>
		/// <c><see cref="MemoryMarshal.TryWrite{T}(Span{byte}, ref T)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWrite<T>(this Span<byte> @this, T value)
			where T : struct
			=> MemoryMarshal.TryWrite(@this, ref value);

		/// <summary>
		/// <c><see cref="MemoryMarshal.Write{T}(Span{byte}, ref T)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static void Write<T>(this Span<byte> @this, ref T value)
			where T : struct
			=> MemoryMarshal.Write(@this, ref value);
	}
}
