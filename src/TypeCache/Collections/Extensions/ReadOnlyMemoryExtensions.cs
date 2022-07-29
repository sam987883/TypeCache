// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class ReadOnlyMemoryExtensions
{
	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToBoolean(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool ToBoolean(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToBoolean(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToChar(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static char ToChar(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToChar(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToDouble(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double ToDouble(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToDouble(@this.Span);

	public static string ToHex(this ReadOnlyMemory<byte> @this)
	{
		const string HEX_CHARS = "0123456789ABCDEF";

		Span<char> chars = stackalloc char[@this.Length * sizeof(char)];
		for (var i = 0; i < @this.Length; ++i)
		{
			var c = i * 2;
			chars[c] = HEX_CHARS[(@this.Span[i] & 0xf0) >> 4];
			chars[c + 1] = HEX_CHARS[@this.Span[i] & 0x0f];
		}
		return new string(chars);
	}

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt16(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static short ToInt16(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToInt16(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt32(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int ToInt32(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToInt32(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt64(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static long ToInt64(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToInt64(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToSingle(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static float ToSingle(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToSingle(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToText(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToString(@this.ToArray());

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.Span, <paramref name="length"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToText(this ReadOnlyMemory<byte> @this, int startIndex, int length)
		=> BitConverter.ToString(@this.ToArray(), startIndex, length);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt16(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ushort ToUInt16(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToUInt16(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt32(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static uint ToUInt32(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToUInt32(@this.Span);

	/// <summary>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt64(@<paramref name="this"/>.Span);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ulong ToUInt64(this ReadOnlyMemory<byte> @this)
		=> BitConverter.ToUInt64(@this.Span);
}
