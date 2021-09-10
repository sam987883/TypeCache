// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions
{
	public static class IEnumerableStringExtensions
	{
		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.Left(<paramref name="length"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachLeft(this IEnumerable<string>? @this, int length)
			=> @this.Each(value => value.Left(length));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.PadLeft(<paramref name="length"/>, <paramref name="character"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachPadLeft(this IEnumerable<string>? @this, int length, char character = ' ')
			=> @this.Each(value => value.PadLeft(length, character));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.PadRight(<paramref name="length"/>, <paramref name="character"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachPadRight(this IEnumerable<string>? @this, int length, char character = ' ')
			=> @this.Each(value => value.PadRight(length, character));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.Replace(<paramref name="oldCharacter"/>, <paramref name="newCharacter"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachReplace(this IEnumerable<string>? @this, char oldCharacter, char newCharacter)
			=> @this.Each(value => value.Replace(oldCharacter, newCharacter));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.Replace(<paramref name="oldText"/>, <paramref name="newText"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachReplace(this IEnumerable<string>? @this, string oldText, string newText, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Each(value => value.Replace(oldText, newText, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.RegexReplace(<paramref name="pattern"/>, <paramref name="evaluator"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachRegexReplace(this IEnumerable<string>? @this, string pattern, MatchEvaluator evaluator, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.Each(value => value.RegexReplace(pattern, evaluator, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.RegexReplace(<paramref name="pattern"/>, <paramref name="evaluator"/>, <paramref name="timeout"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachRegexReplace(this IEnumerable<string>? @this, string pattern, MatchEvaluator evaluator, TimeSpan timeout, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.Each(value => value.RegexReplace(pattern, evaluator, timeout, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.RegexReplace(<paramref name="pattern"/>, <paramref name="replacement"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachRegexReplace(this IEnumerable<string>? @this, string pattern, string replacement, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.Each(value => value.RegexReplace(pattern, replacement, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.RegexReplace(<paramref name="pattern"/>, <paramref name="replacement"/>, <paramref name="timeout"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachRegexReplace(this IEnumerable<string>? @this, string pattern, string replacement, TimeSpan timeout, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.Each(value => value.RegexReplace(pattern, replacement, timeout, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.Replace(<paramref name="oldText"/>, <paramref name="newText"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachRegexReplace(this IEnumerable<string>? @this, string oldText, string newText, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Each(value => value.Replace(oldText, newText, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.Substring(<paramref name="index"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachSubstring(this IEnumerable<string>? @this, int index)
			=> @this.Each(value => value.Substring(index));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.Substring(<paramref name="index"/>, <paramref name="length"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachSubstring(this IEnumerable<string>? @this, int index, int length)
			=> @this.Each(value => value.Substring(index, length));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.Trim(<paramref name="chractersToTrim"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachTrim(this IEnumerable<string>? @this, params char[]? chractersToTrim)
			=> @this.Each(value => value.Trim(chractersToTrim));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.TrimEnd(<paramref name="text"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachTrimEnd(this IEnumerable<string>? @this, string text, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Each(value => value.TrimEnd(text, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.Each(value =&gt; value.TrimStart(<paramref name="text"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> EachTrimStart(this IEnumerable<string>? @this, string text, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Each(value => value.TrimStart(text, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.Group(<paramref name="keyFactory"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDictionary<string, IEnumerable<V>> Group<V>(this IEnumerable<V>? @this, Func<V, string> keyFactory, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Group(keyFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Has(<paramref name="value"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, string value, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Has(value, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Has(<paramref name="values"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, IEnumerable<string>? values, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Has(values, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.If(value =&gt; value.Contains(<paramref name="character"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfContains(this IEnumerable<string>? @this, char character, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.If(value => value.Contains(character, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.If(value =&gt; value.Contains(<paramref name="text"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfContains(this IEnumerable<string>? @this, string text, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.If(value => value.Contains(text, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.IfLeft(value =&gt; value.Left(<paramref name="character"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfLeft(this IEnumerable<string>? @this, char character)
			=> @this.If(value => value.Left(character));

		/// <summary>
		/// <c>@<paramref name="this"/>.IfLeft(value =&gt; value.Left(<paramref name="text"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfLeft(this IEnumerable<string>? @this, string text, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.If(value => value.Left(text, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.If(text =&gt; !text.IsBlank())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfNotBlank(this IEnumerable<string?>? @this)
			=> @this.If(value => !value.IsBlank())!;

		/// <summary>
		/// <c>@<paramref name="this"/>.If(value =&gt; !value.Contains(<paramref name="character"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfNotContains(this IEnumerable<string>? @this, char character, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.If(value => !value.Contains(character, comparison) is true);

		/// <summary>
		/// <c>@<paramref name="this"/>.If(value =&gt; !value.Contains(<paramref name="text"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfNotContains(this IEnumerable<string>? @this, string text, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.If(value => !value.Contains(text, comparison) is true);

		/// <summary>
		/// <c>@<paramref name="this"/>.If(value =&gt; value.RegexIsMatch(<paramref name="pattern"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfRegexMatch(this IEnumerable<string>? @this, string pattern, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.If(value => value.RegexIsMatch(pattern, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.If(value =&gt; value.RegexIsMatch(<paramref name="pattern"/>, <paramref name="timeout"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfRegexMatch(this IEnumerable<string>? @this, string pattern, TimeSpan timeout, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.If(value => value.RegexIsMatch(pattern, timeout, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.IfRight(value =&gt; value.Right(<paramref name="character"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfRight(this IEnumerable<string>? @this, char character)
			=> @this.If(value => value.Right(character));

		/// <summary>
		/// <c>@<paramref name="this"/>.IfRight(value =&gt; value.Right(<paramref name="text"/>, <paramref name="comparison"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfRight(this IEnumerable<string>? @this, string text, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.If(value => value.Right(text, comparison));

		/// <summary>
		/// <c>@<paramref name="this"/>.IsSequence(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSequence(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.IsSequence(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.IsSet(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSet(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.IsSet(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Match(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Match(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Match(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Neither(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> NotMatch(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.NotMatch(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Sort(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string[] Sort(this IEnumerable<string>? @this, StringComparison comparison = Default.SORT_STRING_COMPARISON)
			=> @this.Sort(comparison.ToStringComparer());

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.Any()<br/>
		/// ? @this.To(text => !text.IsBlank() ? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : <see cref="string.Empty"/>).Join(',')<br/>
		/// : <see cref="string.Empty"/>
		/// </code>
		/// </summary>
		public static string ToCSV(this IEnumerable<string>? @this)
			=> @this.Any()
				? @this.To(text => !text.IsBlank() ? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : string.Empty).Join(',')
				: string.Empty;

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<KeyValuePair<string, V>>? @this, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<Tuple<string, V>>? @this, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<ValueTuple<string, V>>? @this, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="valueFactory"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<string>? @this, Func<string, V> valueFactory, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.ToDictionary(valueFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="keyFactory"/>, <paramref name="valueFactory"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<T, V>(this IEnumerable<T>? @this, Func<T, string> keyFactory, Func<T, V> valueFactory, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.ToDictionary(keyFactory, valueFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToHashSet(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> ToHashSet(this IEnumerable<string>? @this, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.ToHashSet(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyComparison"/>.ToStringComparer())
		/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(
			this IEnumerable<KeyValuePair<string, V>>? @this
			, StringComparison keyComparison = Default.STRING_COMPARISON)
			=> @this?.ToImmutableDictionary(keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyFactory"/>, <paramref name="keyComparison"/>.ToStringComparer())
		/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(
			this IEnumerable<V>? @this
			, Func<V, string> keyFactory
			, StringComparison keyComparison = Default.STRING_COMPARISON)
			=> @this?.ToImmutableDictionary(keyFactory, keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyFactory"/>, <paramref name="valueFactory"/>, <paramref name="keyComparison"/>.ToStringComparer())
		/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<T, V>(
			this IEnumerable<T>? @this
			, Func<T, string> keyFactory
			, Func<T, V> valueFactory
			, StringComparison keyComparison = Default.STRING_COMPARISON)
			=> @this?.ToImmutableDictionary(keyFactory, valueFactory, keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToImmutableHashSet(<paramref name="comparison"/>.ToStringComparer()) ?? <see cref="ImmutableHashSet.Create{T}(IEqualityComparer{T}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<string> ToImmutableHashSet(this IEnumerable<string>? @this, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this?.ToImmutableHashSet(comparison.ToStringComparer()) ?? ImmutableHashSet.Create<string>(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToImmutableSortedDictionary(<paramref name="keyComparison"/>.ToStringComparer(), <paramref name="valueComparer"/>)
		/// ?? <see cref="ImmutableSortedDictionary.Create{TKey, TValue}(IComparer{TKey}?, IEqualityComparer{TValue}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedDictionary<string, V> ToImmutableSortedDictionary<V>(
			this IEnumerable<KeyValuePair<string, V>>? @this
			, StringComparison keyComparison = Default.SORT_STRING_COMPARISON
			, IEqualityComparer<V>? valueComparer = null)
			=> @this?.ToImmutableSortedDictionary(keyComparison.ToStringComparer(), valueComparer)
				?? ImmutableSortedDictionary.Create<string, V>(keyComparison.ToStringComparer(), valueComparer);

		/// <summary>
		/// <c>@<paramref name="this"/>.ToImmutableSortedSet(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<string> ToImmutableSortedSet(this IEnumerable<string>? @this, StringComparison comparison = Default.SORT_STRING_COMPARISON)
			=> @this?.ToImmutableSortedSet(comparison.ToStringComparer()) ?? ImmutableSortedSet.Create<string>(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToIndex(<paramref name="value"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex(this IEnumerable<string>? @this, string value, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.ToIndex(value, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Length)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToLength(this IEnumerable<string>? @this)
			=> @this.To(value => value.Length);

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.RegexSplit(<paramref name="pattern"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToRegexSplit(this IEnumerable<string>? @this, string pattern, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.To(value => value.RegexSplit(pattern, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.RegexSplit(<paramref name="pattern"/>, <paramref name="timeout"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToRegexSplit(this IEnumerable<string>? @this, string pattern, TimeSpan timeout, RegexOptions options = Default.REGEX_OPTIONS)
			=> @this.To(value => value.RegexSplit(pattern, timeout, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, char separator, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separator, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, char[] separators, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separators, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, string separator, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separator, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, string[] separators, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separators, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="count"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, char separator, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separator, count, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="count"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, char[] separators, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separators, count, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separator"/>, <paramref name="count"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, string separator, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separator, count, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.To(value =&gt; value.Split(<paramref name="separators"/>, <paramref name="count"/>, <paramref name="options"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string[]> ToSplit(this IEnumerable<string>? @this, int count, string[] separators, StringSplitOptions options = StringSplitOptions.None)
			=> @this.To(value => value.Split(separators, count, options));

		/// <summary>
		/// <c>@<paramref name="this"/>.Union(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Union(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Union(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Without(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Without(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = Default.STRING_COMPARISON)
			=> @this.Without(items, comparison.ToStringComparer());
	}
}
