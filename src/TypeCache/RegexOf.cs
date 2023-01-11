﻿// Copyright (c) 2021 Samuel Abraham

using System.Text.RegularExpressions;
using TypeCache.Collections;
using static System.StringComparer;
using static System.Text.RegularExpressions.RegexOptions;

namespace TypeCache;

public static class RegexOf
{
	private static readonly TimeSpan RegexTimeout = TimeSpan.FromMinutes(1);

	private static readonly IReadOnlyDictionary<string, Regex> MultilineRegex = new LazyDictionary<string, Regex>(pattern =>
		new Regex(pattern, Compiled | CultureInvariant | Multiline, RegexTimeout), comparer: Ordinal);

	private static readonly IReadOnlyDictionary<string, Regex> SinglelineRegex = new LazyDictionary<string, Regex>(pattern =>
		new Regex(pattern, Compiled | CultureInvariant | Singleline, RegexTimeout), comparer: Ordinal);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Regex MultilinePattern([StringSyntax(StringSyntaxAttribute.Regex)] this string @this)
		=> MultilineRegex[@this];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Regex SinglelinePattern([StringSyntax(StringSyntaxAttribute.Regex)] this string @this)
		=> SinglelineRegex[@this];
}
