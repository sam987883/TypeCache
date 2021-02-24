// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using static System.Math;

namespace TypeCache.Extensions
{
	public static class MathExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte AbsoluteValue(this sbyte @this)
			=> Abs(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short AbsoluteValue(this short @this)
			=> Abs(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int AbsoluteValue(this int @this)
			=> Abs(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long AbsoluteValue(this long @this)
			=> Abs(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AbsoluteValue(this float @this)
			=> Abs(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double AbsoluteValue(this double @this)
			=> Abs(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal AbsoluteValue(this decimal @this)
			=> Abs(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double BitDecrement(this double @this)
			=> BitDecrement(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double BitIncrement(this double @this)
			=> BitIncrement(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Ceiling(this float @this)
			=> Ceiling(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Ceiling(this double @this)
			=> Ceiling(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Ceiling(this decimal @this)
			=> Ceiling(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Floor(this float @this)
			=> Floor(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Floor(this double @this)
			=> Floor(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Floor(this decimal @this)
			=> Floor(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this)
			=> Round(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this, int digits)
			=> Round(@this, digits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this, MidpointRounding rounding)
			=> Round(@this, rounding);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(this double @this, int digits, MidpointRounding rounding)
			=> Round(@this, digits, rounding);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this)
			=> Round(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this, int digits)
			=> Round(@this, digits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this, MidpointRounding rounding)
			=> Round(@this, rounding);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(this decimal @this, int digits, MidpointRounding rounding)
			=> Round(@this, digits, rounding);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this sbyte @this)
			=> Sign(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this short @this)
			=> Sign(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this int @this)
			=> Sign(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this long @this)
			=> Sign(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this float @this)
			=> Sign(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this double @this)
			=> Sign(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this decimal @this)
			=> Sign(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Truncate(this float @this)
			=> Truncate(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Truncate(this double @this)
			=> Truncate(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Truncate(this decimal @this)
			=> Truncate(@this);
	}
}
