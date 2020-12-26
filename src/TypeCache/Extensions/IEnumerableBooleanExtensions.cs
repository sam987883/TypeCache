// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class IEnumerableBooleanExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool All(this IEnumerable<bool>? @this, bool value)
			=> !@this.If(item => item != value).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any([NotNullWhen(true)] this IEnumerable<bool>? @this, bool value)
			=> @this.If(item => item == value).Any();
	}
}
