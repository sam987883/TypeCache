// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class MathExtensions
	{
		/// <summary>
		/// <c><see cref="Math.Abs(sbyte)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte AbsoluteValue(this sbyte @this)
			=> Math.Abs(@this);

		/// <summary>
		/// <c><see cref="Math.Abs(short)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short AbsoluteValue(this short @this)
			=> Math.Abs(@this);

		/// <summary>
		/// <c><see cref="Math.Abs(int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int AbsoluteValue(this int @this)
			=> Math.Abs(@this);

		/// <summary>
		/// <c><see cref="Math.Abs(long)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long AbsoluteValue(this long @this)
			=> Math.Abs(@this);

		/// <summary>
		/// <c><see cref="Math.Abs(float)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AbsoluteValue(this float @this)
			=> Math.Abs(@this);

		/// <summary>
		/// <c><see cref="Math.Abs(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double AbsoluteValue(this double @this)
			=> Math.Abs(@this);

		/// <summary>
		/// <c><see cref="Math.Abs(decimal)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal AbsoluteValue(this decimal @this)
			=> Math.Abs(@this);

		/// <summary>
		/// <c><see cref="Math.BitDecrement(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double BitDecrement(this double @this)
			=> Math.BitDecrement(@this);

		/// <summary>
		/// <c><see cref="Math.BitIncrement(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double BitIncrement(this double @this)
			=> Math.BitIncrement(@this);

		/// <summary>
		/// <c><see cref="Math.Ceiling(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Ceiling(this double @this)
			=> Math.Ceiling(@this);

		/// <summary>
		/// <c><see cref="Math.Ceiling(decimal)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Ceiling(this decimal @this)
			=> Math.Ceiling(@this);

		/// <summary>
		/// <c><see cref="Math.Floor(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Floor(this double @this)
			=> Math.Floor(@this);

		/// <summary>
		/// <c><see cref="Math.Floor(decimal)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Floor(this decimal @this)
			=> Math.Floor(@this);

		/// <summary>
		/// <c><see cref="Math.Round(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this)
			=> Math.Round(@this);

		/// <summary>
		/// <c><see cref="Math.Round(double, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this, int digits)
			=> Math.Round(@this, digits);

		/// <summary>
		/// <c><see cref="Math.Round(double, MidpointRounding)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this, MidpointRounding rounding)
			=> Math.Round(@this, rounding);

		/// <summary>
		/// <c><see cref="Math.Round(double, int, MidpointRounding)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this, int digits, MidpointRounding rounding)
			=> Math.Round(@this, digits, rounding);

		/// <summary>
		/// <c><see cref="Math.Round(decimal)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this)
			=> Math.Round(@this);

		/// <summary>
		/// <c><see cref="Math.Round(decimal, int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this, int digits)
			=> Math.Round(@this, digits);

		/// <summary>
		/// <c><see cref="Math.Round(decimal, MidpointRounding)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this, MidpointRounding rounding)
			=> Math.Round(@this, rounding);

		/// <summary>
		/// <c><see cref="Math.Round(decimal, int, MidpointRounding)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this, int digits, MidpointRounding rounding)
			=> Math.Round(@this, digits, rounding);

		/// <summary>
		/// <c><see cref="Math.Sign(sbyte)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this sbyte @this)
			=> Math.Sign(@this);

		/// <summary>
		/// <c><see cref="Math.Sign(short)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this short @this)
			=> Math.Sign(@this);

		/// <summary>
		/// <c><see cref="Math.Sign(int)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this int @this)
			=> Math.Sign(@this);

		/// <summary>
		/// <c><see cref="Math.Sign(long)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this long @this)
			=> Math.Sign(@this);

		/// <summary>
		/// <c><see cref="Math.Sign(float)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this float @this)
			=> Math.Sign(@this);

		/// <summary>
		/// <c><see cref="Math.Sign(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this double @this)
			=> Math.Sign(@this);

		/// <summary>
		/// <c><see cref="Math.Sign(decimal)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this decimal @this)
			=> Math.Sign(@this);

		/// <summary>
		/// <c><see cref="Math.Truncate(double)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Truncate(this double @this)
			=> Math.Truncate(@this);

		/// <summary>
		/// <c><see cref="Math.Truncate(decimal)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Truncate(this decimal @this)
			=> Math.Truncate(@this);
	}
}
