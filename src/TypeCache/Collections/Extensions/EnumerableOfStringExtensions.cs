// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class EnumerableOfStringExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value.Contains(<paramref name="character"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyContains(this IEnumerable<string>? @this, char character, StringComparison comparison = STRING_COMPARISON)
		=> @this.Any(value => value.Contains(character, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value.Contains(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyContains(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.Any(value => value.Contains(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value.Left(<paramref name="character"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyLeft(this IEnumerable<string>? @this, char character)
		=> @this.Any(value => value.Left(character));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value.Left(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyLeft(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.Any(value => value.Left(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(text =&gt; text.IsNotBlank());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyNotBlank(this IEnumerable<string?>? @this)
		=> @this.Any(value => value.IsNotBlank())!;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; !value.Contains(<paramref name="character"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyNotContains(this IEnumerable<string>? @this, char character, StringComparison comparison = STRING_COMPARISON)
		=> @this.Any(value => !value.Contains(character, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; !value.Contains(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyNotContains(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.Any(value => !value.Contains(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(<see cref="RegexOf"/>.Pattern(<paramref name="pattern"/>).IsMatch);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyRegexMatch(this IEnumerable<string>? @this, string pattern)
		=> @this.Any(RegexOf.SinglelinePattern(pattern).IsMatch);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; <see cref="RegexOf"/>.Pattern(<paramref name="pattern"/>).IsMatch(value, <paramref name="start"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyRegexMatch(this IEnumerable<string>? @this, string pattern, int start)
		=> @this.Any(value => RegexOf.SinglelinePattern(pattern).IsMatch(value, start));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value.Right(<paramref name="character"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyRight(this IEnumerable<string>? @this, char character)
		=> @this.Any(value => value.Right(character));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value.Right(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyRight(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.Any(value => value.Right(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.CountOf(<paramref name="item"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	public static int CountOf(this IEnumerable<string>? @this, string item, StringComparison comparison = STRING_COMPARISON)
		=> @this.CountOf(item, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.Left(<paramref name="length"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachLeft(this IEnumerable<string>? @this, int length)
		=> @this.Each(value => value.Left(length));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.PadLeft(<paramref name="length"/>, <paramref name="character"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachPadLeft(this IEnumerable<string>? @this, int length, char character = ' ')
		=> @this.Each(value => value.PadLeft(length, character));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.PadRight(<paramref name="length"/>, <paramref name="character"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachPadRight(this IEnumerable<string>? @this, int length, char character = ' ')
		=> @this.Each(value => value.PadRight(length, character));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.Replace(<paramref name="oldCharacter"/>, <paramref name="newCharacter"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachReplace(this IEnumerable<string>? @this, char oldCharacter, char newCharacter)
		=> @this.Each(value => value.Replace(oldCharacter, newCharacter));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.Replace(<paramref name="oldText"/>, <paramref name="newText"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachReplace(this IEnumerable<string>? @this, string oldText, string newText, StringComparison comparison = STRING_COMPARISON)
		=> @this.Each(value => value.Replace(oldText, newText, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.Substring(<paramref name="index"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachSubstring(this IEnumerable<string>? @this, int index)
		=> @this.Each(value => value.Substring(index));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.Substring(<paramref name="index"/>, <paramref name="length"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachSubstring(this IEnumerable<string>? @this, int index, int length)
		=> @this.Each(value => value.Substring(index, length));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.Trim(<paramref name="chractersToTrim"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachTrim(this IEnumerable<string>? @this, params char[]? chractersToTrim)
		=> @this.Each(value => value.Trim(chractersToTrim));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.TrimEnd(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachTrimEnd(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.Each(value => value.TrimEnd(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Each(value =&gt; value.TrimStart(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> EachTrimStart(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.Each(value => value.TrimStart(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Group(<paramref name="keyFactory"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IDictionary<string, IEnumerable<V>> Group<V>(this IEnumerable<V>? @this, Func<V, string> keyFactory, StringComparison comparison = STRING_COMPARISON)
		=> @this.Group(keyFactory, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Has(<paramref name="value"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, string value, StringComparison comparison = STRING_COMPARISON)
		=> @this.Has(value, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Has(<paramref name="values"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, IEnumerable<string>? values, StringComparison comparison = STRING_COMPARISON)
		=> @this.Has(values, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt;@<paramref name="this"/>.HasAny(<paramref name="values"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool HasAny([NotNullWhen(true)] this IEnumerable<string>? @this, IEnumerable<string>? values, StringComparison comparison = STRING_COMPARISON)
		=> @this.HasAny(values, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; value.Contains(<paramref name="character"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfContains(this IEnumerable<string>? @this, char character, StringComparison comparison = STRING_COMPARISON)
		=> @this.If(value => value.Contains(character, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; value.Contains(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfContains(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.If(value => value.Contains(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; value.Left(<paramref name="character"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfLeft(this IEnumerable<string>? @this, char character)
		=> @this.If(value => value.Left(character));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; value.Left(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfLeft(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.If(value => value.Left(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; value.IsNotBlank());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfNotBlank(this IEnumerable<string?>? @this)
		=> @this.If(value => value.IsNotBlank())!;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; !value.Contains(<paramref name="character"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfNotContains(this IEnumerable<string>? @this, char character, StringComparison comparison = STRING_COMPARISON)
		=> @this.If(value => !value.Contains(character, comparison) is true);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; !value.Contains(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfNotContains(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.If(value => !value.Contains(text, comparison) is true);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<see cref="RegexOf"/>.Pattern(<paramref name="pattern"/>).IsMatch);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfRegexMatch(this IEnumerable<string>? @this, string pattern)
		=> @this.If(RegexOf.SinglelinePattern(pattern).IsMatch);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; <see cref="RegexOf"/>.Pattern(<paramref name="pattern"/>).IsMatch(value, <paramref name="start"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfRegexMatch(this IEnumerable<string>? @this, string pattern, int start)
		=> @this.If(value => RegexOf.SinglelinePattern(pattern).IsMatch(value, start));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; value.Right(<paramref name="character"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfRight(this IEnumerable<string>? @this, char character)
		=> @this.If(value => value.Right(character));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(value =&gt; value.Right(<paramref name="text"/>, <paramref name="comparison"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string> IfRight(this IEnumerable<string>? @this, string text, StringComparison comparison = STRING_COMPARISON)
		=> @this.If(value => value.Right(text, comparison));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsSequence(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsSequence(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = STRING_COMPARISON)
		=> @this.IsSequence(items, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IsSet(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsSet(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = STRING_COMPARISON)
		=> @this.IsSet(items, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.MatchBy(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<string> Match(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = STRING_COMPARISON)
		=> @this.Match(items, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.MatchBy(<paramref name="items"/>, <paramref name="getKey"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<T> MatchBy<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, string> getKey, StringComparison comparison = STRING_COMPARISON)
		=> @this.MatchBy(items, getKey, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Maximum(<paramref name="defaultValue"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	public static string? Maximum(this IEnumerable<string>? @this, string? defaultValue = default, StringComparison comparison = STRING_COMPARISON)
		=> @this.Maximum(defaultValue, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.MaximumBy(<paramref name="getValue"/>, <paramref name="defaultValue"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T? MaximumBy<T>(this IEnumerable<T>? @this, Func<T, string> getValue, T? defaultValue = default, StringComparison comparison = STRING_COMPARISON)
		=> @this.MaximumBy(getValue, defaultValue, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Minimum(<paramref name="defaultValue"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	public static string? Minimum(this IEnumerable<string>? @this, string? defaultValue = default, StringComparison comparison = STRING_COMPARISON)
		=> @this.Minimum(defaultValue, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.MinimumBy(<paramref name="getValue"/>, <paramref name="defaultValue"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T? MinimumBy<T>(this IEnumerable<T>? @this, Func<T, string> getValue, T? defaultValue = default, StringComparison comparison = STRING_COMPARISON)
		=> @this.MinimumBy(getValue, defaultValue, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.NotMatch(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<string> NotMatch(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = STRING_COMPARISON)
		=> @this.NotMatch(items, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.NotMatchBy(<paramref name="items"/>, <paramref name="getKey"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<T> NotMatchBy<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, string> getKey, StringComparison comparison = STRING_COMPARISON)
		=> @this.NotMatchBy(items, getKey, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Sort(<paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string[] Sort(this IEnumerable<string>? @this, StringComparison comparison = SORT_STRING_COMPARISON)
		=> @this.Sort(comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionary(<paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IDictionary<string, V> ToDictionary<V>(this IEnumerable<KeyValuePair<string, V>>? @this, StringComparison comparison = STRING_COMPARISON)
		=> @this.ToDictionary(comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionaryByKey(<paramref name="keyFactory"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IDictionary<string, V> ToDictionaryByKey<V>(this IEnumerable<V>? @this, Func<V, string> keyFactory, StringComparison comparison = STRING_COMPARISON)
		=> @this.ToDictionaryByKey(keyFactory, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionaryByValue(<paramref name="valueFactory"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IDictionary<string, V> ToDictionaryByValue<V>(this IEnumerable<string>? @this, Func<string, V> valueFactory, StringComparison comparison = STRING_COMPARISON)
		=> @this.ToDictionaryByValue(valueFactory, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionary(<paramref name="keyFactory"/>, <paramref name="valueFactory"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IDictionary<string, V> ToDictionary<T, V>(this IEnumerable<T>? @this, Func<T, string> keyFactory, Func<T, V> valueFactory, StringComparison comparison = STRING_COMPARISON)
		=> @this.ToDictionary(keyFactory, valueFactory, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToHashSet(<paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<string> ToHashSet(this IEnumerable<string>? @this, StringComparison comparison = STRING_COMPARISON)
		=> @this.ToHashSet(comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<T> ToHashSetBy<T>(this IEnumerable<T>? @this, Func<T, string> getKey, StringComparison comparison = STRING_COMPARISON)
		=> @this.ToHashSetBy(getKey, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyComparison"/>.ToStringComparer())
	/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(
		this IEnumerable<KeyValuePair<string, V>>? @this
		, StringComparison keyComparison = STRING_COMPARISON)
		=> @this?.ToImmutableDictionary(keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyFactory"/>, <paramref name="keyComparison"/>.ToStringComparer())
	/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(
		this IEnumerable<V>? @this
		, Func<V, string> keyFactory
		, StringComparison keyComparison = STRING_COMPARISON)
		=> @this?.ToImmutableDictionary(keyFactory, keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyFactory"/>, <paramref name="valueFactory"/>, <paramref name="keyComparison"/>.ToStringComparer())
	/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableDictionary<string, V> ToImmutableDictionary<T, V>(
		this IEnumerable<T>? @this
		, Func<T, string> keyFactory
		, Func<T, V> valueFactory
		, StringComparison keyComparison = STRING_COMPARISON)
		=> @this?.ToImmutableDictionary(keyFactory, valueFactory, keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToImmutableHashSet(<paramref name="comparison"/>.ToStringComparer()) ?? <see cref="ImmutableHashSet.Create{T}(IEqualityComparer{T}?)"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableHashSet<string> ToImmutableHashSet(this IEnumerable<string>? @this, StringComparison comparison = STRING_COMPARISON)
		=> @this?.ToImmutableHashSet(comparison.ToStringComparer()) ?? ImmutableHashSet.Create<string>(comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToImmutableSortedDictionary(<paramref name="keyComparison"/>.ToStringComparer(), <paramref name="valueComparer"/>)
	/// ?? <see cref="ImmutableSortedDictionary.Create{TKey, TValue}(IComparer{TKey}?, IEqualityComparer{TValue}?)"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableSortedDictionary<string, V> ToImmutableSortedDictionary<V>(
		this IEnumerable<KeyValuePair<string, V>>? @this
		, StringComparison keyComparison = SORT_STRING_COMPARISON
		, IEqualityComparer<V>? valueComparer = null)
		=> @this?.ToImmutableSortedDictionary(keyComparison.ToStringComparer(), valueComparer)
			?? ImmutableSortedDictionary.Create<string, V>(keyComparison.ToStringComparer(), valueComparer);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToImmutableSortedSet(<paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableSortedSet<string> ToImmutableSortedSet(this IEnumerable<string>? @this, StringComparison comparison = SORT_STRING_COMPARISON)
		=> @this?.ToImmutableSortedSet(comparison.ToStringComparer()) ?? ImmutableSortedSet.Create<string>(comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToIndex(<paramref name="value"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<int> ToIndex(this IEnumerable<string>? @this, string value, StringComparison comparison = STRING_COMPARISON)
		=> @this.ToIndex(value, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Length);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<int> ToLength(this IEnumerable<string>? @this)
		=> @this.Map(value => value.Length);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; <see cref="RegexOf"/>.Pattern(<paramref name="pattern"/>).Split(value));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToRegexSplit(this IEnumerable<string>? @this, string pattern)
		=> @this.Map(value => RegexOf.SinglelinePattern(pattern).Split(value));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; <see cref="RegexOf"/>.Pattern(<paramref name="pattern"/>).Split(value, <paramref name="count"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToRegexSplit(this IEnumerable<string>? @this, string pattern, int count)
		=> @this.Map(value => RegexOf.SinglelinePattern(pattern).Split(value, count));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; <see cref="RegexOf"/>.Pattern(<paramref name="pattern"/>).Split(value, <paramref name="count"/>, <paramref name="start"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToRegexSplit(this IEnumerable<string>? @this, string pattern, int count, int start)
		=> @this.Map(value => RegexOf.SinglelinePattern(pattern).Split(value, count, start));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, char separator, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separator, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, char[] separators, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separators, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, string separator, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separator, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, string[] separators, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separators, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="count"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, char separator, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separator, count, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="count"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, char[] separators, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separators, count, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="count"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, string separator, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separator, count, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="count"/>, <paramref name="options"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, string[] separators, StringSplitOptions options = StringSplitOptions.None)
		=> @this.Map(value => value.Split(separators, count, options));

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Union(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<string> Union(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = STRING_COMPARISON)
		=> @this.Union(items, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.UnionBy(<paramref name="items"/>, <paramref name="getKey"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<T> UnionBy<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, string> getKey, StringComparison comparison = STRING_COMPARISON)
		=> @this.UnionBy(items, getKey, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Without(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<string> Without(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = STRING_COMPARISON)
		=> @this.Without(items, comparison.ToStringComparer());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.WithoutBy(<paramref name="items"/>, <paramref name="getKey"/>, <paramref name="comparison"/>.ToStringComparer());</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static HashSet<T> WithoutBy<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, string> getKey, StringComparison comparison = STRING_COMPARISON)
		=> @this.WithoutBy(items, getKey, comparison.ToStringComparer());
}
