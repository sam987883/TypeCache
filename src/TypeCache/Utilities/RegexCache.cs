// Copyright (c) 2021 Samuel Abraham

using System.Text.RegularExpressions;
using TypeCache.Collections;
using static System.Text.RegularExpressions.RegexOptions;

namespace TypeCache.Utilities;

public static class RegexCache
{
	private static readonly TimeSpan DefaultMatchTimeout = TimeSpan.FromMinutes(1);

	private static readonly IReadOnlyDictionary<(string Pattern, RegexOptions Options), Regex> Cache =
		new LazyDictionary<(string Pattern, RegexOptions Options), Regex>(_ => new(_.Pattern, _.Options, DefaultMatchTimeout));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Regex Regex([StringSyntax(StringSyntaxAttribute.Regex)] this string @this, RegexOptions options = Compiled | CultureInvariant | Singleline)
		=> Cache[(@this, options)];
}
