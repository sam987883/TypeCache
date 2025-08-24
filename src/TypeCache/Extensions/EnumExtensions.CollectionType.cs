// Copyright (c) 2021 Samuel Abraham

using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static partial class EnumExtensions
{
	public static bool IsConcurrent(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ConcurrentBag
			or CollectionType.ConcurrentDictionary
			or CollectionType.ConcurrentQueue
			or CollectionType.ConcurrentStack => true,
			_ => false
		};

	public static bool IsDictionary(this CollectionType @this)
		=> @this switch
		{
			CollectionType.Dictionary
			or CollectionType.ConcurrentDictionary
			or CollectionType.FrozenDictionary
			or CollectionType.SortedDictionary
			or CollectionType.ImmutableDictionary
			or CollectionType.ImmutableSortedDictionary
			or CollectionType.Hashtable
			or CollectionType.HybridDictionary
			or CollectionType.OrderedDictionary
			or CollectionType.ReadOnlyDictionary
			or CollectionType.KeyedCollection
			or CollectionType.ListDictionary
			or CollectionType.StringDictionary
			or CollectionType.NameObjectCollection
			or CollectionType.NameValueCollection
			or CollectionType.SortedList => true,
			_ => false
		};

	public static bool IsFrozen(this CollectionType @this)
		=> @this switch
		{
			CollectionType.FrozenDictionary or CollectionType.FrozenSet => true,
			_ => false
		};

	public static bool IsImmutable(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ImmutableArray
			or CollectionType.ImmutableDictionary or CollectionType.ImmutableSortedDictionary
			or CollectionType.ImmutableSet or CollectionType.ImmutableSortedSet
			or CollectionType.ImmutableList
			or CollectionType.ImmutableQueue
			or CollectionType.ImmutableStack => true,
			_ => false
		};

	public static bool IsQueue(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ConcurrentQueue
			or CollectionType.ImmutableQueue
			or CollectionType.PriorityQueue
			or CollectionType.Queue => true,
			_ => false
		};

	public static bool IsReadOnly(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ReadOnlyCollection
			or CollectionType.ReadOnlyDictionary
			or CollectionType.ReadOnlyObservableCollection
			or CollectionType.ReadOnlyList
			or CollectionType.ReadOnlyObservableCollection
			or CollectionType.ReadOnlySet => true,
			_ => false
		};

	public static bool IsSet(this CollectionType @this)
		=> @this switch
		{
			CollectionType.Set
			or CollectionType.ReadOnlySet
			or CollectionType.SortedSet
			or CollectionType.FrozenSet
			or CollectionType.ImmutableSet
			or CollectionType.ImmutableSortedSet => true,
			_ => false
		};

	public static bool IsStack(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ConcurrentStack
			or CollectionType.ImmutableStack
			or CollectionType.Stack => true,
			_ => false
		};
}
