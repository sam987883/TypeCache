// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sam987883.Common.Extensions
{
	public static class IEnumerableStringExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static StringComparer GetStringOrdinalComparer(bool compareCase)
			=> compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, string value, bool compareCase = false)
			=> @this.Has(value, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has([NotNullWhen(true)] this IEnumerable<string>? @this, IEnumerable<string>? values, bool compareCase = false)
			=> @this.Has(values, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> IfNotBlank(this IEnumerable<string?>? @this)
			=> @this.If(_ => !_.IsBlank());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(this IEnumerable<string>? @this, IEnumerable<string>? items, bool compareCase = false)
			=> @this.Is(items, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Match(this IEnumerable<string>? @this, IEnumerable<string>? items, bool compareCase = false)
			=> @this.Match(items, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (string Text, bool Exists) Maximum(this IEnumerable<string>? @this, bool compareCase = false)
			=> @this.Maximum(GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (string Text, bool Exists) Minimum(this IEnumerable<string>? @this, bool compareCase = false)
			=> @this.Minimum(GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Neither(this IEnumerable<string>? @this, IEnumerable<string>? items, bool compareCase = false)
			=> @this.Neither(items, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> Sort(this IEnumerable<string>? @this)
			=> @this.Sort(StringComparer.Ordinal);

		public static string ToCsv(this IEnumerable<string>? @this)
			=> @this.Any() ? string.Join(',', @this.To(text => !text.IsBlank() ? text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\"") : string.Empty)) : string.Empty;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<V>(this IEnumerable<string>? @this, Func<string, V> valueFactory, bool compareCase = false)
			=> @this.ToDictionary(valueFactory, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<string, V> ToDictionary<T, V>(this IEnumerable<T>? @this, Func<T, string> keyFactory, Func<T, V> valueFactory, bool compareCase = false)
			=> @this.ToDictionary(keyFactory, valueFactory, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> ToHashSet(this IEnumerable<string>? @this, bool compareCase = false)
			=> @this.ToHashSet(GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, V> ToImmutable<V>(this IEnumerable<KeyValuePair<string, V>>? @this, bool compareCase = false)
			=> @this.ToImmutable(GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableDictionary<string, V> ToImmutable<V>(this IEnumerable<KeyValuePair<string, V>>? @this, IEqualityComparer<V> valueComparer, bool compareCase = false)
			=> @this.ToImmutable(GetStringOrdinalComparer(compareCase), valueComparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex(this IEnumerable<string>? @this, string value, bool compareCase = false)
			=> @this.ToIndex(value, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Union(this IEnumerable<string>? @this, IEnumerable<string>? items, bool compareCase = false)
			=> @this.Union(items, GetStringOrdinalComparer(compareCase));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<string> Without(this IEnumerable<string>? @this, IEnumerable<string>? items, bool compareCase = false)
			=> @this.Without(items, GetStringOrdinalComparer(compareCase));
	}
}
