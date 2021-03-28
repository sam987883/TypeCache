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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDictionary<string, IEnumerable<V>> Group<V>(this IEnumerable<V>? @this, Func<V, string> keyFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Group(keyFactory, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Has(value, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, IEnumerable<string>? values, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> values.Has(values, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfNotBlank(this IEnumerable<string?>? @this)
			=> @this.If(text => !text.IsBlank())!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSequence(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.IsSequence(items, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSet(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.IsSet(items, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Match(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Match(items, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Neither(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Neither(items, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string[] Sort(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.Ordinal)
			=> @this.Sort(comparison.ToStringComparer());

		public static string ToCsv(this IEnumerable<string>? @this)
			=> @this.Any()
				? string.Join(',', @this.To(text => !text.IsBlank() ? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : string.Empty))
				: string.Empty;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<KeyValuePair<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<Tuple<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<ValueTuple<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<string>? @this, Func<string, V> valueFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(valueFactory, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<T, V>(this IEnumerable<T>? @this, Func<T, string> keyFactory, Func<T, V> valueFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(keyFactory, valueFactory, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> ToHashSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToHashSet(comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(this IEnumerable<KeyValuePair<string, V>>? @this, StringComparison keyComparison = StringComparison.OrdinalIgnoreCase, IEqualityComparer<V>? valueComparer = null)
			=> @this.ToImmutableDictionary(keyComparison.ToStringComparer(), valueComparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<string> ToImmutableHashSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToImmutableHashSet(comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedDictionary<string, V> ToImmutableSortedDictionary<V>(this IEnumerable<KeyValuePair<string, V>>? @this, StringComparison keyComparison = StringComparison.Ordinal, IEqualityComparer<V>? valueComparer = null)
			=> @this.ToImmutableSortedDictionary(keyComparison.ToStringComparer(), valueComparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<string> ToImmutableSortedSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.Ordinal)
			=> @this.ToImmutableSortedSet(comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex(this IEnumerable<string>? @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToIndex(value, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Union(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Union(items, comparison.ToStringComparer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Without(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Without(items, comparison.ToStringComparer());
	}
}
