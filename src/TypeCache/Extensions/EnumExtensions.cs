// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static class EnumExtensions
{
	[DebuggerHidden]
	public static IReadOnlyCollection<Attribute> Attributes<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.FirstOrDefault(token => EnumOf<T>.Comparer.EqualTo(@this, token.Value))?.Attributes ?? Array<Attribute>.Empty;

	[DebuggerHidden]
	public static bool HasAnyFlag<T>(this T @this, params T[] flags)
		where T : struct, Enum
		=> flags?.Any(flag => @this.HasFlag(flag)) ?? false;

	[DebuggerHidden]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.FirstOrDefault(token => EnumOf<T>.Comparer.EqualTo(@this, token.Value))?.Hex ?? @this.ToString("X");

	[DebuggerHidden]
	public static bool IsAny<T>(this T @this, params T[] tokens)
		where T : struct, Enum
		=> tokens.Any(token => EnumOf<T>.Comparer.EqualTo(@this, token));

	[DebuggerHidden]
	public static bool IsDefined<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.Any(token => EnumOf<T>.Comparer.EqualTo(@this, token.Value));

	[DebuggerHidden]
	public static string Name<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.FirstOrDefault(token => EnumOf<T>.Comparer.EqualTo(@this, token.Value))?.Name ?? @this.ToString();

	[DebuggerHidden]
	public static string Number<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.FirstOrDefault(token => EnumOf<T>.Comparer.EqualTo(@this, token.Value))?.Number ?? @this.ToString("D");

	/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringComparer ToStringComparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);

	public static bool IsCollection(this SystemType @this) => @this switch
	{
		SystemType.Array => true,
		SystemType.ArrayList => true,
		SystemType.BitArray => true,
		SystemType.BlockingCollection => true,
		SystemType.Collection => true,
		SystemType.ConcurrentBag => true,
		SystemType.ConcurrentDictionary => true,
		SystemType.ConcurrentQueue => true,
		SystemType.ConcurrentStack => true,
		SystemType.Dictionary => true,
		SystemType.HashSet => true,
		SystemType.Hashtable => true,
		SystemType.HybridDictionary => true,
		SystemType.ImmutableArray => true,
		SystemType.ImmutableDictionary => true,
		SystemType.ImmutableHashSet => true,
		SystemType.ImmutableList => true,
		SystemType.ImmutableQueue => true,
		SystemType.ImmutableSortedDictionary => true,
		SystemType.ImmutableSortedSet => true,
		SystemType.ImmutableStack => true,
		SystemType.KeyedCollection => true,
		SystemType.LinkedList => true,
		SystemType.List => true,
		SystemType.ListDictionary => true,
		SystemType.NameObjectCollectionBase => true,
		SystemType.NameValueCollection => true,
		SystemType.ObservableCollection => true,
		SystemType.OrderedDictionary => true,
		SystemType.PriorityQueue => true,
		SystemType.Queue => true,
		SystemType.ReadOnlyCollection => true,
		SystemType.ReadOnlyDictionary => true,
		SystemType.ReadOnlyObservableCollection => true,
		SystemType.SortedDictionary => true,
		SystemType.SortedList => true,
		SystemType.SortedSet => true,
		SystemType.Stack => true,
		SystemType.StringCollection => true,
		SystemType.StringDictionary => true,
		_ => false
	};

	public static bool IsConcurrent(this SystemType @this) => @this switch
	{
		SystemType.ConcurrentBag => true,
		SystemType.ConcurrentDictionary => true,
		SystemType.ConcurrentQueue => true,
		SystemType.ConcurrentStack => true,
		_ => false
	};

	public static bool IsDictionary(this SystemType @this) => @this switch
	{
		SystemType.ConcurrentDictionary => true,
		SystemType.Dictionary => true,
		SystemType.Hashtable => true,
		SystemType.HybridDictionary => true,
		SystemType.ImmutableDictionary => true,
		SystemType.ImmutableSortedDictionary => true,
		SystemType.KeyedCollection => true,
		SystemType.ListDictionary => true,
		SystemType.NameObjectCollectionBase => true,
		SystemType.NameValueCollection => true,
		SystemType.OrderedDictionary => true,
		SystemType.ReadOnlyDictionary => true,
		SystemType.SortedDictionary => true,
		SystemType.SortedList => true,
		SystemType.StringDictionary => true,
		_ => false
	};

	public static bool IsImmutable(this SystemType @this) => @this switch
	{
		SystemType.ImmutableArray => true,
		SystemType.ImmutableDictionary => true,
		SystemType.ImmutableHashSet => true,
		SystemType.ImmutableList => true,
		SystemType.ImmutableQueue => true,
		SystemType.ImmutableSortedDictionary => true,
		SystemType.ImmutableSortedSet => true,
		SystemType.ImmutableStack => true,
		_ => false
	};

	public static bool IsPrimitive(this SystemType @this) => @this switch
	{
		SystemType.Boolean => true,
		SystemType.SByte => true,
		SystemType.Int16 => true,
		SystemType.Int32 => true,
		SystemType.Int64 => true,
		SystemType.Int128 => true,
		SystemType.IntPtr => true,
		SystemType.Byte => true,
		SystemType.UInt16 => true,
		SystemType.UInt32 => true,
		SystemType.UInt64 => true,
		SystemType.UInt128 => true,
		SystemType.UIntPtr => true,
		SystemType.Char => true,
		SystemType.Single => true,
		SystemType.Double => true,
		_ => false
	};

	public static bool IsReadOnly(this SystemType @this) => @this switch
	{
		SystemType.ReadOnlyCollection => true,
		SystemType.ReadOnlyDictionary => true,
		SystemType.ReadOnlyObservableCollection => true,
		_ => false
	};

	public static bool IsEnumUnderlyingType(this SystemType @this)
		=> @this switch
		{
			SystemType.SByte => true,
			SystemType.Int16 => true,
			SystemType.Int32 => true,
			SystemType.Int64 => true,
			SystemType.Byte => true,
			SystemType.UInt16 => true,
			SystemType.UInt32 => true,
			SystemType.UInt64 => true,
			_ => false
		};
}
