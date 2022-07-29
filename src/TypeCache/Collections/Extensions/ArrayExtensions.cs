﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class ArrayExtensions
{
	/// <inheritdoc cref="Task.WhenAll(Task[])"/>
	/// <remarks>
	/// <c>=&gt; <see langword="await"/> <see cref="Task"/>.WhenAll(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async Task AllAsync<T>(this Task[] @this)
		=> await Task.WhenAll(@this);

	/// <inheritdoc cref="Task.WhenAll{TResult}(Task{TResult}[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any()
	/// ? <see langword="await"/> <see cref="Task"/>.WhenAll(@<paramref name="this"/>)
	/// : <see langword="await"/> <see cref="Task"/>.FromResult(<see cref="Array{T}.Empty"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async Task<T[]> AllAsync<T>(this Task<T>[]? @this)
		=> @this.Any() ? await Task.WhenAll(@this) : await Task.FromResult(Array<T>.Empty);

	/// <inheritdoc cref="Task.WhenAny(Task[])"/>
	/// <remarks>
	/// <c>=&gt; <see langword="await"/> <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async Task AnyAsync<T>(this Task[] @this)
		=> await Task.WhenAny(@this);

	/// <inheritdoc cref="Task.WhenAny{TResult}(Task{TResult}[])"/>
	/// <remarks>
	/// <c>=&gt; <see langword="await"/> <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async Task<Task<T>> AnyAsync<T>(this Task<T>[] @this)
		=> await Task.WhenAny(@this);

	/// <inheritdoc cref="Array.Clear(Array, int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Clear(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> == 0
	/// ? @<paramref name="this"/>.Length
	/// : <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Clear<T>(this T[] @this, int start = 0, int length = 0)
		=> Array.Clear(@this, start, length == 0 ? @this.Length : length);

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<paramref name="tuple"/>.Item1 <see langword="is null"/> || tuple.Item2 <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> count = (<paramref name="tuple"/>.Item1.Length, <paramref name="tuple"/>.Item2.Length).Minimum();<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    yield return"/> (<paramref name="tuple"/>.Item1[i], <paramref name="tuple"/>.Item2[i]);<br/>
	/// </code>
	/// </summary>
	public static IEnumerable<(A, B)> Combine<A, B>((A[], B[]) tuple)
	{
		if (tuple.Item1 is null || tuple.Item2 is null)
			yield break;

		var count = (tuple.Item1.Length, tuple.Item2.Length).Minimum();
		for (var i = 0; i < count; ++i)
			yield return (tuple.Item1[i], tuple.Item2[i]);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<paramref name="tuple"/>.Item1 <see langword="is null"/> || tuple.Item2 <see langword="is null"/> || tuple.Item3 <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> count = ((<paramref name="tuple"/>.Item1.Length, <paramref name="tuple"/>.Item2.Length).Minimum(), <paramref name="tuple"/>.Item3.Length).Minimum();<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    yield return"/> (<paramref name="tuple"/>.Item1[i], <paramref name="tuple"/>.Item2[i], <paramref name="tuple"/>.Item3[i]);<br/>
	/// </code>
	/// </summary>
	public static IEnumerable<(A, B, C)> Combine<A, B, C>((A[], B[], C[]) tuple)
	{
		if (tuple.Item1 is null || tuple.Item2 is null || tuple.Item3 is null)
			yield break;

		var count = ((tuple.Item1.Length, tuple.Item2.Length).Minimum(), tuple.Item3.Length).Minimum();
		for (var i = 0; i < count; ++i)
			yield return (tuple.Item1[i], tuple.Item2[i], tuple.Item3[i]);
	}

	/// <summary>
	/// Can modify the items in the array.
	/// </summary>
	public static void Do<T>(this T[]? @this, ActionRef<T> action)
	{
		action.AssertNotNull();

		var count = @this?.Length ?? 0;
		for (var i = 0; i < count; ++i)
			action(ref @this![i]);
	}

	/// <summary>
	/// Can modify the contents of the array and the looping index.<br/>
	/// index = 0 restarts the loop, --index repeats the current item and ++index skips the next item.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this T[]? @this, ActionRef<T, int> action)
	{
		action.AssertNotNull();

		var count = @this?.Length ?? 0;
		for (var index = 0; index < count; ++index)
			action(ref @this![index], ref index);
	}

	/// <remarks>
	/// <code>
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IsEmpty)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/><paramref name="each"/>(@<paramref name="this"/>[i]);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this T[]? @this, Func<T, T> edit)
	{
		edit.AssertNotNull();

		if (@this is null)
			yield break;

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			yield return edit(@this[i]);
	}

	/// <remarks>
	/// <code>
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IsEmpty)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> count = @<paramref name="this"/>.Length;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="    "/><paramref name="each"/>(@<paramref name="this"/>[i], i);
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this T[]? @this, Func<T, int, T> edit)
	{
		edit.AssertNotNull();

		if (@this is null)
			yield break;

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			yield return edit(@this[i], i);
	}

	/// <exception cref="IndexOutOfRangeException" />
	public static IEnumerable<T> Get<T>(this T[] @this, Range range)
	{
		range = range.Normalize(@this.Length);
		if (!range.Any())
			return Array<T>.Empty;

		var reverse = range.IsReverse() is true;
		if (reverse)
			range = range.Reverse();

		var span = @this.AsSpan(range);
		var copy = new T[span.Length].AsSpan();
		span.CopyTo(copy);
		if (reverse)
			copy.Reverse();

		return copy.ToArray();
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="    return"/> <see cref="Array{T}.Empty"/><br/>
	/// <br/>
	/// <see langword="var"/> copy = <see langword="new"/> <typeparamref name="T"/>[@<paramref name="this"/>.Length];<br/>
	/// @<paramref name="this"/>.AsSpan().CopyTo(copy.AsSpan());<br/>
	/// <see langword="return"/> copy;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="IndexOutOfRangeException"/>
	public static T[] GetCopy<T>(this T[] @this)
	{
		if (!@this.Any())
			return Array<T>.Empty;

		var copy = new T[@this.Length];
		@this.AsSpan().CopyTo(copy.AsSpan());
		return copy;
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> If<T>(this T[]? @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		if (@this?.Length > 0)
		{
			var count = @this.Length;
			for (var i = 0; i < count; ++i)
			{
				var item = @this[i];
				if (filter(item))
					yield return item;
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this T[]? @this, Func<T, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		if (@this?.Length > 0)
		{
			var count = @this.Length;
			for (var i = 0; i < count; ++i)
			{
				if (token.IsCancellationRequested)
					yield break;

				var item = @this[i];
				if (await filter(item))
					yield return item;
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this T[]? @this, Func<T, CancellationToken, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		if (@this?.Length > 0)
		{
			var count = @this.Length;
			for (var i = 0; i < count; ++i)
			{
				if (token.IsCancellationRequested)
					yield break;

				var item = @this[i];
				if (await filter(item, token))
					yield return item;
			}
		}
	}

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InParallel(this Action[] @this)
		=> Parallel.Invoke(@this);

	/// <inheritdoc cref="Parallel.Invoke(ParallelOptions, Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(<paramref name="options"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void InParallel(this Action[] @this, ParallelOptions options)
		=> Parallel.Invoke(options, @this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetUpperBound(<paramref name="dimension"/>) - @<paramref name="this"/>.GetLowerBound(<paramref name="dimension"/>);</c>
	/// </remarks>
	/// <exception cref="IndexOutOfRangeException"/>
	public static int Length<T>(this T[,] @this, int dimension)
		=> @this.GetUpperBound(dimension) - @this.GetLowerBound(dimension);

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this T[]? @this, Func<T, int, V> map)
	{
		map.AssertNotNull();

		if (@this?.Length > 0)
		{
			var count = @this.Length;
			var array = new V[count];
			for (var i = 0; i < count; ++i)
				array[i] = map(@this[i], i);
			return array;
		}

		return Array<V>.Empty;
	}

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this T[]? @this, Func<T, V> map)
	{
		map.AssertNotNull();

		if (@this?.Length > 0)
		{
			var count = @this.Length;
			var array = new V[count];
			for (var i = 0; i < count; ++i)
				array[i] = map(@this![i]);
			return array;
		}

		return Array<V>.Empty;
	}

	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<V> MapAsync<T, V>(this T[]? @this, Func<T, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
	{
		map.AssertNotNull();

		if (@this?.Length > 0)
		{
			var count = @this.Length;
			for (var i = 0; i < count; ++i)
			{
				if (token.IsCancellationRequested)
					yield break;

				yield return await map(@this![i]);
			}
		}
	}

	/// <inheritdoc cref="Array.Reverse{T}(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Reverse(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Reverse<T>(this T[] @this)
		=> Array.Reverse(@this);

	/// <inheritdoc cref="Array.BinarySearch{T}(T[], T, IComparer{T}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.BinarySearch(@<paramref name="this"/>, <paramref name="value"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Search<T>(this T[] @this, T value, IComparer<T>? comparer = null)
		=> Array.BinarySearch(@this, value, comparer);

	/// <inheritdoc cref="Array.BinarySearch{T}(T[], int, int, T, IComparer{T}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.BinarySearch(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> &gt; 0
	/// ? <paramref name="length"/>
	/// : @<paramref name="this"/>.Length, <paramref name="value"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Search<T>(this T[] @this, T value, int start, int length = 0, IComparer<T>? comparer = null)
		=> Array.BinarySearch(@this, start, length > 0 ? length : @this.Length, value, comparer);

	/// <inheritdoc cref="Array.Sort{T}(T[], Comparison{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Sort(@<paramref name="this"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Sort<T>(this T[] @this, Comparison<T> comparison)
		=> Array.Sort(@this, comparison);

	/// <inheritdoc cref="Array.Sort{T}(T[], Comparison{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Sort(@<paramref name="this"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Sort<T>(this T[] @this, IComparer<T>? comparer = null)
		=> Array.Sort(@this, comparer);

	/// <inheritdoc cref="Array.Sort{T}(T[], int, int, IComparer{T}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Sort(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> &gt; 0
	/// ? <paramref name="length"/>
	/// : @<paramref name="this"/>.Length, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Sort<T>(this T[] @this, int start, int length = 0, IComparer<T>? comparer = null)
		=> Array.Sort(@this, start, length > 0 ? length : @this.Length, comparer);

	/// <inheritdoc cref="Array.Copy(Array, int, Array, int, int)"/>
	public static T[] Subarray<T>(this T[] @this, int sourceIndex, int length = 0)
	{
		if (sourceIndex + length > @this.Length)
			throw new IndexOutOfRangeException($"{nameof(Subarray)}: last index {sourceIndex + length} is more than array length {@this.Length}.");

		var array = new T[length > 0 ? length : (@this.Length - sourceIndex)];
		Array.Copy(@this, sourceIndex, array, 0, array.Length);
		return array;
	}

	public static V[] ToArray<T, V>(this T[]? @this, Func<T, V> map)
	{
		if (!@this.Any())
			return Array<V>.Empty;

		var array = new V[@this.Length];
		@this.Do((item, index) => array[index] = map(item));
		return array;
	}

	/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
	/// <remarks>
	/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@<paramref name="this"/>).ToBase64(<paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToBase64(this byte[] @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> ((ReadOnlySpan<byte>)@this).ToBase64(options);

	/// <remarks>
	/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@<paramref name="this"/>).ToBase64Chars(<paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static char[] ToBase64Chars(this byte[] @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> ((ReadOnlySpan<byte>)@this).ToBase64Chars(options);

	/// <inheritdoc cref="BitConverter.ToBoolean(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToBoolean(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool ToBoolean(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToBoolean(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToChar(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToChar(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static char ToChar(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToChar(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToDouble(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToDouble(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static double ToDouble(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToDouble(@this, startIndex);

	public static string ToHex(this byte[] @this)
	{
		const string HEX_CHARS = "0123456789ABCDEF";

		Span<char> chars = stackalloc char[@this.Length * sizeof(char)];
		var bytes = @this.ToReadOnlySpan();
		for (var i = 0; i < bytes.Length; ++i)
		{
			var c = i * 2;
			chars[c] = HEX_CHARS[(bytes[i] & 0xf0) >> 4];
			chars[c + 1] = HEX_CHARS[bytes[i] & 0x0f];
		}
		return new string(chars);
	}

	/// <inheritdoc cref="ImmutableQueue.Create{T}(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="ImmutableQueue"/>.Create(@<paramref name="this"/> ?? <see cref="Array{T}.Empty"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableQueue<T> ToImmutableQueue<T>(this T[]? @this)
		where T : notnull
		=> ImmutableQueue.Create(@this ?? Array<T>.Empty);

	/// <inheritdoc cref="ImmutableStack.Create{T}(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="ToImmutableStack"/>.Create(@<paramref name="this"/> ?? <see cref="Array{T}.Empty"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ImmutableStack<T> ToImmutableStack<T>(this T[]? @this)
		where T : notnull
		=> ImmutableStack.Create(@this ?? Array<T>.Empty);

	/// <inheritdoc cref="BitConverter.ToInt16(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt16(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static short ToInt16(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToInt16(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToInt32(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt32(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int ToInt32(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToInt32(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToInt64(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToInt64(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static long ToInt64(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToInt64(@this, startIndex);

	/// <remarks>
	/// <c>=&gt; <see cref="JsonSerializer"/>.SerializeToNode(@<paramref name="this"/>, <paramref name="options"/>) <see langword="as"/> <see cref="JsonArray"/>;</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static JsonArray? ToJSON<T>(this T[]? @this, JsonSerializerOptions? options = null)
		=> JsonSerializer.SerializeToNode(@this, options) as JsonArray;

	/// <inheritdoc cref="BitConverter.ToSingle(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToSingle(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static float ToSingle(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToSingle(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToString(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToText(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToString(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>, <paramref name="startIndex"/>, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToText(this byte[] @this, int startIndex, int length)
		=> BitConverter.ToString(@this, startIndex, length);

	/// <inheritdoc cref="Encoding.GetString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetString(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToText(this byte[] @this, Encoding encoding)
		=> encoding.GetString(@this);

	/// <inheritdoc cref="Encoding.GetString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetString(@<paramref name="this"/>, <paramref name="index"/>, <paramref name="count"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string ToText(this byte[] @this, Encoding encoding, int index, int count)
		=> encoding.GetString(@this, index, count);

	/// <inheritdoc cref="BitConverter.ToUInt16(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt16(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ushort ToUInt16(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToUInt16(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToUInt32(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt32(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static uint ToUInt32(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToUInt32(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToUInt64(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToUInt64(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ulong ToUInt64(this byte[] @this, int startIndex = 0)
		=> BitConverter.ToUInt64(@this, startIndex);

	/// <inheritdoc cref="Task.WaitAll(Task[], CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAll(@<paramref name="this"/>, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void WaitForAll<T>(this Task[] @this, CancellationToken token = default)
		=> Task.WaitAll(@this, token);

	/// <inheritdoc cref="Task.WaitAll(Task[], int, CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAll(@<paramref name="this"/>, (<see cref="int"/>)<paramref name="timeout"/>.TotalMilliseconds, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void WaitForAll<T>(this Task[] @this, TimeSpan timeout, CancellationToken token = default)
		=> Task.WaitAll(@this, (int)timeout.TotalMilliseconds, token);

	/// <inheritdoc cref="Task.WaitAny(Task[], CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAny(@<paramref name="this"/>, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void WaitForAny<T>(this Task[] @this, CancellationToken token = default)
		=> Task.WaitAny(@this, token);

	/// <inheritdoc cref="Task.WaitAny(Task[], int, CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAny(@<paramref name="this"/>, <paramref name="milliseconds"/>, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void WaitForAny<T>(this Task[] @this, int milliseconds, CancellationToken token = default)
		=> Task.WaitAny(@this, milliseconds, token);
}
