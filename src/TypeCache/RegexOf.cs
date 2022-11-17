// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TypeCache.Collections;
using static TypeCache.Default;

namespace TypeCache
{
	public static class RegexOf
	{
		private static readonly IReadOnlyDictionary<string, Regex> MultilineRegex = new LazyDictionary<string, Regex>(pattern =>
			new Regex(pattern, REGEX_OPTIONS | RegexOptions.Multiline, RegexTimeout), comparer: StringComparer.Ordinal);

		private static readonly IReadOnlyDictionary<string, Regex> SinglelineRegex = new LazyDictionary<string, Regex>(pattern =>
			new Regex(pattern, REGEX_OPTIONS | RegexOptions.Singleline, RegexTimeout), comparer: StringComparer.Ordinal);

		[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
		public static Regex MultilinePattern([StringSyntax(StringSyntaxAttribute.Regex)] this string @this)
			=> MultilineRegex[@this];

		[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
		public static Regex SinglelinePattern([StringSyntax(StringSyntaxAttribute.Regex)] this string @this)
			=> SinglelineRegex[@this];
	}
}
