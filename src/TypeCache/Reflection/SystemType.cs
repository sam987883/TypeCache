// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection;

public enum SystemType
{
	Unknown = 0,
	/// <summary>
	/// <c><see cref="System.Object"/></c>
	/// </summary>
	Object,
	/// <summary>
	/// <c><see cref="System.Boolean"/></c>
	/// </summary>
	Boolean,
	/// <summary>
	/// <c><see cref="System.SByte"/></c>
	/// </summary>
	SByte,
	/// <summary>
	/// <c><see cref="System.Byte"/></c>
	/// </summary>
	Byte,
	/// <summary>
	/// <c><see cref="System.Int16"/></c>
	/// </summary>
	Int16,
	/// <summary>
	/// <c><see cref="System.UInt16"/></c>
	/// </summary>
	UInt16,
	/// <summary>
	/// <c><see cref="System.Int32"/></c>
	/// </summary>
	Int32,
	/// <summary>
	/// <c><see cref="System.UInt32"/></c>
	/// </summary>
	UInt32,
	/// <summary>
	/// <c><see cref="System.Int64"/></c>
	/// </summary>
	Int64,
	/// <summary>
	/// <c><see cref="System.UInt64"/></c>
	/// </summary>
	UInt64,
	/// <summary>
	/// <c><see cref="System.Numerics.BigInteger"/></c>
	/// </summary>
	BigInteger,
	/// <summary>
	/// <c><see cref="System.IntPtr"/></c>
	/// </summary>
	IntPtr,
	/// <summary>
	/// <c><see cref="System.UIntPtr"/></c>
	/// </summary>
	UIntPtr,
	/// <summary>
	/// <c><see cref="System.Single"/></c>
	/// </summary>
	Single,
	/// <summary>
	/// <c><see cref="System.Double"/></c>
	/// </summary>
	Double,
	/// <summary>
	/// <c><see cref="System.Half"/></c>
	/// </summary>
	Half,
	/// <summary>
	/// <c><see cref="System.Decimal"/></c>
	/// </summary>
	Decimal,
	/// <summary>
	/// <c><see cref="System.DateOnly"/></c>
	/// </summary>
	DateOnly,
	/// <summary>
	/// <c><see cref="System.DateTime"/></c>
	/// </summary>
	DateTime,
	/// <summary>
	/// <c><see cref="System.DateTimeOffset"/></c>
	/// </summary>
	DateTimeOffset,
	/// <summary>
	/// <c><see cref="System.TimeOnly"/></c>
	/// </summary>
	TimeOnly,
	/// <summary>
	/// <c><see cref="System.TimeSpan"/></c>
	/// </summary>
	TimeSpan,
	/// <summary>
	/// <c><see cref="System.Guid"/></c>
	/// </summary>
	Guid,
	/// <summary>
	/// <c><see cref="System.Char"/></c>
	/// </summary>
	Char,
	/// <summary>
	/// <c><see cref="System.Index"/></c>
	/// </summary>
	Index,
	/// <summary>
	/// <c><see cref="System.Range"/></c>
	/// </summary>
	Range,
	/// <summary>
	/// <c><see cref="System.Text.Json.JsonElement"/></c>
	/// </summary>
	JsonElement,
	/// <summary>
	/// <c><see cref="System.String"/></c>
	/// </summary>
	String,
	/// <summary>
	/// <c><see cref="System.Uri"/></c>
	/// </summary>
	Uri,
	/// <summary>
	/// <c><see cref="System.DBNull"/></c>
	/// </summary>
	DBNull,
	/// <summary>
	/// <c><see cref="System.Void"/></c>
	/// </summary>
	Void,
	/// <summary>
	/// <c><see cref="System.Span{T}"/></c>
	/// </summary>
	Span,
	/// <summary>
	/// <c><see cref="System.Memory{T}"/></c>
	/// </summary>
	Memory,
	/// <summary>
	/// <c><see cref="System.ReadOnlySpan{T}"/></c>
	/// </summary>
	ReadOnlySpan,
	/// <summary>
	/// <c><see cref="System.ReadOnlyMemory{T}"/></c>
	/// </summary>
	ReadOnlyMemory,
	/// <summary>
	/// <code>
	/// <see cref="System.Lazy{T}"/><br/>
	/// <see cref="System.Lazy{T, TMetadata}"/>
	/// </code>
	/// </summary>
	Lazy,
	/// <summary>
	/// <c><see cref="System.Nullable{T}"/></c>
	/// </summary>
	Nullable,
	/// <summary>
	/// <code>
	/// <see cref="System.Threading.Tasks.Task"/><br/>
	/// <see cref="System.Threading.Tasks.Task{TResult}"/>
	/// </code>
	/// </summary>
	Task,
	/// <summary>
	/// <code>
	/// <see cref="System.Threading.Tasks.ValueTask"/><br/>
	/// <see cref="System.Threading.Tasks.ValueTask{TResult}"/>
	/// </code>
	/// </summary>
	ValueTask,
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
	/// <c><see cref="System.Collections.ObjectModel.Collection{T}"/></c>
	/// </summary>
	Collection,
	/// <summary>
	/// <c><see cref="System.Collections.CollectionBase"/></c>
	/// </summary>
	CollectionBase,
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
	/// <c><see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/></c>
	/// </summary>
	Dictionary,
	/// <summary>
	/// <c><see cref="System.Collections.DictionaryBase"/></c>
	/// </summary>
	DictionaryBase,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.HashSet{T}"/></c>
	/// </summary>
	HashSet,
	/// <summary>
	/// <c><see cref="System.Collections.Hashtable"/></c>
	/// </summary>
	Hashtable,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.HybridDictionary"/></c>
	/// </summary>
	HybridDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.IAsyncEnumerable{T}"/></c>
	/// </summary>
	IAsyncEnumerable,
	/// <summary>
	/// <code>
	/// <see cref="System.Collections.ICollection"/><br/>
	/// <see cref="System.Collections.Generic.ICollection{T}"/>
	/// </code>
	/// </summary>
	ICollection,
	/// <summary>
	/// <code>
	/// <see cref="System.Collections.IDictionary"/><br/>
	/// <see cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
	/// </code>
	/// </summary>
	IDictionary,
	/// <summary>
	/// <code>
	/// <see cref="System.Collections.IEnumerable"/><br/>
	/// <see cref="System.Collections.Generic.IEnumerable{T}"/>
	/// </code>
	/// </summary>
	IEnumerable,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableDictionary{TKey, TValue}"/></c>
	/// </summary>
	IImmutableDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableList{T}"/></c>
	/// </summary>
	IImmutableList,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableQueue{T}"/></c>
	/// </summary>
	IImmutableQueue,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableSet{T}"/></c>
	/// </summary>
	IImmutableSet,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.IImmutableStack{T}"/></c>
	/// </summary>
	IImmutableStack,
	/// <summary>
	/// <code>
	/// <see cref="System.Collections.IList"/><br/>
	/// <see cref="System.Collections.Generic.IList{T}"/>
	/// </code>
	/// </summary>
	IList,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableArray{T}"/></c>
	/// </summary>
	ImmutableArray,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableDictionary{TKey, TValue}"/></c>
	/// </summary>
	ImmutableDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableHashSet{T}"/></c>
	/// </summary>
	ImmutableHashSet,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableList{T}"/></c>
	/// </summary>
	ImmutableList,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableQueue{T}"/></c>
	/// </summary>
	ImmutableQueue,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableSortedDictionary{TKey, TValue}"/></c>
	/// </summary>
	ImmutableSortedDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableSortedSet{T}"/></c>
	/// </summary>
	ImmutableSortedSet,
	/// <summary>
	/// <c><see cref="System.Collections.Immutable.ImmutableStack{T}"/></c>
	/// </summary>
	ImmutableStack,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.IOrderedDictionary"/></c>
	/// </summary>
	IOrderedDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.IReadOnlyCollection{T}"/></c>
	/// </summary>
	IReadOnlyCollection,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.IReadOnlyDictionary{TKey, TValue}"/></c>
	/// </summary>
	IReadOnlyDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.IReadOnlyList{T}"/></c>
	/// </summary>
	IReadOnlyList,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.IReadOnlySet{T}"/></c>
	/// </summary>
	IReadOnlySet,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.ISet{T}"/></c>
	/// </summary>
	ISet,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.KeyedCollection{TKey, TItem}"/></c>
	/// </summary>
	KeyedCollection,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.LinkedList{T}"/></c>
	/// </summary>
	LinkedList,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.List{T}"/></c>
	/// </summary>
	List,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.ListDictionary"/></c>
	/// </summary>
	ListDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Specialized.NameObjectCollectionBase"/></c>
	/// </summary>
	NameObjectCollectionBase,
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
	/// <c><see cref="System.Collections.Generic.PriorityQueue{TElement, TPriority}"/></c>
	/// </summary>
	PriorityQueue,
	/// <summary>
	/// <c><see cref="System.Collections.Queue"/></c>
	/// </summary>
	Queue,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/></c>
	/// </summary>
	ReadOnlyCollection,
	/// <summary>
	/// <c><see cref="System.Collections.ReadOnlyCollectionBase"/></c>
	/// </summary>
	ReadOnlyCollectionBase,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/></c>
	/// </summary>
	ReadOnlyDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{T}"/></c>
	/// </summary>
	ReadOnlyObservableCollection,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.SortedDictionary{TKey, TValue}"/></c>
	/// </summary>
	SortedDictionary,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.SortedList{TKey, TValue}"/></c>
	/// </summary>
	SortedList,
	/// <summary>
	/// <c><see cref="System.Collections.Generic.SortedSet{T}"/></c>
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
	StringDictionary,
	/// <summary>
	/// <code>
	/// <see cref="System.Tuple{T1}"/><br/>
	/// <see cref="System.Tuple{T1, T2}"/><br/>
	/// <see cref="System.Tuple{T1, T2, T3}"/><br/>
	/// <see cref="System.Tuple{T1, T2, T3, T4}"/><br/>
	/// <see cref="System.Tuple{T1, T2, T3, T4, T5}"/><br/>
	/// <see cref="System.Tuple{T1, T2, T3, T4, T5, T6}"/><br/>
	/// <see cref="System.Tuple{T1, T2, T3, T4, T5, T6, T7}"/><br/>
	/// <see cref="System.Tuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/>
	/// </code>
	/// </summary>
	Tuple,
	/// <summary>
	/// <code>
	/// <see cref="System.ValueTuple"/><br/>
	/// <see cref="System.ValueTuple{T1}"/><br/>
	/// <see cref="System.ValueTuple{T1, T2}"/><br/>
	/// <see cref="System.ValueTuple{T1, T2, T3}"/><br/>
	/// <see cref="System.ValueTuple{T1, T2, T3, T4}"/><br/>
	/// <see cref="System.ValueTuple{T1, T2, T3, T4, T5}"/><br/>
	/// <see cref="System.ValueTuple{T1, T2, T3, T4, T5, T6}"/><br/>
	/// <see cref="System.ValueTuple{T1, T2, T3, T4, T5, T6, T7}"/><br/>
	/// <see cref="System.ValueTuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/>
	/// </code>
	/// </summary>
	ValueTuple,
	/// <summary>
	/// <code>
	/// <see cref="System.Action"/><br/>
	/// <see cref="System.Action{T}"/><br/>
	/// <see cref="System.Action{T1, T2}"/><br/>
	/// <see cref="System.Action{T1, T2, T3}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/><br/>
	/// <see cref="System.Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>
	/// </code>
	/// </summary>
	Action,
	/// <summary>
	/// <code>
	/// <see cref="System.Func{TResult}"/><br/>
	/// <see cref="System.Func{T, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}"/><br/>
	/// <see cref="System.Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}"/>
	/// </code>
	/// </summary>
	Func,
	/// <summary>
	/// <c><see cref="System.Type"/></c>
	/// </summary>
	Type,
	/// <summary>
	/// <code>
	/// <see cref="System.WeakReference"/><br/>
	/// <see cref="System.WeakReference{T}"/>
	/// </code>
	/// </summary>
	WeakReference
}
