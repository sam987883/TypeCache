// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using TypeCache.Collections;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public delegate void ActionRef<T>(ref T item);

public static class ArrayExtensions
{
	/// <inheritdoc cref="Array.Clear(Array, int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Clear(@<paramref name="this"/>, <paramref name="start"/>, <paramref name="length"/> == 0 ? @<paramref name="this"/>.Length : <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Clear<T>(this T[] @this, int start = 0, int length = 0)
		=> Array.Clear(@this, start, length is 0 ? @this.Length : length);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Exists(item =&gt; <see cref="StringComparer.OrdinalIgnoreCase"/>.Equals(item, <paramref name="value"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsIgnoreCase(this string[] @this, string value)
		=> @this.Exists(item => StringComparer.OrdinalIgnoreCase.Equals(item, value));

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Exists(item =&gt; <see cref="StringComparer.Ordinal"/>.Equals(item, <paramref name="value"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsOrdinal(this string[] @this, string value)
		=> @this.Exists(item => StringComparer.Ordinal.Equals(item, value));

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	public static T[] Copy<T>(this T[] @this)
	{
		@this.ThrowIfNull();

		if (@this.Length is 0)
			return [];

		var copy = new T[@this.Length];
		@this.AsSpan().CopyTo(copy);
		return copy;
	}

	/// <inheritdoc cref="Array.Exists{T}(T[], Predicate{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array"/>.Exists(@<paramref name="this"/>, <see langword="new"/> <see cref="Predicate{T}"/>(<paramref name="match"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Exists<T>(this T[] @this, Func<T, bool> match)
		=> Array.Exists(@this, new Predicate<T>(match));

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[] @this, Action<T> action)
	{
		@this.ThrowIfNull();

		@this.AsSpan().AsReadOnly().ForEach(action);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[] @this, Action<T, int> action)
	{
		@this.ThrowIfNull();

		@this.AsSpan().AsReadOnly().ForEach(action);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[] @this, Action<T> action, Action between)
	{
		@this.ThrowIfNull();

		@this.AsSpan().AsReadOnly().ForEach(action, between);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[] @this, Action<T, int> action, Action between)
	{
		@this.ThrowIfNull();

		@this.AsSpan().AsReadOnly().ForEach(action, between);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this T[] @this, ActionRef<T> action)
	{
		@this.ThrowIfNull();
		action.ThrowIfNull();

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			action(ref @this![i]);
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
		=> Array.BinarySearch(@this, start, length is 0 ? @this.Length : length, value, comparer);

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
		=> Array.Sort(@this, start, length is 0 ? @this.Length : length, comparer);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="IndexOutOfRangeException"/>
	public static T[] Subarray<T>(this T[] @this, int sourceIndex, int length = 0)
	{
		@this.ThrowIfNull();

		if (sourceIndex + length > @this.Length)
			throw new IndexOutOfRangeException($"{nameof(Subarray)}: last index {sourceIndex + length} is more than array length {@this.Length}.");

		var array = new T[length is 0 ? @this.Length - sourceIndex : length];
		Array.Copy(@this, sourceIndex, array, 0, array.Length);
		return array;
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Any(_ =&gt; _ &gt; (<see cref="byte"/>)0);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ToBoolean(this byte[] @this)
		=> @this.Any(_ => _ > (byte)0);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Skip(<paramref name="startIndex"/>).Any(_ =&gt; _ &gt; (<see cref="byte"/>)0);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ToBoolean(this byte[] @this, int startIndex)
		=> @this.Skip(startIndex).Any(_ => _ > (byte)0);

	/// <remarks>
	/// <c>=&gt; <see cref="JsonSerializer"/>.SerializeToNode(@<paramref name="this"/>, <paramref name="options"/>) <see langword="as"/> <see cref="JsonArray"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static JsonArray? ToJSON<T>(this T[]? @this, JsonSerializerOptions? options = null)
		=> JsonSerializer.SerializeToNode(@this, options) as JsonArray;

	/// <inheritdoc cref="ArraySegment{T}.ArraySegment(T[])"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ArraySegment<T> ToSegment<T>(this T[] @this)
		=> new(@this);

	/// <inheritdoc cref="ArraySegment{T}.ArraySegment(T[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>, <paramref name="offset"/>, <paramref name="count"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ArraySegment<T> ToSegment<T>(this T[] @this, int offset, int count)
		=> new(@this, offset, count);

	/// <inheritdoc cref="Encoding.GetString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetString(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToString(this byte[] @this, Encoding encoding)
		=> encoding.GetString(@this);

	/// <inheritdoc cref="Encoding.GetString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="encoding"/>.GetString(@<paramref name="this"/>, <paramref name="index"/>, <paramref name="count"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToString(this byte[] @this, Encoding encoding, int index, int count)
		=> encoding.GetString(@this, index, count);

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

	/// <inheritdoc cref="Convert.ToHexString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.ToHexString(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToHexString(this byte[] @this)
		=> Convert.ToHexString(@this);

	/// <inheritdoc cref="Convert.ToHexString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.ToHexString(@<paramref name="this"/>, <paramref name="offset"/>, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToHexString(this byte[] @this, int offset, int length)
		=> Convert.ToHexString(@this, offset, length);

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
	/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@<paramref name="this"/>).ToNumber&lt;<typeparamref name="T"/>&gt;();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T ToNumber<T>(this byte[] @this)
		where T : struct, INumber<T>
		=> ((ReadOnlySpan<byte>)@this).ToNumber<T>();

	public static T ToNumber<T>(this byte[] @this, int startIndex)
		where T : struct, INumber<T>
		=> typeof(T).ScalarType() switch
		{
			ScalarType.Char => Unsafe.BitCast<char, T>(BitConverter.ToChar(@this, startIndex)),
			ScalarType.SByte => Unsafe.BitCast<sbyte, T>((sbyte)BitConverter.ToInt32(@this, startIndex)),
			ScalarType.Int16 => Unsafe.BitCast<short, T>(BitConverter.ToInt16(@this, startIndex)),
			ScalarType.Int32 => Unsafe.BitCast<int, T>(BitConverter.ToInt32(@this, startIndex)),
			ScalarType.IntPtr => Unsafe.BitCast<nint, T>(BitConverter.ToInt32(@this, startIndex)),
			ScalarType.Int64 => Unsafe.BitCast<long, T>(BitConverter.ToInt64(@this, startIndex)),
			ScalarType.BigInteger => Unsafe.BitCast<BigInteger, T>(new BigInteger(@this.AsSpan().Slice(startIndex))),
			ScalarType.Byte => Unsafe.BitCast<byte, T>((byte)BitConverter.ToUInt32(@this, startIndex)),
			ScalarType.UInt16 => Unsafe.BitCast<ushort, T>(BitConverter.ToUInt16(@this, startIndex)),
			ScalarType.UInt32 => Unsafe.BitCast<uint, T>(BitConverter.ToUInt32(@this, startIndex)),
			ScalarType.UIntPtr => Unsafe.BitCast<nuint, T>(BitConverter.ToUInt32(@this, startIndex)),
			ScalarType.UInt64 => Unsafe.BitCast<ulong, T>(BitConverter.ToUInt64(@this, startIndex)),
			ScalarType.Half => Unsafe.BitCast<Half, T>(BitConverter.ToHalf(@this, startIndex)),
			ScalarType.Single => Unsafe.BitCast<float, T>(BitConverter.ToSingle(@this, startIndex)),
			ScalarType.Double => Unsafe.BitCast<double, T>(BitConverter.ToDouble(@this, startIndex)),
			ScalarType.Decimal => Unsafe.BitCast<decimal, T>(new decimal(@this.AsSpan().Slice(startIndex).Cast<byte, int>())),
			var scalarType => throw new UnreachableException(Invariant($"Cannot convert bytes to {scalarType.Name()}."))
		};

	/// <inheritdoc cref="BitConverter.ToString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this byte[] @this)
		=> BitConverter.ToString(@this);

	/// <inheritdoc cref="BitConverter.ToString(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>, <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this byte[] @this, int startIndex)
		=> BitConverter.ToString(@this, startIndex);

	/// <inheritdoc cref="BitConverter.ToString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.Span, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this byte[] @this, int startIndex, int length)
		=> BitConverter.ToString(@this, startIndex, length);

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
