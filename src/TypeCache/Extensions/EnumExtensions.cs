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
		/// <code>
		/// <list type="table">
		/// <item><term><see cref="StringComparison.CurrentCulture"/></term> <description><see cref="StringComparer.CurrentCulture"/></description></item>
		/// <item><term><see cref="StringComparison.CurrentCultureIgnoreCase"/></term> <description><see cref="StringComparer.CurrentCultureIgnoreCase"/></description></item>
		/// <item><term><see cref="StringComparison.InvariantCulture"/></term> <description><see cref="StringComparer.InvariantCulture"/></description></item>
		/// <item><term><see cref="StringComparison.InvariantCultureIgnoreCase"/></term> <description><see cref="StringComparer.InvariantCultureIgnoreCase"/></description></item>
		/// <item><term><see cref="StringComparison.Ordinal"/></term> <description><see cref="StringComparer.Ordinal"/></description></item>
		/// <item><term><see cref="StringComparison.OrdinalIgnoreCase"/></term> <description><see cref="StringComparer.OrdinalIgnoreCase"/></description></item>
		/// </list>
		/// </code>
		/// </summary>
		public static StringComparer ToStringComparer(this StringComparison @this)
			=> @this switch
			{
				StringComparison.CurrentCulture => StringComparer.CurrentCulture,
				StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
				StringComparison.InvariantCulture => StringComparer.InvariantCulture,
				StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
				StringComparison.Ordinal => StringComparer.Ordinal,
				StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
				_ => throw new NotImplementedException($"[{nameof(StringComparison)}] with a value of {@this.Number()} does not exist.")
			};
	}
}
