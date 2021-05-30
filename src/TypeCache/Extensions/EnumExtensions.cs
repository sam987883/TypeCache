// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class EnumExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<Attribute> Attributes<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Attributes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Hex<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Hex;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Name<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Name;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Number<T>(this T @this)
			where T : struct, Enum
			=> EnumOf<T>.Tokens[@this].Number;

		public static StringComparer ToStringComparer(this StringComparison @this)
			=> @this switch
			{
				StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
				StringComparison.Ordinal => StringComparer.Ordinal,
				StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
				StringComparison.InvariantCulture => StringComparer.InvariantCulture,
				StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
				StringComparison.CurrentCulture => StringComparer.CurrentCulture,
				_ => throw new NotImplementedException($"[{nameof(StringComparison)}] with a value of {@this.Number()} does not exist.")
			};
	}
}
