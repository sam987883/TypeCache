// Copyright (c) 2020 Samuel Abraham

using System;
using System.Reflection;

namespace Sam987883.Reflection
{
	public static class TypeCache
	{
		public const BindingFlags INSTANCE_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		internal static StringComparer NameComparer { get; } = StringComparer.Ordinal;
	}
}
