// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class EnumExtensions
{
	[DebuggerHidden]
	public static IReadOnlyCollection<Attribute> Attributes<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Attributes : Array<Attribute>.Empty;

	[DebuggerHidden]
	public static bool HasAnyFlag<T>(this T @this, params T[] flags)
		where T : struct, Enum
		=> flags?.Any(flag => @this.HasFlag(flag)) ?? false;

	[DebuggerHidden]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Hex : @this.ToString("X");

	[DebuggerHidden]
	public static bool IsDefined<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.ContainsKey(@this);

	[DebuggerHidden]
	public static string Name<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Name : @this.ToString();

	[DebuggerHidden]
	public static string Number<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Number : @this.ToString("D");

	/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringComparer ToStringComparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);

	public static bool IsCollection(this SystemType @this) => @this switch
	{
		SystemType.Array
		or SystemType.BitArray
		or SystemType.BlockingCollection
		or SystemType.Collection
		or SystemType.ConcurrentBag or SystemType.ConcurrentDictionary or SystemType.ConcurrentQueue or SystemType.ConcurrentStack
		or SystemType.Dictionary
		or SystemType.HashSet
		or SystemType.Hashtable
		or SystemType.HybridDictionary
		or SystemType.ImmutableArray or SystemType.ImmutableDictionary or SystemType.ImmutableHashSet or SystemType.ImmutableList
		or SystemType.ImmutableQueue or SystemType.ImmutableSortedDictionary or SystemType.ImmutableSortedSet or SystemType.ImmutableStack
		or SystemType.KeyedCollection
		or SystemType.List or SystemType.LinkedList or SystemType.ArrayList
		or SystemType.ListDictionary
		or SystemType.NameObjectCollectionBase or SystemType.NameValueCollection
		or SystemType.ObservableCollection
		or SystemType.OrderedDictionary
		or SystemType.PriorityQueue
		or SystemType.Queue
		or SystemType.ReadOnlyCollection or SystemType.ReadOnlyDictionary or SystemType.ReadOnlyObservableCollection
		or SystemType.SortedDictionary or SystemType.SortedList or SystemType.SortedSet
		or SystemType.Stack
		or SystemType.StringCollection
		or SystemType.StringDictionary => true,
		_ => false
	};

	public static bool IsConcurrent(this SystemType @this) => @this switch
	{
		SystemType.ConcurrentBag or SystemType.ConcurrentDictionary
		or SystemType.ConcurrentQueue or SystemType.ConcurrentStack => true,
		_ => false
	};

	public static bool IsDictionary(this SystemType @this) => @this switch
	{
		SystemType.Dictionary or SystemType.ConcurrentDictionary or SystemType.SortedDictionary
		or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary
		or SystemType.Hashtable or SystemType.HybridDictionary or SystemType.OrderedDictionary or SystemType.ReadOnlyDictionary
		or SystemType.KeyedCollection or SystemType.ListDictionary or SystemType.StringDictionary
		or SystemType.NameObjectCollectionBase or SystemType.NameValueCollection or SystemType.SortedList => true,
		_ => false
	};

	public static bool IsEnumUnderlyingType(this SystemType @this) => @this switch
	{
		SystemType.SByte or SystemType.Byte
		or SystemType.Int16 or SystemType.Int32 or SystemType.Int64
		or SystemType.UInt16 or SystemType.UInt32 or SystemType.UInt64 => true,
		_ => false
	};

	public static bool IsImmutable(this SystemType @this) => @this switch
	{
		SystemType.ImmutableArray
		or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary
		or SystemType.ImmutableHashSet or SystemType.ImmutableSortedSet
		or SystemType.ImmutableList
		or SystemType.ImmutableQueue
		or SystemType.ImmutableStack => true,
		_ => false
	};

	public static bool IsPrimitive(this SystemType @this) => @this switch
	{
		SystemType.Boolean
		or SystemType.SByte or SystemType.Byte
		or SystemType.Int16 or SystemType.Int32 or SystemType.Int64 or SystemType.Int128
		or SystemType.IntPtr or SystemType.UIntPtr
		or SystemType.UInt16 or SystemType.UInt32 or SystemType.UInt64 or SystemType.UInt128
		or SystemType.Single or SystemType.Double
		or SystemType.Char => true,
		_ => false
	};

	public static bool IsQueue(this SystemType @this) => @this switch
	{
		SystemType.ConcurrentQueue or SystemType.ImmutableQueue or SystemType.PriorityQueue or SystemType.Queue => true,
		_ => false
	};

	public static bool IsReadOnly(this SystemType @this) => @this switch
	{
		SystemType.ReadOnlyCollection or SystemType.ReadOnlyDictionary or SystemType.ReadOnlyObservableCollection => true,
		_ => false
	};

	public static bool IsStack(this SystemType @this) => @this switch
	{
		SystemType.ConcurrentStack or SystemType.ImmutableStack or SystemType.Stack => true,
		_ => false
	};
}
