// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Collections;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public delegate void ActionRef<T>(ref T item);

public static class ArrayExtensions
{
	extension(Array @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.GetUpperBound(<paramref name="dimension"/>) - @this.GetLowerBound(<paramref name="dimension"/>) + 1;</c>
		/// </remarks>
		/// <exception cref="IndexOutOfRangeException"/>
		public int Length(int dimension)
			=> @this.GetUpperBound(dimension) - @this.GetLowerBound(dimension) + 1;
	}

	extension<T>(T[] @this)
	{
		/// <inheritdoc cref="Array.Clear(Array, int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Array"/>.Clear(@this, <paramref name="start"/>, <paramref name="length"/> == 0 ? @this.Length : <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void Clear(int start = 0, int length = 0)
			=> Array.Clear(@this, start, length is 0 ? @this.Length : length);

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		public T[] Copy()
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
		/// <c>=&gt; <see cref="Array"/>.Exists(@this, <see langword="new"/> <see cref="Predicate{T}"/>(<paramref name="match"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Exists(Func<T, bool> match)
			=> Array.Exists(@this, new Predicate<T>(match));

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T> action)
		{
			@this.ThrowIfNull();

			@this.AsSpan().AsReadOnly().ForEach(action);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T, int> action)
		{
			@this.ThrowIfNull();

			@this.AsSpan().AsReadOnly().ForEach(action);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T> action, Action between)
		{
			@this.ThrowIfNull();

			@this.AsSpan().AsReadOnly().ForEach(action, between);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T, int> action, Action between)
		{
			@this.ThrowIfNull();

			@this.AsSpan().AsReadOnly().ForEach(action, between);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(ActionRef<T> action)
		{
			@this.ThrowIfNull();
			action.ThrowIfNull();

			var count = @this.Length;
			for (var i = 0; i < count; ++i)
				action(ref @this![i]);
		}

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="IndexOutOfRangeException"/>
		public T[] Subarray(int sourceIndex, int length = 0)
		{
			@this.ThrowIfNull();

			if (sourceIndex + length > @this.Length)
				throw new IndexOutOfRangeException($"{nameof(Subarray)}: last index {sourceIndex + length} is more than array length {@this.Length}.");

			if (length is 0)
				length = @this.Length - sourceIndex;

			return @this.AsSpan(sourceIndex, length).ToArray();
		}

		/// <inheritdoc cref="ImmutableQueue.Create{T}(T[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="ImmutableQueue"/>.Create(@this ?? <see cref="Array{T}.Empty"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ImmutableQueue<T> ToImmutableQueue()
			=> ImmutableQueue.Create(@this);

		/// <inheritdoc cref="ImmutableStack.Create{T}(T[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="ToImmutableStack"/>.Create(@this ?? <see cref="Array{T}.Empty"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ImmutableStack<T> ToImmutableStack()
			=> ImmutableStack.Create(@this);

		/// <remarks>
		/// <c>=&gt; <see cref="JsonSerializer"/>.SerializeToNode(@this, <paramref name="options"/>) <see langword="as"/> <see cref="JsonArray"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public JsonArray? ToJSON(JsonSerializerOptions? options = null)
			=> JsonSerializer.SerializeToNode(@this, options) as JsonArray;

		/// <inheritdoc cref="ArraySegment{T}.ArraySegment(T[])"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ArraySegment<T> ToSegment()
			=> new(@this);

		/// <inheritdoc cref="ArraySegment{T}.ArraySegment(T[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this, <paramref name="offset"/>, <paramref name="count"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ArraySegment<T> ToSegment(int offset, int count)
			=> new(@this, offset, count);
	}

