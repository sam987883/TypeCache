// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TypeCache.Reflection;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class ReadOnlySpanExtensions
	{
		public static T Parse<T>(this ReadOnlySpan<char> @this)
			where T : unmanaged
			=> (T)(TypeOf<T>.SystemType switch
			{
				_ when TypeOf<T>.Kind == Kind.Enum => (object)Enum.Parse<T>(@this),
				SystemType.Boolean => (object)bool.Parse(@this),
				SystemType.Char => (object)@this[0].ToString(),
				SystemType.SByte => (object)sbyte.Parse(@this),
				SystemType.Int16 => (object)short.Parse(@this),
				SystemType.Int32 => (object)int.Parse(@this),
				SystemType.Int64 => (object)long.Parse(@this),
				SystemType.Byte => (object)sbyte.Parse(@this),
				SystemType.UInt16 => (object)ushort.Parse(@this),
				SystemType.UInt32 => (object)uint.Parse(@this),
				SystemType.UInt64 => (object)ulong.Parse(@this),
				SystemType.Single => (object)float.Parse(@this),
				SystemType.Double => (object)double.Parse(@this),
				SystemType.Decimal => (object)decimal.Parse(@this),
				_ => (object)default(T)
			});

		/// <summary>
		/// <c><see cref="MemoryMarshal.Read{T}(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static T To<T>(this ReadOnlySpan<byte> @this)
			where T : struct
			=> MemoryMarshal.Read<T>(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ReadOnlySpan<R> To<T, R>(this ReadOnlySpan<T> @this)
			where T : struct
			where R : struct
			=> MemoryMarshal.Cast<T, R>(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.AsBytes{T}(ReadOnlySpan{T})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ReadOnlySpan<byte> ToBytes<T>(this ReadOnlySpan<T> @this)
			where T : struct
			=> MemoryMarshal.AsBytes(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.ToEnumerable{T}(ReadOnlyMemory{T})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> @this)
			where T : struct
			=> MemoryMarshal.ToEnumerable(@this);

		/// <summary>
		/// <c><see cref="MemoryMarshal.AsRef{T}(ReadOnlySpan{byte})"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ref readonly T ToRef<T>(this ReadOnlySpan<byte> @this)
			where T : struct
			=> ref MemoryMarshal.AsRef<T>(@this);

		public static T? TryParse<T>(this ReadOnlySpan<char> @this)
			where T : unmanaged
			=> TypeOf<T>.SystemType switch
			{
				_ when TypeOf<T>.Kind == Kind.Enum => Enum.TryParse<T>(@this, out var value) ? (object)value : null,
				SystemType.Boolean => bool.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Char => (object)@this[0].ToString(),
				SystemType.SByte => sbyte.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Int16 => short.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Int32 => int.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Int64 => long.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Byte => sbyte.TryParse(@this, out var value) ? (object)value : null,
				SystemType.UInt16 => ushort.TryParse(@this, out var value) ? (object)value : null,
				SystemType.UInt32 => uint.TryParse(@this, out var value) ? (object)value : null,
				SystemType.UInt64 => ulong.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Single => float.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Double => double.TryParse(@this, out var value) ? (object)value : null,
				SystemType.Decimal => decimal.TryParse(@this, out var value) ? (object)value : null,
				_ => null
			} as T?;
	}
}
