// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Collections;

namespace TypeCache.Extensions;

public delegate void ActionRef<T>(ref T item);
public delegate void ActionIndexRef<T>(ref T item, int index);

public static class ArrayExtensions
{
	/// <inheritdoc cref="Array.Clear(Array, int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Clear(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> == 0 ? @<paramref name="this"/>.Length : <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Clear<T>(this T[] @this, int start = 0, int length = 0)
		=> Array.Clear(@this, start, length == 0 ? @this.Length : length);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	public static T[] Copy<T>(this T[] @this)
	{
		@this.AssertNotNull();

		if (@this.Length == 0)
			return Array<T>.Empty;

		var copy = new T[@this.Length];
		@this.AsSpan().CopyTo(copy);
		return copy;
	}

	/// <inheritdoc cref="Array.ForEach{T}(T[], Action{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.ForEach(@<paramref name="this"/>, action);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void ForEach<T>(this T[] @this, Action<T> action)
		=> Array.ForEach(@this, action);

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[] @this, Action<T> action, Action between)
	{
		between.AssertNotNull();

		var first = true;
		@this.ForEach(value =>
		{
			if (first)
				first = false;
			else
				between();

			action(value);
		});
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[]? @this, ActionRef<T> action)
	{
		action.AssertNotNull();

		var count = @this?.Length ?? 0;
		for (var i = 0; i < count; ++i)
			action(ref @this![i]);
	}

	public static void ForEach<T>(this T[] @this, Action<T, int> action)
	{
		var i = -1;
		@this.ForEach(value => action(value, ++i));
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[]? @this, ActionIndexRef<T> action)
	{
		action.AssertNotNull();

		var count = @this?.Length ?? 0;
		for (var i = 0; i < count; ++i)
			action(ref @this![i], i);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[] @this, Action<T, int> action, Action between)
	{
		between.AssertNotNull();

		var i = -1;
		@this.ForEach(value =>
		{
			if (++i > 0)
				between();

			action(value, i);
		});
	}

	/// <inheritdoc cref="Convert.FromBase64CharArray(char[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.FromBase64CharArray(@<paramref name="this"/>, 0, @<paramref name="this"/>.Length);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] FromBase64(this char[] @this)
		=> Convert.FromBase64CharArray(@this, 0, @this.Length);

	/// <inheritdoc cref="Convert.FromBase64CharArray(char[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.FromBase64CharArray(@<paramref name="this"/>, <paramref name="offset"/>, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] FromBase64(this char[] @this, int offset, int length)
		=> Convert.FromBase64CharArray(@this, offset, length);

	/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InParallel(this Action[] @this)
		=> Parallel.Invoke(@this);

	/// <inheritdoc cref="Parallel.Invoke(ParallelOptions, Action[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Parallel"/>.Invoke(<paramref name="options"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void InParallel(this Action[] @this, ParallelOptions options)
		=> Parallel.Invoke(options, @this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.GetUpperBound(<paramref name="dimension"/>) - @<paramref name="this"/>.GetLowerBound(<paramref name="dimension"/>) + 1;</c>
	/// </remarks>
	/// <exception cref="IndexOutOfRangeException"/>
	public static int Length(this Array @this, int dimension)
		=> @this.GetUpperBound(dimension) - @this.GetLowerBound(dimension) + 1;

	/// <inheritdoc cref="Array.Reverse{T}(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Reverse(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Reverse<T>(this T[] @this)
		=> Array.Reverse(@this);

	/// <inheritdoc cref="Array.BinarySearch{T}(T[], T, IComparer{T}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.BinarySearch(@<paramref name="this"/>, <paramref name="value"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Search<T>(this T[] @this, T value, IComparer<T>? comparer = null)
		=> Array.BinarySearch(@this, value, comparer);

	/// <inheritdoc cref="Array.BinarySearch{T}(T[], int, int, T, IComparer{T}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.BinarySearch(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> &gt; 0
	/// ? <paramref name="length"/>
	/// : @<paramref name="this"/>.Length, <paramref name="value"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Search<T>(this T[] @this, T value, int start, int length = 0, IComparer<T>? comparer = null)
		=> Array.BinarySearch(@this, start, length > 0 ? length : @this.Length, value, comparer);

	/// <inheritdoc cref="Array.Sort{T}(T[], Comparison{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Sort(@<paramref name="this"/>, <paramref name="comparison"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Sort<T>(this T[] @this, Comparison<T> comparison)
		=> Array.Sort(@this, comparison);

	/// <inheritdoc cref="Array.Sort{T}(T[], Comparison{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Sort(@<paramref name="this"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Sort<T>(this T[] @this, IComparer<T>? comparer = null)
		=> Array.Sort(@this, comparer);

	/// <inheritdoc cref="Array.Sort{T}(T[], int, int, IComparer{T}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Sort(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> &gt; 0
	/// ? <paramref name="length"/>
	/// : @<paramref name="this"/>.Length, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Sort<T>(this T[] @this, int start, int length = 0, IComparer<T>? comparer = null)
		=> Array.Sort(@this, start, length > 0 ? length : @this.Length, comparer);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="IndexOutOfRangeException"/>
	public static T[] Subarray<T>(this T[] @this, int sourceIndex, int length = 0)
	{
		@this.AssertNotNull();

		if (sourceIndex + length > @this.Length)
			throw new IndexOutOfRangeException($"{nameof(Subarray)}: last index {sourceIndex + length} is more than array length {@this.Length}.");

		var array = new T[length > 0 ? length : @this.Length - sourceIndex];
		Array.Copy(@this, sourceIndex, array, 0, array.Length);
		return array;
	}

	/// <inheritdoc cref="ArraySegment{T}.ArraySegment(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ArraySegment<T> Segment<T>(this T[] @this)
		=> new(@this);

	/// <inheritdoc cref="ArraySegment{T}.ArraySegment(T[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>, <paramref name="offset"/>, <paramref name="count"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ArraySegment<T> Segment<T>(this T[] @this, int offset, int count)
		=> new(@this, offset, count);

	/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.ToBase64String(@<paramref name="this"/>, <paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToBase64(this byte[] @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> Convert.ToBase64String(@this, options);

	/// <remarks>
	/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@<paramref name="this"/>).ToBase64Chars(<paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char[] ToBase64Chars(this byte[] @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> ((ReadOnlySpan<byte>)@this).ToBase64Chars(options);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.AsSpan().AsReadOnly().ToHex();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToHexString(this byte[] @this)
		=> @this.AsSpan().AsReadOnly().ToHexString();

	/// <inheritdoc cref="ImmutableQueue.Create{T}(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="ImmutableQueue"/>.Create(@<paramref name="this"/> ?? <see cref="Array{T}.Empty"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ImmutableQueue<T> ToImmutableQueue<T>(this T[] @this)
		where T : notnull
		=> ImmutableQueue.Create(@this);

	/// <inheritdoc cref="ImmutableStack.Create{T}(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="ToImmutableStack"/>.Create(@<paramref name="this"/> ?? <see cref="Array{T}.Empty"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ImmutableStack<T> ToImmutableStack<T>(this T[] @this)
		where T : notnull
		=> ImmutableStack.Create(@this);

	/// <remarks>
	/// <c>=&gt; <see cref="JsonSerializer"/>.SerializeToNode(@<paramref name="this"/>, <paramref name="options"/>) <see langword="as"/> <see cref="JsonArray"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static JsonArray? ToJSON<T>(this T[]? @this, JsonSerializerOptions? options = null)
		=> JsonSerializer.SerializeToNode(@this, options) as JsonArray;

	/// <inheritdoc cref="Task.WaitAll(Task[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAll(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void WaitAll(this Task[] @this)
		=> Task.WaitAll(@this);

	/// <inheritdoc cref="Task.WaitAll(Task[], CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAll(@<paramref name="this"/>, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void WaitAll(this Task[] @this, CancellationToken token)
		=> Task.WaitAll(@this, token);

	/// <inheritdoc cref="Task.WaitAll(Task[], CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAll(@<paramref name="this"/>, (<see cref="int"/>)<paramref name="timeout"/>.TotalMilliseconds, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void WaitAll(this Task[] @this, TimeSpan timeout, CancellationToken token)
		=> Task.WaitAll(@this, (int)timeout.TotalMilliseconds, token);

	/// <inheritdoc cref="Task.WhenAll(Task[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WhenAll(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Task WhenAllAsync<T>(this Task[] @this)
		=> Task.WhenAll(@this);

	/// <inheritdoc cref="Task.WhenAll{TResult}(Task{TResult}[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WhenAll(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Task<T[]> WhenAllAsync<T>(this Task<T>[] @this)
		=> Task.WhenAll(@this);

	/// <inheritdoc cref="Task.WaitAny(Task[], CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAny(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void WaitAny<T>(this Task[] @this)
		=> Task.WaitAny(@this);

	/// <inheritdoc cref="Task.WaitAny(Task[], CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAny(@<paramref name="this"/>, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void WaitAny<T>(this Task[] @this, CancellationToken token)
		=> Task.WaitAny(@this, token);

	/// <inheritdoc cref="Task.WaitAny(Task[], int, CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WaitAny(@<paramref name="this"/>, (<see cref="int"/>)<paramref name="timeout"/>.TotalMilliseconds, <paramref name="token"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void WaitAny<T>(this Task[] @this, TimeSpan timeout, CancellationToken token)
		=> Task.WaitAny(@this, (int)timeout.TotalMilliseconds, token);

	/// <inheritdoc cref="Task.WhenAny(Task[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Task WhenAnyAsync<T>(this Task[] @this)
		=> Task.WhenAny(@this);

	/// <inheritdoc cref="Task.WhenAny{TResult}(Task{TResult}[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Task<Task<T>> WhenAnyAsync<T>(this Task<T>[] @this)
		=> Task.WhenAny(@this);
}