	extension(string[] @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.Exists(item =&gt; <see cref="StringComparer.OrdinalIgnoreCase"/>.Equals(item, <paramref name="value"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsIgnoreCase(string value)
			=> @this.Exists(item => StringComparer.OrdinalIgnoreCase.Equals(item, value));

		/// <remarks>
		/// <c>=&gt; @this.Exists(item =&gt; <see cref="StringComparer.Ordinal"/>.Equals(item, <paramref name="value"/>));</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsOrdinal(string value)
			=> @this.Exists(item => StringComparer.Ordinal.Equals(item, value));
	}

	extension(char[] @this)
	{
		/// <inheritdoc cref="Convert.FromBase64CharArray(char[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.FromBase64CharArray(@this, 0, @this.Length);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public byte[] FromBase64()
			=> Convert.FromBase64CharArray(@this, 0, @this.Length);

		/// <inheritdoc cref="Convert.FromBase64CharArray(char[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Base64Url"/>.DecodeFromChars(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public byte[] FromBase64Url()
			=> Base64Url.DecodeFromChars(@this);

		/// <inheritdoc cref="Convert.FromBase64CharArray(char[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.FromBase64CharArray(@this, <paramref name="offset"/>, <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public byte[] FromBase64(int offset, int length)
			=> Convert.FromBase64CharArray(@this, offset, length);
	}

	extension(Action[] @this)
	{
		/// <inheritdoc cref="Parallel.Invoke(Action[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Parallel"/>.Invoke(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void InParallel()
			=> Parallel.Invoke(@this);

		/// <inheritdoc cref="Parallel.Invoke(ParallelOptions, Action[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Parallel"/>.Invoke(<paramref name="options"/>, @this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void InParallel(ParallelOptions options)
			=> Parallel.Invoke(options, @this);
	}

	extension(byte[] @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.Any(_ =&gt; _ &gt; (<see cref="byte"/>)0);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ToBoolean()
			=> @this.Any(_ => _ > (byte)0);

		/// <remarks>
		/// <c>=&gt; @this.Skip(<paramref name="startIndex"/>).Any(_ =&gt; _ &gt; (<see cref="byte"/>)0);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ToBoolean(int startIndex)
			=> @this.Skip(startIndex).Any(_ => _ > (byte)0);

		/// <inheritdoc cref="Encoding.GetString(byte[])"/>
		/// <remarks>
		/// <c>=&gt; <paramref name="encoding"/>.GetString(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToString(Encoding encoding)
			=> encoding.GetString(@this);

		/// <inheritdoc cref="Encoding.GetString(byte[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <paramref name="encoding"/>.GetString(@this, <paramref name="index"/>, <paramref name="count"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToString(Encoding encoding, int index, int count)
			=> encoding.GetString(@this, index, count);

		/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToBase64String(@this, <paramref name="options"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToBase64(Base64FormattingOptions options = Base64FormattingOptions.None)
			=> Convert.ToBase64String(@this, options);

		/// <remarks>
		/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@this).ToBase64Chars(<paramref name="options"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char[] ToBase64Chars(Base64FormattingOptions options = Base64FormattingOptions.None)
			=> ((ReadOnlySpan<byte>)@this).ToBase64Chars(options);

		/// <inheritdoc cref="Base64Url.EncodeToString(ReadOnlySpan{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Base64Url"/>.EncodeToString(@this.AsSpan().AsReadOnly());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToBase64Url()
			=> Base64Url.EncodeToString(@this.AsSpan().AsReadOnly());

		/// <inheritdoc cref="Base64Url.EncodeToChars(ReadOnlySpan{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Base64Url"/>.EncodeToChars(@this.AsSpan().AsReadOnly());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char[] ToBase64UrlChars()
			=> Base64Url.EncodeToChars(@this.AsSpan().AsReadOnly());

		/// <inheritdoc cref="Convert.ToHexString(byte[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToHexString(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToHexString()
			=> Convert.ToHexString(@this);

		/// <inheritdoc cref="Convert.ToHexString(byte[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToHexString(@this, <paramref name="offset"/>, <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToHexString(int offset, int length)
			=> Convert.ToHexString(@this, offset, length);

		/// <inheritdoc cref="Convert.ToHexStringLower(byte[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToHexStringLower(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToHexStringLower()
			=> Convert.ToHexStringLower(@this);

		/// <inheritdoc cref="Convert.ToHexStringLower(byte[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToHexStringLower(@this, <paramref name="offset"/>, <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToHexStringLower(int offset, int length)
			=> Convert.ToHexStringLower(@this, offset, length);

		/// <remarks>
		/// <c>=&gt; ((ReadOnlySpan&lt;<see cref="byte"/>&gt;)@this).ToNumber&lt;<typeparamref name="T"/>&gt;();</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T ToNumber<T>()
			where T : struct, INumber<T>
			=> ((ReadOnlySpan<byte>)@this).ToNumber<T>();

		public T ToNumber<T>(int startIndex)
			where T : struct, INumber<T>
			=> typeof(T).ScalarType switch
			{
				ScalarType.Char => Unsafe.BitCast<char, T>(BitConverter.ToChar(@this, startIndex)),
				ScalarType.SByte => Unsafe.BitCast<sbyte, T>((sbyte)BitConverter.ToInt32(@this, startIndex)),
				ScalarType.Int16 => Unsafe.BitCast<short, T>(BitConverter.ToInt16(@this, startIndex)),
				ScalarType.Int32 => Unsafe.BitCast<int, T>(BitConverter.ToInt32(@this, startIndex)),
				ScalarType.IntPtr => Unsafe.BitCast<nint, T>(BitConverter.ToInt32(@this, startIndex)),
				ScalarType.Int64 => Unsafe.BitCast<long, T>(BitConverter.ToInt64(@this, startIndex)),
				ScalarType.Int128 => Unsafe.BitCast<Int128, T>(BitConverter.ToInt128(@this, startIndex)),
				ScalarType.BigInteger => Unsafe.BitCast<BigInteger, T>(new BigInteger(@this.AsSpan().Slice(startIndex))),
				ScalarType.Byte => Unsafe.BitCast<byte, T>((byte)BitConverter.ToUInt32(@this, startIndex)),
				ScalarType.UInt16 => Unsafe.BitCast<ushort, T>(BitConverter.ToUInt16(@this, startIndex)),
				ScalarType.UInt32 => Unsafe.BitCast<uint, T>(BitConverter.ToUInt32(@this, startIndex)),
				ScalarType.UIntPtr => Unsafe.BitCast<nuint, T>(BitConverter.ToUInt32(@this, startIndex)),
				ScalarType.UInt64 => Unsafe.BitCast<ulong, T>(BitConverter.ToUInt64(@this, startIndex)),
				ScalarType.UInt128 => Unsafe.BitCast<UInt128, T>(BitConverter.ToUInt128(@this, startIndex)),
				ScalarType.Half => Unsafe.BitCast<Half, T>(BitConverter.ToHalf(@this, startIndex)),
				ScalarType.Single => Unsafe.BitCast<float, T>(BitConverter.ToSingle(@this, startIndex)),
				ScalarType.Double => Unsafe.BitCast<double, T>(BitConverter.ToDouble(@this, startIndex)),
				ScalarType.Decimal => Unsafe.BitCast<decimal, T>(new decimal(@this.AsSpan().Slice(startIndex).Cast<byte, int>())),
				var scalarType => throw new UnreachableException(Invariant($"Cannot convert bytes to {scalarType.Name}."))
			};

		/// <inheritdoc cref="BitConverter.ToString(byte[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="BitConverter"/>.ToString(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText()
			=> BitConverter.ToString(@this);

		/// <inheritdoc cref="BitConverter.ToString(byte[], int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="BitConverter"/>.ToString(@this, <paramref name="startIndex"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText(int startIndex)
			=> BitConverter.ToString(@this, startIndex);

		/// <inheritdoc cref="BitConverter.ToString(byte[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="BitConverter"/>.ToString(@this.Span, <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText(int startIndex, int length)
			=> BitConverter.ToString(@this, startIndex, length);
	}

	extension(Task[] @this)
	{
		/// <inheritdoc cref="Task.WaitAll(Task[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WaitAll(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void WaitAll()
			=> Task.WaitAll(@this);

		/// <inheritdoc cref="Task.WaitAll(Task[], CancellationToken)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WaitAll(@this, <paramref name="token"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void WaitAll(CancellationToken token)
			=> Task.WaitAll(@this, token);

		/// <inheritdoc cref="Task.WaitAll(Task[], CancellationToken)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WaitAll(@this, (<see cref="int"/>)<paramref name="timeout"/>.TotalMilliseconds, <paramref name="token"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void WaitAll(TimeSpan timeout, CancellationToken token)
			=> Task.WaitAll(@this, (int)timeout.TotalMilliseconds, token);

		/// <inheritdoc cref="Task.WhenAll(Task[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WhenAll(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Task WhenAll()
			=> Task.WhenAll(@this);

		/// <inheritdoc cref="Task.WaitAny(Task[], CancellationToken)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WaitAny(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void WaitAny()
			=> Task.WaitAny(@this);

		/// <inheritdoc cref="Task.WaitAny(Task[], CancellationToken)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WaitAny(@this, <paramref name="token"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void WaitAny(CancellationToken token)
			=> Task.WaitAny(@this, token);

		/// <inheritdoc cref="Task.WaitAny(Task[], int, CancellationToken)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WaitAny(@this, (<see cref="int"/>)<paramref name="timeout"/>.TotalMilliseconds, <paramref name="token"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void WaitAny(TimeSpan timeout, CancellationToken token)
			=> Task.WaitAny(@this, (int)timeout.TotalMilliseconds, token);

		/// <inheritdoc cref="Task.WhenAny(Task[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WhenAny(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Task WhenAny()
			=> Task.WhenAny(@this);
	}

	extension<T>(Task<T>[] @this)
	{
		/// <inheritdoc cref="Task.WhenAll{TResult}(Task{TResult}[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WhenAll(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Task<T[]> WhenAll()
			=> Task.WhenAll(@this);

		/// <inheritdoc cref="Task.WhenAny{TResult}(Task{TResult}[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Task"/>.WhenAny(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Task<Task<T>> WhenAny()
			=> Task.WhenAny(@this);
	}
}
