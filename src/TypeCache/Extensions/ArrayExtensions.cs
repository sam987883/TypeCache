// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Extensions;

public static class ArrayExtensions
{
	/// <inheritdoc cref="Array.Clear(Array, int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Clear(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> == 0 ? @<paramref name="this"/>.Length : <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Clear<T>(this T[] @this, int start = 0, int length = 0)
		=> Array.Clear(@this, start, length == 0 ? @this.Length : length);

	/// <inheritdoc cref="Array.ForEach{T}(T[], Action{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.ForEach(@<paramref name="this"/>, action);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void ForEach<T>(this T[] @this, Action<T> action)
		=> Array.ForEach(@this, action);

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

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[]? @this, ActionRef<T, int> action)
	{
		action.AssertNotNull();

		var count = @this?.Length ?? 0;
		for (var i = 0; i < count; ++i)
			action(ref @this![i], ref i);
	}

	public static void ForEach<T>(this T[] @this, Action<T, int> action)
	{
		var i = -1;
		@this.ForEach(value => action(value, ++i));
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

	public static IEnumerable<T> Get<T>(this T[] @this, Range range)
	{
		range = range.Normalize(@this.Length);
		if (range.Any() is not true)
			return Array<T>.Empty;

		var reverse = range.IsReverse() is true;
		if (reverse)
			range = range.Reverse();

		var span = @this.AsSpan(range);
		var copy = new T[span.Length];
		var copySpan = copy.AsSpan();
		span.CopyTo(copySpan);
		if (reverse)
			copySpan.Reverse();

		return copy;
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	public static T[] GetCopy<T>(this T[] @this)
	{
		if (!@this.Any())
			return Array<T>.Empty;

		var copy = new T[@this.Length];
		@this.AsSpan().CopyTo(copy);
		return copy;
	}

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
	/// <c>=&gt; @<paramref name="this"/>.GetUpperBound(<paramref name="dimension"/>) - @<paramref name="this"/>.GetLowerBound(<paramref name="dimension"/>);</c>
	/// </remarks>
	/// <exception cref="IndexOutOfRangeException"/>
	public static int Length<T>(this T[,] @this, int dimension)
		=> @this.GetUpperBound(dimension) - @this.GetLowerBound(dimension);

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

	/// <exception cref="IndexOutOfRangeException"/>
	public static T[] Subarray<T>(this T[] @this, int sourceIndex, int length = 0)
	{
		if (sourceIndex + length > @this.Length)
			throw new IndexOutOfRangeException($"{nameof(Subarray)}: last index {sourceIndex + length} is more than array length {@this.Length}.");

		var array = new T[length > 0 ? length : @this.Length - sourceIndex];
		Array.Copy(@this, sourceIndex, array, 0, array.Length);
		return array;
	}

	/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
	/// <remarks>
	/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@<paramref name="this"/>).ToBase64(<paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToBase64(this byte[] @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> ((ReadOnlySpan<byte>)@this).ToBase64(options);

	/// <remarks>
	/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@<paramref name="this"/>).ToBase64Chars(<paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char[] ToBase64Chars(this byte[] @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> ((ReadOnlySpan<byte>)@this).ToBase64Chars(options);

	public static string ToHex(this byte[] @this)
	{
		const string HEX_CHARS = "0123456789ABCDEF";

		Span<char> chars = stackalloc char[@this.Length * sizeof(char)];
		var bytes = @this.AsSpan().AsReadOnly();
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
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ImmutableQueue<T> ToImmutableQueue<T>(this T[]? @this)
		where T : notnull
		=> ImmutableQueue.Create(@this ?? Array<T>.Empty);

	/// <inheritdoc cref="ImmutableStack.Create{T}(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="ToImmutableStack"/>.Create(@<paramref name="this"/> ?? <see cref="Array{T}.Empty"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ImmutableStack<T> ToImmutableStack<T>(this T[]? @this)
		where T : notnull
		=> ImmutableStack.Create(@this ?? Array<T>.Empty);

	/// <remarks>
	/// <c>=&gt; <see cref="JsonSerializer"/>.SerializeToNode(@<paramref name="this"/>, <paramref name="options"/>) <see langword="as"/> <see cref="JsonArray"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static JsonArray? ToJSON<T>(this T[]? @this, JsonSerializerOptions? options = null)
		=> JsonSerializer.SerializeToNode(@this, options) as JsonArray;

	/// <inheritdoc cref="Encoding.GetString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetString(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this byte[] @this, Encoding encoding)
		=> encoding.GetString(@this);

	/// <inheritdoc cref="Encoding.GetString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetString(@<paramref name="this"/>, <paramref name="index"/>, <paramref name="count"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this byte[] @this, Encoding encoding, int index, int count)
		=> encoding.GetString(@this, index, count);

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
