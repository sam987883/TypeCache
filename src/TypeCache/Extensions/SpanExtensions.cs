// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class SpanExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, bool value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, char value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, short value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, int value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, long value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, ushort value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, uint value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, ulong value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, float value)
			=> BitConverter.TryWriteBytes(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWriteBytes(this Span<byte> @this, double value)
			=> BitConverter.TryWriteBytes(@this, value);
	}
}
