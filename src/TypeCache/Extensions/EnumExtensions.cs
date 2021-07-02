// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class EnumExtensions
	{
		/// <summary>
		/// <c><see cref="EnumOf{T}.Tokens"/>[@this].Attributes</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<Attribute> Attributes<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Attributes;

		/// <summary>
		/// <c><see cref="EnumOf{T}.Tokens"/>[@this].Hex</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Hex<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Hex;

		/// <summary>
		/// <c><see cref="EnumOf{T}.Tokens"/>[@this].Name</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Name<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Name;

		/// <summary>
		/// <c><see cref="EnumOf{T}.Tokens"/>[@this].Number</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Number<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Number;

		/// <summary>
		/// <c><see cref="StringComparer.FromComparison(StringComparison)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringComparer ToStringComparer(this StringComparison @this)
			=> StringComparer.FromComparison(@this);
	}
}
