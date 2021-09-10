// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TypeCache
{
	public static class Default
	{
		public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

		public const string DATASOURCE = "Default";

		public const BindingFlags INSTANCE_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		public const StringComparison NAME_STRING_COMPARISON = StringComparison.Ordinal;

		public const RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

		public const StringComparison SORT_STRING_COMPARISON = StringComparison.Ordinal;

		public const BindingFlags STATIC_BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

		public const StringComparison STRING_COMPARISON = StringComparison.OrdinalIgnoreCase;
	}
}
