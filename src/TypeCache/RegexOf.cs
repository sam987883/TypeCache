// Copyright (c) 2021 Samuel Abraham

using System.Text.RegularExpressions;
using TypeCache.Collections;

namespace TypeCache
{
	public static class RegexOf
	{
		private static readonly TimeSpan RegexTimeout = TimeSpan.FromMinutes(1);

		private static readonly IReadOnlyDictionary<string, Regex> MultilineRegex = new LazyDictionary<string, Regex>(pattern =>
			new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline, RegexTimeout), comparer: StringComparer.Ordinal);

		private static readonly IReadOnlyDictionary<string, Regex> SinglelineRegex = new LazyDictionary<string, Regex>(pattern =>
			new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline, RegexTimeout), comparer: StringComparer.Ordinal);

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public static Regex MultilinePattern([StringSyntax(StringSyntaxAttribute.Regex)] this string @this)
			=> MultilineRegex[@this];

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public static Regex SinglelinePattern([StringSyntax(StringSyntaxAttribute.Regex)] this string @this)
			=> SinglelineRegex[@this];
	}
}
