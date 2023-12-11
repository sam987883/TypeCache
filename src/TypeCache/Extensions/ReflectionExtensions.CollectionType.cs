// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public enum CollectionType
{
	None = 0,
	/// <summary>
	/// <c><see cref="System.Array"/></c>
	/// </summary>
	Array,
	/// <summary>
	/// <c><see cref="System.Collections.ArrayList"/></c>
	/// </summary>
	ArrayList,
	/// <summary>
	/// <c><see cref="System.Collections.BitArray"/></c>
	/// </summary>
	BitArray,
	/// <summary>
	/// <c><see cref="System.Collections.Concurrent.BlockingCollection{T}"/></c>
	/// </summary>
	BlockingCollection,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.Collection{T}"/></c><br/>
	/// <c><see cref="System.Collections.CollectionBase"/></c><br/>
	/// <c><see cref="ICollection{T}"/></c><br/>
	/// </summary>
	Collection,
	/// <summary>
	/// <c><see cref="System.Collections.Concurrent.ConcurrentBag{T}"/></c>
	/// </summary>
	ConcurrentBag,
	/// <summary>
	/// <c><see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/></c>
	/// </summary>
	ConcurrentDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Concurrent.ConcurrentQueue{T}"/></c>
	/// </summary>
	ConcurrentQueue,
	/// <summary>
	/// <c><see cref="System.Collections.Concurrent.ConcurrentStack{T}"/></c>
	/// </summary>
	ConcurrentStack,
	/// <summary>
	/// <c><see cref="IDictionary{TKey, TValue}"/></c>
	/// </summary>
	Dictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Frozen.FrozenDictionary{TKey, TValue}"/></c>
	/// </summary>
	FrozenDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Frozen.FrozenSet{T}"/></c>
	/// </summary>
	FrozenSet,
	/// <summary>
	/// <c><see cref="System.Collections.Hashtable"/></c>
	/// </summary>
	Hashtable,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.HybridDictionary"/></c>
	/// </summary>
	HybridDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableArray{T}"/></c>
	/// </summary>
	ImmutableArray,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableDictionary{TKey, TValue}"/></c>
	/// </summary>
	ImmutableDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableList{T}"/></c>
	/// </summary>
	ImmutableList,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableQueue{T}"/></c>
	/// </summary>
	ImmutableQueue,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableSet{T}"/></c>
	/// </summary>
	ImmutableSet,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableSortedDictionary{TKey, TValue}"/></c>
	/// </summary>
	ImmutableSortedDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableSortedSet{T}"/></c>
	/// </summary>
	ImmutableSortedSet,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableStack{T}"/></c>
	/// </summary>
	ImmutableStack,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.KeyedCollection{TKey, TItem}"/></c>
	/// </summary>
	KeyedCollection,
	/// <summary>
	/// <c><see cref="LinkedList{T}"/></c>
	/// </summary>
	LinkedList,
	/// <summary>
	/// <c><see cref="IList{T}"/></c>
	/// </summary>
	List,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.ListDictionary"/></c>
	/// </summary>
	ListDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.NameObjectCollectionBase"/></c>
	/// </summary>
	NameObjectCollection,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.NameValueCollection"/></c>
	/// </summary>
	NameValueCollection,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.ObservableCollection{T}"/></c>
	/// </summary>
	ObservableCollection,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.OrderedDictionary"/></c>
	/// </summary>
	OrderedDictionary,
	/// <summary>
	/// <c><see cref="PriorityQueue{TElement, TPriority}"/></c>
	/// </summary>
	PriorityQueue,
	/// <summary>
	/// <c><see cref="System.Collections.Queue"/></c>
	/// </summary>
	Queue,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/></c><br/>
	/// <c><see cref="System.Collections.ReadOnlyCollectionBase"/></c><br/>
	/// <c><see cref="System.Collections.Generic.IReadOnlyCollection{T}"/></c>
	/// </summary>
	ReadOnlyCollection,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/></c>
	/// </summary>
	ReadOnlyDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.IReadOnlyList{T}"/></c>
	/// </summary>
	ReadOnlyList,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{T}"/></c>
	/// </summary>
	ReadOnlyObservableCollection,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.IReadOnlySet{T}"/></c>
	/// </summary>
	ReadOnlySet,
	/// <summary>
	/// <c><see cref="ISet{T}"/></c>
	/// </summary>
	Set,
	/// <summary>
	/// <c><see cref="SortedDictionary{TKey, TValue}"/></c>
	/// </summary>
	SortedDictionary,
	/// <summary>
	/// <c><see cref="SortedList{TKey, TValue}"/></c>
	/// </summary>
	SortedList,
	/// <summary>
	/// <c><see cref="SortedSet{T}"/></c>
	/// </summary>
	SortedSet,
	/// <summary>
	/// <c><see cref="System.Collections.Stack"/></c>
	/// </summary>
	Stack,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.StringCollection"/></c>
	/// </summary>
	StringCollection,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.StringDictionary"/></c>
	/// </summary>
	StringDictionary
}
