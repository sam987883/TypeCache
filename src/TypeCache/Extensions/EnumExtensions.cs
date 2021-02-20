// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class EnumExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<Attribute> Attributes<T>(this T @this)
			where T : struct, Enum
			=> Enum<T>.Map[@this].Attributes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Hex<T>(this T @this)
			where T : Enum
			=> @this.ToString("X");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Name<T>(this T @this)
			where T : struct, Enum
			=> Enum<T>.Map[@this].Name;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Number<T>(this T @this)
			where T : Enum
			=> @this.ToString("D");
	}
}
