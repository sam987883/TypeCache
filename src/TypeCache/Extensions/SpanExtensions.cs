// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class SpanExtensions
	{
		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, bool)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, bool value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, char)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, char value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, short)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, short value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, int value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, long)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, long value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, ushort)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, ushort value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, uint)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, uint value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, ulong)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, ulong value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, float)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, float value)
			=> BitConverter.TryWriteBytes(@this, value);

		/// <summary>
		/// <c><see cref="BitConverter.TryWriteBytes(Span{byte}, double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, double value)
			=> BitConverter.TryWriteBytes(@this, value);
	}
}
