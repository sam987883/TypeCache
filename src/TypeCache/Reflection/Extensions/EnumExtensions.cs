// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TypeCache.Reflection.Extensions;

public static class EnumExtensions
{
	/// <summary>
	/// <b>Collections:</b><br/>
	/// <c>
	/// <see cref="Array"/>,
	/// <see cref="ArrayList"/>,
	/// <see cref="BitArray"/>,
	/// <see cref="BlockingCollection{T}"/>,
	/// <see cref="Collection{T}"/>,
	/// <see cref="CollectionBase"/>,
	/// <see cref="ConcurrentBag{T}"/>,
	/// <see cref="ConcurrentDictionary{TKey, TValue}"/>,
	/// <see cref="ConcurrentQueue{T}"/>,
	/// <see cref="ConcurrentStack{T}"/>,
	/// <see cref="Dictionary{TKey, TValue}"/>,
	/// <see cref="DictionaryBase"/>,
	/// <see cref="HashSet{T}"/>,
	/// <see cref="HybridDictionary"/>,
	/// <see cref="IAsyncEnumerable{T}"/>,
	/// <see cref="ICollection"/>, <see cref="ICollection{T}"/>,
	/// <see cref="IDictionary"/>, <see cref="IDictionary{TKey, TValue}"/>,
	/// <see cref="IEnumerable"/>, <see cref="IEnumerable{T}"/>,
	/// <see cref="IImmutableDictionary{TKey, TValue}"/>,
	/// <see cref="IImmutableList{T}"/>,
	/// <see cref="IImmutableQueue{T}"/>,
	/// <see cref="IImmutableSet{T}"/>,
	/// <see cref="IImmutableStack{T}"/>,
	/// <see cref="IList"/>, <see cref="IList{T}"/>,
	/// <see cref="ImmutableArray{T}"/>,
	/// <see cref="ImmutableDictionary{TKey, TValue}"/>,
	/// <see cref="ImmutableHashSet{T}"/>,
	/// <see cref="ImmutableList{T}"/>,
	/// <see cref="ImmutableQueue{T}"/>,
	/// <see cref="ImmutableSortedDictionary{TKey, TValue}"/>,
	/// <see cref="ImmutableSortedSet{T}"/>,
	/// <see cref="ImmutableStack{T}"/>,
	/// <see cref="IOrderedDictionary"/>,
	/// <see cref="IReadOnlyCollection{T}"/>,
	/// <see cref="IReadOnlyDictionary{TKey, TValue}"/>,
	/// <see cref="IReadOnlyList{T}"/>,
	/// <see cref="IReadOnlySet{T}"/>,
	/// <see cref="ISet{T}"/>,
	/// <see cref="KeyedCollection{TKey, TItem}"/>,
	/// <see cref="LinkedList{T}"/>,
	/// <see cref="List{T}"/>,
	/// <see cref="ListDictionary"/>,
	/// <see cref="NameObjectCollectionBase"/>,
	/// <see cref="NameValueCollection"/>,
	/// <see cref="ObservableCollection{T}"/>,
	/// <see cref="OrderedDictionary"/>,
	/// <see cref="PriorityQueue{TElement, TPriority}"/>,
	/// <see cref="Queue"/>, <see cref="Queue{T}"/>,
	/// <see cref="ReadOnlyCollection{T}"/>,
	/// <see cref="ReadOnlyCollectionBase"/>,
	/// <see cref="ReadOnlyDictionary{TKey, TValue}"/>,
	/// <see cref="ReadOnlyObservableCollection{T}"/>,
	/// <see cref="SortedDictionary{TKey, TValue}"/>,
	/// <see cref="SortedList"/>, <see cref="SortedList{TKey, TValue}"/>,
	/// <see cref="SortedSet{T}"/>,
	/// <see cref="Stack"/>, <see cref="Stack{T}"/>,
	/// <see cref="StringCollection"/>,
	/// <see cref="StringDictionary"/>,
	/// </c>
	/// </summary>
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

	/// <summary>
	/// <b>Concurrent collections:</b><br/>
	/// <c>
	/// <see cref="ConcurrentBag{T}"/>,
	/// <see cref="ConcurrentDictionary{TKey, TValue}"/>,
	/// <see cref="ConcurrentQueue{T}"/>,
	/// <see cref="ConcurrentStack{T}"/>,
	/// </c>
	/// </summary>
	public static bool IsConcurrent(this SystemType @this) => @this switch
	{
		SystemType.ConcurrentBag => true,
		SystemType.ConcurrentDictionary => true,
		SystemType.ConcurrentQueue => true,
		SystemType.ConcurrentStack => true,
		_ => false
	};

