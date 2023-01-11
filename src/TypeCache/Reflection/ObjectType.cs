// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Text;

namespace TypeCache.Reflection;

public enum ObjectType
{
	Unknown = 0,
	/// <summary>
	/// <c><see cref="Type.IsArray"/> <see langword="is true"/></c>
	/// </summary>
	Array,
	/// <summary>
	/// Implements: <c><see cref="IAsyncEnumerable{T}"/></c>
	/// </summary>
	AsyncEnumerable,
	/// <summary>
	/// Implements: <c><see cref="IAsyncEnumerator{T}"/></c>
	/// </summary>
	AsyncEnumerator,
	/// <summary>
	/// Implements: <c><see cref="IAsyncResult"/></c>
	/// </summary>
	AsyncResult,
	/// <summary>
	/// Implements: <c><see cref="System.Attribute"/></c>
	/// </summary>
	Attribute,
	/// <summary>
	/// Implements: <c><see cref="ICollection{T}"/></c>
	/// </summary>
	Collection,
	/// <summary>
	/// Implements: <c><see cref="System.Delegate"/></c>
	/// </summary>
	Delegate,
	/// <summary>
	/// Implements: <c><see cref="IDictionary{TKey, TValue}"/></c>
	/// </summary>
	Dictionary,
	/// <summary>
	/// <c><see cref="Type.IsEnum"/> <see langword="is true"/></c>
	/// </summary>
	Enum,
	/// <summary>
	/// Implements: <c><see cref="IEnumerable{T}"/></c>
	/// </summary>
	Enumerable,
	/// <summary>
	/// Implements: <c><see cref="IEnumerator{T}"/></c>
	/// </summary>
	Enumerator,
	/// <summary>
	/// Implements: <c><see cref="System.Exception"/></c>
	/// </summary>
	Exception,
	/// <summary>
	/// Is: <c><see cref="ImmutableArray{T}"/></c>
	/// </summary>
	ImmutableArray,
	/// <summary>
	/// Implements: <c><see cref="IImmutableDictionary{TKey, TValue}"/></c>
	/// </summary>
	ImmutableDictionary,
	/// <summary>
	/// Implements: <c><see cref="IImmutableList{T}"/></c>
	/// </summary>
	ImmutableList,
	/// <summary>
	/// Implements: <c><see cref="IImmutableQueue{T}"/></c>
	/// </summary>
	ImmutableQueue,
	/// <summary>
	/// Implements: <c><see cref="IImmutableSet{T}"/></c>
	/// </summary>
	ImmutableSet,
	/// <summary>
	/// Implements: <c><see cref="IImmutableStack{T}"/></c>
	/// </summary>
	ImmutableStack,
	/// <summary>
	/// Implements: <c><see cref="System.Text.Json.Nodes.JsonNode"/></c>
	/// </summary>
	JsonNode,
	/// <summary>
	/// Implements: <c><see cref="IList{T}"/></c>
	/// </summary>
	List,
	/// <summary>
	/// Is: <c><see cref="object"/></c>
	/// </summary>
	Object,
	/// <summary>
	/// Implements: <c><see cref="IObservable{T}"/></c>
	/// </summary>
	Observable,
	/// <summary>
	/// Implements: <c><see cref="IObserver{T}"/></c>
	/// </summary>
	Observer,
	/// <summary>
	/// Implements: <c><see cref="System.Collections.Specialized.IOrderedDictionary"/></c>
	/// </summary>
	OrderedDictionary,
	/// <summary>
	/// Is any: <c></c>
	/// </summary>
	Primitive,
	/// <summary>
	/// Implements: <c><see cref="IReadOnlyCollection{T}"/></c>
	/// </summary>
	ReadOnlyCollection,
	/// <summary>
	/// Implements: <c><see cref="IReadOnlyDictionary{TKey, TValue}"/></c>
	/// </summary>
	ReadOnlyDictionary,
	/// <summary>
	/// Implements: <c><see cref="IReadOnlyList{T}"/></c>
	/// </summary>
	ReadOnlyList,
	/// <summary>
	/// Implements: <c><see cref="IReadOnlySet{T}"/></c>
	/// </summary>
	ReadOnlySet,
	/// <summary>
	/// Implements: <c><see cref="ISet{T}"/></c>
	/// </summary>
	Set,
	/// <summary>
	/// Implements: <c><see cref="System.IO.Stream"/></c>
	/// </summary>
	Stream,
	/// <summary>
	/// Is: <c><see cref="string"/></c>
	/// </summary>
	String,
	/// <summary>
	/// Is: <c><see cref="System.Text.StringBuilder"/></c>
	/// </summary>
	StringBuilder
}
