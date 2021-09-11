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
		/// <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T To<T>(this Span<byte> @this)
			where T : struct
			=> MemoryMarshal.Read<T>(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Span<R> To<T, R>(this Span<T> @this)
			where T : struct
			where R : struct
			=> MemoryMarshal.Cast<T, R>(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.AsBytes(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Span<byte> ToBytes<T>(this Span<T> @this)
			where T : struct
			=> MemoryMarshal.AsBytes(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T ToEnumerable<T>(this Span<byte> @this)
			where T : struct
			=> MemoryMarshal.Read<T>(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ref readonly T ToRef<T>(this Span<byte> @this)
			where T : struct
			=> ref MemoryMarshal.AsRef<T>(@this);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, bool)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, bool value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, char)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, char value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, double)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, double value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, float)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, float value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, int)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, int value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, long)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, long value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, short)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, short value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, uint)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, uint value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, ulong)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, ulong value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, ushort)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool TryWriteBytes(this Span<byte> @this, ushort value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.Write&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, ref <paramref name="value"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static void Write<T>(this Span<byte> @this, ref T value)
			where T : struct
			=> MemoryMarshal.Write(@this, ref value);
	}
}
