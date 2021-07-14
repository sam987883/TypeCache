// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions
{
	public static class IEnumerableStringExtensions
	{
		/// <summary>
		/// <c>@<paramref name="this"/>.Group(<paramref name="keyFactory"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDictionary<string, IEnumerable<V>> Group<V>(this IEnumerable<V>? @this, Func<V, string> keyFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Group(keyFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Has(<paramref name="value"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Has(value, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Has(<paramref name="values"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, IEnumerable<string>? values, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Has(values, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.If(text => !text.IsBlank())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfNotBlank(this IEnumerable<string?>? @this)
			=> @this.If(text => !text.IsBlank())!;

		/// <summary>
		/// <c>@<paramref name="this"/>.IsSequence(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSequence(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.IsSequence(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.IsSet(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSet(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.IsSet(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Match(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Match(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Match(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Neither(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> NotMatch(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.NotMatch(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Sort(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string[] Sort(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.Ordinal)
			=> @this.Sort(comparison.ToStringComparer());

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.Any()<br/>
		/// ? string.Join(',', @this.To(text => !text.IsBlank()? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : string.Empty))<br/>
		/// : string.Empty
		/// </code>
		/// </summary>
		public static string ToCSV(this IEnumerable<string>? @this)
			=> @this.Any()
				? string.Join(',', @this.To(text => !text.IsBlank() ? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : string.Empty))
				: string.Empty;

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<KeyValuePair<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<Tuple<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<ValueTuple<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="valueFactory"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<string>? @this, Func<string, V> valueFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(valueFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToDictionary(<paramref name="keyFactory"/>, <paramref name="valueFactory"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<T, V>(this IEnumerable<T>? @this, Func<T, string> keyFactory, Func<T, V> valueFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(keyFactory, valueFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToHashSet(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> ToHashSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToHashSet(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyComparison"/>.ToStringComparer())
		/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(
			this IEnumerable<KeyValuePair<string, V>>? @this
			, StringComparison keyComparison = StringComparison.OrdinalIgnoreCase)
			=> @this?.ToImmutableDictionary(keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>?.ToImmutableDictionary(<paramref name="keyFactory"/>, <paramref name="keyComparison"/>.ToStringComparer())
		/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(
			this IEnumerable<V>? @this
			, Func<V, string> keyFactory
			, StringComparison keyComparison = StringComparison.OrdinalIgnoreCase)
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
			, StringComparison keyComparison = StringComparison.OrdinalIgnoreCase)
			=> @this?.ToImmutableDictionary(keyFactory, valueFactory, keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToImmutableHashSet(<paramref name="comparison"/>.ToStringComparer()) ?? <see cref="ImmutableHashSet.Create{T}(IEqualityComparer{T}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<string> ToImmutableHashSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this?.ToImmutableHashSet(comparison.ToStringComparer()) ?? ImmutableHashSet.Create<string>(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToImmutableSortedDictionary(<paramref name="keyComparison"/>.ToStringComparer(), <paramref name="valueComparer"/>)
		/// ?? <see cref="ImmutableSortedDictionary.Create{TKey, TValue}(IComparer{TKey}?, IEqualityComparer{TValue}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedDictionary<string, V> ToImmutableSortedDictionary<V>(
			this IEnumerable<KeyValuePair<string, V>>? @this
			, StringComparison keyComparison = StringComparison.Ordinal
			, IEqualityComparer<V>? valueComparer = null)
			=> @this?.ToImmutableSortedDictionary(keyComparison.ToStringComparer(), valueComparer) ?? ImmutableSortedDictionary.Create<string, V>(keyComparison.ToStringComparer(), valueComparer);

		/// <summary>
		/// <c>@<paramref name="this"/>.ToImmutableSortedSet(<paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<string> ToImmutableSortedSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.Ordinal)
			=> @this?.ToImmutableSortedSet(comparison.ToStringComparer()) ?? ImmutableSortedSet.Create<string>(comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.ToIndex(<paramref name="value"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex(this IEnumerable<string>? @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToIndex(value, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Union(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Union(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Union(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@<paramref name="this"/>.Without(<paramref name="items"/>, <paramref name="comparison"/>.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Without(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Without(items, comparison.ToStringComparer());
	}
}
