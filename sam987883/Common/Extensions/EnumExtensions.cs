// Copyright (c) 2020 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace sam987883.Common.Extensions
{
	public static class EnumExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Hex<T>(this T @this) where T : Enum =>
			@this.ToString("X");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Name<T>(this T @this) where T : Enum =>
			@this.ToString("G");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Number<T>(this T @this) where T : Enum =>
			@this.ToString("D");
	}
}
