// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public enum ObjectType
{
	Unknown = 0,
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
	/// Implements: <c><see cref="System.Data.DataColumn"/></c>
	/// </summary>
	DataColumn,
	/// <summary>
	/// Implements: <c><see cref="System.Data.DataRow"/></c>
	/// </summary>
	DataRow,
	/// <summary>
	/// Implements: <c><see cref="System.Data.DataRowView"/></c>
	/// </summary>
	DataRowView,
	/// <summary>
	/// Implements: <c><see cref="System.Data.DataSet"/></c>
	/// </summary>
	DataSet,
	/// <summary>
	/// Implements: <c><see cref="System.Data.DataTable"/></c>
	/// </summary>
	DataTable,
	/// <summary>
	/// Is any type part of <see cref="DataType"/>.
	/// </summary>
	DataType,
	/// <summary>
	/// Implements: <c><see cref="System.Data.DataView"/></c>
	/// </summary>
	DataView,
	/// <summary>
	/// Implements: <c><see cref="System.Delegate"/></c>
	/// </summary>
	Delegate,
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
	/// is: <c><see cref="System.Text.Json.JsonDocument"/></c>
	/// </summary>
	JsonDocument,
	/// <summary>
	/// Implements: <c><see cref="Lazy{T}"/></c> or <c><see cref="Lazy{T, TMetadata}"/></c>
	/// </summary>
	Lazy,
	/// <summary>
	/// Is: <c><see cref="Memory{T}"/></c>
	/// </summary>
	Memory,
	/// <summary>
	/// Implements: <c><see cref="object"/></c>
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
	/// Is: <c><see cref="IntPtr"/></c> or <c><see cref="UIntPtr"/></c>
	/// </summary>
	Pointer,
	/// <summary>
	/// Implements: <c><see cref="System.Range"/></c>
	/// </summary>
	Range,
	/// <summary>
	/// Is: <c><see cref="ReadOnlyMemory{T}"/></c>
	/// </summary>
	ReadOnlyMemory,
	/// <summary>
	/// Is: <c><see cref="ReadOnlySpan{T}"/></c>
	/// </summary>
	ReadOnlySpan,
	/// <summary>
	/// Is: <c><see cref="Span{T}"/></c>
	/// </summary>
	Span,
	/// <summary>
	/// Implements: <c><see cref="System.IO.Stream"/></c>
	/// </summary>
	Stream,
	/// <summary>
	/// Is: <c><see cref="System.Text.StringBuilder"/></c>
	/// </summary>
	StringBuilder,
	/// <summary>
	/// Is: <c><see cref="System.Threading.Tasks.Task"/></c> or <c><see cref="Task{TResult}"/></c>
	/// </summary>
	Task,
	/// <summary>
	/// Implements: <c><see cref="System.Type"/></c>
	/// </summary>
	Type,
	/// <summary>
	/// <c><see langword="class"/></c> type that implements: <c><see cref="ITuple"/></c>
	/// </summary>
	Tuple,
	/// <summary>
	/// Is: <c><see cref="System.Threading.Tasks.ValueTask"/></c> or <c><see cref="ValueTask{TResult}"/></c>
	/// </summary>
	ValueTask,
	/// <summary>
	/// <c><see langword="struct"/></c> type that implements: <c><see cref="ITuple"/></c>
	/// </summary>
	ValueTuple,
	/// <summary>
	/// Is: <c><see cref="void"/></c>
	/// </summary>
	Void,
	/// <summary>
	/// Is: <c><see cref="WeakReference{T}"/></c> or <c><see cref="System.WeakReference"/></c>
	/// </summary>
	WeakReference
}
