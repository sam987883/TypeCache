// Copyright (c) 2020 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace sam987883.Extensions
{
	public static class ReflectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements(this Type @this, Type type) =>
			@this == type || @this.GetInterfaces().Any(interfaceType => interfaceType == type);
	}
}