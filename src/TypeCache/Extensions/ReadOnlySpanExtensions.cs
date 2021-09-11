// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class ReadOnlySpanExtensions
	{
		/// <summary>
		/// <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T To<T>(this ReadOnlySpan<byte> @this)
			where T : struct
			=> MemoryMarshal.Read<T>(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ReadOnlySpan<R> To<T, R>(this ReadOnlySpan<T> @this)
			where T : struct
			where R : struct
			=> MemoryMarshal.Cast<T, R>(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToBoolean(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool ToBoolean(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToBoolean(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.AsBytes(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ReadOnlySpan<byte> ToBytes<T>(this ReadOnlySpan<T> @this)
			where T : struct
			=> MemoryMarshal.AsBytes(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToChar(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static char ToChar(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToChar(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToDouble(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static double ToDouble(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToDouble(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt16(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static short ToInt16(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt16(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt32(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static int ToInt32(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt32(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToInt64(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static long ToInt64(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToInt64(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ref readonly T ToRef<T>(this ReadOnlySpan<byte> @this)
			where T : struct
			=> ref MemoryMarshal.AsRef<T>(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToSingle(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static float ToSingle(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToSingle(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt16(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ushort ToUInt16(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt16(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt32(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static uint ToUInt32(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt32(@this);

		/// <summary>
		/// <c><see cref="BitConverter.ToUInt64(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ulong ToUInt64(this ReadOnlySpan<byte> @this)
			=> BitConverter.ToUInt64(@this);
	}
}
