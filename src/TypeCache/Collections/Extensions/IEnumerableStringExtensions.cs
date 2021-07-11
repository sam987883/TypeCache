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
		/// <c>@this.Group(keyFactory, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDictionary<string, IEnumerable<V>> Group<V>(this IEnumerable<V>? @this, Func<V, string> keyFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Group(keyFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.Has(value, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Has(value, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.Has(value, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, IEnumerable<string>? values, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Has(values, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.If(text => !text.IsBlank())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfNotBlank(this IEnumerable<string?>? @this)
			=> @this.If(text => !text.IsBlank())!;

		/// <summary>
		/// <c>@this.IsSequence(items, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSequence(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.IsSequence(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.IsSet(items, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSet(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.IsSet(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.Match(items, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Match(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Match(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.Neither(items, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Neither(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Neither(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.Sort(comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string[] Sort(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.Ordinal)
			=> @this.Sort(comparison.ToStringComparer());

		/// <summary>
		/// <code>
		/// @this.Any()<br/>
		/// ? string.Join(',', @this.To(text => !text.IsBlank()? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : string.Empty))<br/>
		/// : string.Empty
		/// </code>
		/// </summary>
		public static string ToCSV(this IEnumerable<string>? @this)
			=> @this.Any()
				? string.Join(',', @this.To(text => !text.IsBlank() ? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : string.Empty))
				: string.Empty;

		/// <summary>
		/// <c>@this.ToDictionary(comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<KeyValuePair<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.ToDictionary(comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<ValueTuple<string, V>>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.ToDictionary(valueFactory, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<string>? @this, Func<string, V> valueFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(valueFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.ToDictionary(keyFactory, valueFactory, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<T, V>(this IEnumerable<T>? @this, Func<T, string> keyFactory, Func<T, V> valueFactory, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToDictionary(keyFactory, valueFactory, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.ToHashSet(comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> ToHashSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToHashSet(comparison.ToStringComparer());

		/// <summary>
		/// <c>@this?.ToImmutableDictionary(<paramref name="keyComparison"/>.ToStringComparer())
		/// ?? ImmutableDictionary.Create&lt;<see cref="string"/>, <typeparamref name="V"/>&gt;(<paramref name="keyComparison"/>)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<V>(
			this IEnumerable<KeyValuePair<string, V>>? @this
			, StringComparison keyComparison = StringComparison.OrdinalIgnoreCase)
			=> @this != null ? ImmutableDictionary.CreateRange(keyComparison.ToStringComparer(), @this) : ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

		/// <summary>
		/// <c>@this?.ToImmutableDictionary(keyFactory, valueFactory, keyComparison.ToStringComparer()) ?? <see cref="ImmutableDictionary{K, V}.Empty"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableDictionary<string, V> ToImmutableDictionary<T, V>(
			this IEnumerable<T>? @this
			, Func<T, string> keyFactory
			, Func<T, V> valueFactory
			, StringComparison keyComparison = StringComparison.OrdinalIgnoreCase)
			=> @this?.ToImmutableDictionary(keyFactory, valueFactory, keyComparison.ToStringComparer()) ?? ImmutableDictionary.Create<string, V>(keyComparison.ToStringComparer());

		/// <summary>
		/// <c>@this.ToImmutableHashSet(comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableHashSet<string> ToImmutableHashSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this?.ToImmutableHashSet(comparison.ToStringComparer()) ?? ImmutableHashSet.Create<string>(comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.ToImmutableSortedDictionary(keyComparison.ToStringComparer(), valueComparer)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedDictionary<string, V> ToImmutableSortedDictionary<V>(
			this IEnumerable<KeyValuePair<string, V>>? @this
			, StringComparison keyComparison = StringComparison.Ordinal
			, IEqualityComparer<V>? valueComparer = null)
			=> @this?.ToImmutableSortedDictionary(keyComparison.ToStringComparer(), valueComparer) ?? ImmutableSortedDictionary.Create<string, V>(keyComparison.ToStringComparer(), valueComparer);

		/// <summary>
		/// <c>@this.ToImmutableSortedSet(comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableSortedSet<string> ToImmutableSortedSet(this IEnumerable<string>? @this, StringComparison comparison = StringComparison.Ordinal)
			=> @this?.ToImmutableSortedSet(comparison.ToStringComparer()) ?? ImmutableSortedSet.Create<string>(comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.ToIndex(value, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex(this IEnumerable<string>? @this, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.ToIndex(value, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.Union(items, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Union(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Union(items, comparison.ToStringComparer());

		/// <summary>
		/// <c>@this.Without(items, comparison.ToStringComparer())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Without(this IEnumerable<string>? @this, IEnumerable<string>? items, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
			=> @this.Without(items, comparison.ToStringComparer());
	}
}
