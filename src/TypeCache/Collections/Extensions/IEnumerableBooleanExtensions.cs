// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections.Extensions
{
	public static class IEnumerableBooleanExtensions
	{
		/// <summary>
		/// <c>!@<paramref name="this"/>.If(item => item != <paramref name="value"/>).Any()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool All(this IEnumerable<bool>? @this, bool value)
			=> !@this.If(item => item != value).Any();

		/// <summary>
		/// <c>@<paramref name="this"/>.If(item => item == <paramref name="value"/>).Any()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any([NotNullWhen(true)] this IEnumerable<bool>? @this, bool value)
			=> @this.If(item => item == value).Any();
	}
}