	/// <summary>
	/// <b>Dictionary collections:</b><br/>
	/// <c>
	/// <see cref="ConcurrentDictionary{TKey, TValue}"/>,
	/// <see cref="Dictionary{TKey, TValue}"/>,
	/// <see cref="DictionaryBase"/>,
	/// <see cref="Hashtable"/>,
	/// <see cref="HybridDictionary"/>,
	/// <see cref="IDictionary"/>, <see cref="IDictionary{TKey, TValue}"/>,
	/// <see cref="IImmutableDictionary{TKey, TValue}"/>,
	/// <see cref="ImmutableDictionary{TKey, TValue}"/>,
	/// <see cref="ImmutableSortedDictionary{TKey, TValue}"/>,
	/// <see cref="IOrderedDictionary"/>,
	/// <see cref="IReadOnlyDictionary{TKey, TValue}"/>,
	/// <see cref="KeyedCollection{TKey, TItem}"/>,
	/// <see cref="ListDictionary"/>,
	/// <see cref="NameObjectCollectionBase"/>,
	/// <see cref="NameValueCollection"/>,
	/// <see cref="OrderedDictionary"/>,
	/// <see cref="ReadOnlyDictionary{TKey, TValue}"/>,
	/// <see cref="SortedDictionary{TKey, TValue}"/>,
	/// <see cref="SortedList"/>, <see cref="SortedList{TKey, TValue}"/>,
	/// <see cref="StringDictionary"/>
	/// </c>
	/// </summary>
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

	/// <summary>
	/// <b>Immutable collections:</b><br/>
	/// <c>
	/// <see cref="IImmutableDictionary{TKey, TValue}"/>,
	/// <see cref="IImmutableList{T}"/>,
	/// <see cref="IImmutableQueue{T}"/>,
	/// <see cref="IImmutableSet{T}"/>,
	/// <see cref="IImmutableStack{T}"/>,
	/// <see cref="ImmutableArray{T}"/>,
	/// <see cref="ImmutableDictionary{TKey, TValue}"/>,
	/// <see cref="ImmutableHashSet{T}"/>,
	/// <see cref="ImmutableList{T}"/>,
	/// <see cref="ImmutableQueue{T}"/>,
	/// <see cref="ImmutableSortedDictionary{TKey, TValue}"/>,
	/// <see cref="ImmutableSortedSet{T}"/>,
	/// <see cref="ImmutableStack{T}"/>,
	/// </c>
	/// </summary>
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

	/// <summary>
	/// <b>Primitive types:</b><br/>
	/// <c>
	/// <see cref="bool"/>,
	/// <see cref="IntPtr"/>,
	/// <see cref="sbyte"/>,
	/// <see cref="short"/>,
	/// <see cref="int"/>,
	/// <see cref="long"/>,
	/// <see cref="UIntPtr"/>,
	/// <see cref="byte"/>,
	/// <see cref="ushort"/>,
	/// <see cref="uint"/>,
	/// <see cref="ulong"/>,
	/// <see cref="char"/>,
	/// <see cref="float"/>,
	/// <see cref="double"/>
	/// </c>
	/// </summary>
	public static bool IsPrimitive(this SystemType @this) => @this switch
	{
		SystemType.Boolean => true,
		SystemType.Byte => true,
		SystemType.SByte => true,
		SystemType.Int16 => true,
		SystemType.UInt16 => true,
		SystemType.Int32 => true,
		SystemType.UInt32 => true,
		SystemType.Int64 => true,
		SystemType.UInt64 => true,
		SystemType.IntPtr => true,
		SystemType.UIntPtr => true,
		SystemType.Char => true,
		SystemType.Single => true,
		SystemType.Double => true,
		_ => false
	};

	/// <summary>
	/// <b>Read-only collections:</b><br/>
	/// <c>
	/// <see cref="IAsyncEnumerable{T}"/>,
	/// <see cref="IEnumerable"/>, <see cref="IEnumerable{T}"/>,
	/// <see cref="IReadOnlyCollection{T}"/>,
	/// <see cref="IReadOnlyDictionary{TKey, TValue}"/>,
	/// <see cref="IReadOnlyList{T}"/>,
	/// <see cref="IReadOnlySet{T}"/>,
	/// <see cref="ReadOnlyCollection{T}"/>
	/// <see cref="ReadOnlyCollectionBase"/>,
	/// <see cref="ReadOnlyDictionary{TKey, TValue}"/>,
	/// <see cref="ReadOnlyObservableCollection{T}"/>
	/// </c>
	/// </summary>
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
