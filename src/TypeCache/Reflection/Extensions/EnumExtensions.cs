// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypeCache.Reflection.Extensions;

public static class EnumExtensions
{
	public static bool IsCollection(this SystemType @this) => @this switch
	{
		SystemType.Array => true,
		SystemType.ArrayList => true,
		SystemType.BitArray => true,
		SystemType.BlockingCollection => true,
		SystemType.Collection => true,
		SystemType.CollectionBase => true,
		SystemType.ConcurrentBag => true,
		SystemType.ConcurrentDictionary => true,
		SystemType.ConcurrentQueue => true,
		SystemType.ConcurrentStack => true,
		SystemType.Dictionary => true,
		SystemType.DictionaryBase => true,
		SystemType.LinkedList => true,
		SystemType.List => true,
		SystemType.HashSet => true,
		SystemType.Hashtable => true,
		SystemType.HybridDictionary => true,
		SystemType.IAsyncEnumerable => true,
		SystemType.ICollection => true,
		SystemType.IDictionary => true,
		SystemType.IEnumerable => true,
		SystemType.IImmutableDictionary => true,
		SystemType.IImmutableList => true,
		SystemType.IImmutableQueue => true,
		SystemType.IImmutableSet => true,
		SystemType.IImmutableStack => true,
		SystemType.IList => true,
		SystemType.ImmutableArray => true,
		SystemType.ImmutableDictionary => true,
		SystemType.ImmutableHashSet => true,
		SystemType.ImmutableList => true,
		SystemType.ImmutableQueue => true,
		SystemType.ImmutableSortedDictionary => true,
		SystemType.ImmutableSortedSet => true,
		SystemType.ImmutableStack => true,
		SystemType.IOrderedDictionary => true,
		SystemType.IReadOnlyCollection => true,
		SystemType.IReadOnlyDictionary => true,
		SystemType.IReadOnlyList => true,
		SystemType.IReadOnlySet => true,
		SystemType.ISet => true,
		SystemType.KeyedCollection => true,
		SystemType.ListDictionary => true,
		SystemType.NameObjectCollectionBase => true,
		SystemType.NameValueCollection => true,
		SystemType.ObservableCollection => true,
		SystemType.OrderedDictionary => true,
		SystemType.PriorityQueue => true,
		SystemType.Queue => true,
		SystemType.ReadOnlyCollection => true,
		SystemType.ReadOnlyCollectionBase => true,
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
		SystemType.DictionaryBase => true,
		SystemType.Hashtable => true,
		SystemType.HybridDictionary => true,
		SystemType.IDictionary => true,
		SystemType.IImmutableDictionary => true,
		SystemType.ImmutableDictionary => true,
		SystemType.ImmutableSortedDictionary => true,
		SystemType.IOrderedDictionary => true,
		SystemType.IReadOnlyDictionary => true,
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
		SystemType.IImmutableDictionary => true,
		SystemType.IImmutableList => true,
		SystemType.IImmutableQueue => true,
		SystemType.IImmutableSet => true,
		SystemType.IImmutableStack => true,
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

	public static bool IsReadOnly(this SystemType @this) => @this switch
	{
		SystemType.IAsyncEnumerable => true,
		SystemType.IEnumerable => true,
		SystemType.IReadOnlyCollection => true,
		SystemType.IReadOnlyDictionary => true,
		SystemType.IReadOnlyList => true,
		SystemType.IReadOnlySet => true,
		SystemType.ReadOnlyCollection => true,
		SystemType.ReadOnlyCollectionBase => true,
		SystemType.ReadOnlyDictionary => true,
		SystemType.ReadOnlyObservableCollection => true,
		_ => false
	};
}
