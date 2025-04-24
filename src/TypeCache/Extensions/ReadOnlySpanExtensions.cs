// Copyright (c) 2021 Samuel Abraham

using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class ReadOnlySpanExtensions
{
	/// <inheritdoc cref="MemoryMarshal.AsBytes{T}(ReadOnlySpan{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ReadOnlySpan<byte> AsBytes<T>(this ReadOnlySpan<T> @this)
		where T : struct
		=> MemoryMarshal.AsBytes(@this);

	/// <inheritdoc cref="MemoryMarshal.AsRef{T}(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="ref"/> <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ref readonly T AsRef<T>(this ReadOnlySpan<byte> @this)
		where T : struct
		=> ref MemoryMarshal.AsRef<T>(@this);

	/// <inheritdoc cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ReadOnlySpan<R> Cast<T, R>(this ReadOnlySpan<T> @this)
		where T : struct
		where R : struct
		=> MemoryMarshal.Cast<T, R>(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<paramref name="other"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool EqualsIgnoreCase(this ReadOnlySpan<char> @this, ReadOnlySpan<char> other)
		=> @this.Equals(other, StringComparison.OrdinalIgnoreCase);

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this ReadOnlySpan<T> @this, Action<T> action)
	{
		action.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		foreach (var item in @this)
			action(item);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this ReadOnlySpan<T> @this, Action<T, int> action)
	{
		action.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		var i = -1;
		foreach (var item in @this)
			action(item, ++i);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this ReadOnlySpan<T> @this, Action<T> action, Action between)
	{
		action.ThrowIfNull();
		between.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		action(@this[0]);
		var slice = @this.Slice(1);
		if (slice.IsEmpty)
			return;

		foreach (var item in slice)
		{
			between();
			action(item);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this ReadOnlySpan<T> @this, Action<T, int> action, Action between)
	{
		action.ThrowIfNull();
		between.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		action(@this[0], 0);
		var slice = @this.Slice(1);
		if (slice.IsEmpty)
			return;

		var i = 0;
		foreach (var item in slice)
		{
			between();
			action(item, ++i);
		}
	}

	/// <inheritdoc cref="Base64.IsValid(ReadOnlySpan{char})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Base64"/>.IsValid(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsBase64([NotNullWhen(false)] this ReadOnlySpan<char> @this)
		=> Base64.IsValid(@this);

	public static string Join(this ReadOnlySpan<char> @this, IEnumerable<string> values)
	{
		if (values?.Any() is not true)
			return new(@this);

		var totalLength = @this.Length * (values.Count() - 1) + values.Select(value => value.Length).Sum();
		Span<char> result = stackalloc char[totalLength];

		var offset = 0;
		foreach (var value in values)
		{
			if (offset > 0)
			{
				@this.CopyTo(result.Slice(offset, @this.Length));
				offset += @this.Length;
			}

			var span = value.AsSpan();
			span.CopyTo(result.Slice(offset, span.Length));
			offset += span.Length;
		}

		return new(result);
	}

	public static string Join(this ReadOnlySpan<char> @this, string[] values)
	{
		if (values?.Any() is not true)
			return new(@this);

		var totalLength = @this.Length * (values.Length - 1) + values.Select(value => value.Length).Sum();
		Span<char> result = stackalloc char[totalLength];

		var offset = 0;
		var count = values.Length;
		for (var i = 0; i < count; ++i)
		{
			if (offset > 0)
			{
				@this.CopyTo(result.Slice(offset, @this.Length));
				offset += @this.Length;
			}

			var span = values[i].AsSpan();
			span.CopyTo(result.Slice(offset, span.Length));
			offset += span.Length;
		}

		return new(result);
	}

	/// <inheritdoc cref="ReadOnlySpan{T}.Slice(int, int)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Slice(0, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ReadOnlySpan<char> Left(this ReadOnlySpan<char> @this, int length)
		=> @this.Slice(0, length);

	/// <summary>
	/// Mask letter or numbers in a string.
	/// </summary>
	public static string Mask(this ReadOnlySpan<char> @this, char mask = '*', string[]? terms = null, StringComparison comparison = StringComparison.Ordinal)
	{
		if (@this.IsEmpty)
			return string.Empty;

		Span<char> span = stackalloc char[@this.Length];
		@this.CopyTo(span);

		if (terms?.Length > 0)
		{
			foreach (var term in terms)
			{
NextTerm:
				var index = span.AsReadOnly().IndexOf(term, comparison);
				if (index > -1)
				{
					var slice = span.Slice(index, term.Length);
					slice.Fill(mask);
					goto NextTerm;
				}
			}
		}
		else
		{
			for (var i = 0; i < span.Length; ++i)
				if (span[i].IsLetterOrDigit())
					span[i] = mask;
		}

		return new(span);
	}

	/// <summary>
	/// Mask letter or numbers in a string.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string MaskIgnoreCase(this ReadOnlySpan<char> @this, char mask = '*', string[]? terms = null)
		=> @this.Mask(mask, terms, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Parse<T>(this ReadOnlySpan<char> @this, IFormatProvider? formatProvider)
		where T : ISpanParsable<T>
		=> T.Parse(@this, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Parse<T>(this ReadOnlySpan<char> @this, NumberStyles style, IFormatProvider? formatProvider = null)
		where T : INumberBase<T>
		=> T.Parse(@this, style, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="MemoryMarshal.Read{T}(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Read<T>(this ReadOnlySpan<byte> @this)
		where T : struct
		=> MemoryMarshal.Read<T>(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool StartsWithIgnoreCase(this ReadOnlySpan<char> @this, string text)
		=> @this.StartsWith(text, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.ToBase64String(@<paramref name="this"/>, <paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToBase64(this ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> Convert.ToBase64String(@this, options);

	public static char[] ToBase64Chars(this ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
	{
		Span<char> chars = stackalloc char[@this.Length * sizeof(int)];
		return Convert.TryToBase64Chars(@this, chars, out var length, options) ? chars.Slice(0, length).ToArray() : [];
	}

	/// <inheritdoc cref="BitConverter.ToBoolean(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToBoolean(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ToBoolean(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToBoolean(@this);

	/// <inheritdoc cref="Enum.TryParse{TEnum}(ReadOnlySpan{char}, out TEnum)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>.TryParse(@<paramref name="this"/>, <see langword="out"/> <typeparamref name="T"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
	/// </remarks>
	[DebuggerHidden]
	public static T? ToEnum<T>(this ReadOnlySpan<char> @this)
		where T : struct, Enum
		=> Enum.TryParse<T>(@this, out var result) ? (T?)result : null;

	/// <inheritdoc cref="Enum.TryParse{TEnum}(ReadOnlySpan{char}, bool, out TEnum)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>.TryParse(@<paramref name="this"/>, <paramref name="ignoreCase"/>, <see langword="out"/> <typeparamref name="T"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
	/// </remarks>
	[DebuggerHidden]
	public static T? ToEnumIgnoreCase<T>(this ReadOnlySpan<char> @this)
		where T : struct, Enum
		=> Enum.TryParse<T>(@this, true, out var result) ? (T?)result : null;

	/// <inheritdoc cref="MemoryMarshal.ToEnumerable{T}(ReadOnlyMemory{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.ToEnumerable(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> @this)
		where T : struct
		=> MemoryMarshal.ToEnumerable(@this);

	/// <inheritdoc cref="Convert.ToHexString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.ToHexString(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToHexString(this ReadOnlySpan<byte> @this)
		=> Convert.ToHexString(@this);

	public static T ToNumber<T>(this ReadOnlySpan<byte> @this)
		where T : struct, INumber<T>
		=> typeof(T).GetScalarType() switch
		{
			ScalarType.Char => Unsafe.BitCast<char, T>(BitConverter.ToChar(@this)),
			ScalarType.SByte => Unsafe.BitCast<sbyte, T>((sbyte)BitConverter.ToInt32(@this)),
			ScalarType.Int16 => Unsafe.BitCast<short, T>(BitConverter.ToInt16(@this)),
			ScalarType.Int32 => Unsafe.BitCast<int, T>(BitConverter.ToInt32(@this)),
			ScalarType.IntPtr => Unsafe.BitCast<nint, T>(BitConverter.ToInt32(@this)),
			ScalarType.Int64 => Unsafe.BitCast<long, T>(BitConverter.ToInt64(@this)),
			ScalarType.BigInteger => Unsafe.BitCast<BigInteger, T>(new BigInteger(@this)),
			ScalarType.Byte => Unsafe.BitCast<byte, T>((byte)BitConverter.ToUInt32(@this)),
			ScalarType.UInt16 => Unsafe.BitCast<ushort, T>(BitConverter.ToUInt16(@this)),
			ScalarType.UInt32 => Unsafe.BitCast<uint, T>(BitConverter.ToUInt32(@this)),
			ScalarType.UIntPtr => Unsafe.BitCast<nuint, T>(BitConverter.ToUInt32(@this)),
			ScalarType.UInt64 => Unsafe.BitCast<ulong, T>(BitConverter.ToUInt64(@this)),
			ScalarType.Half => Unsafe.BitCast<Half, T>(BitConverter.ToHalf(@this)),
			ScalarType.Single => Unsafe.BitCast<float, T>(BitConverter.ToSingle(@this)),
			ScalarType.Double => Unsafe.BitCast<double, T>(BitConverter.ToDouble(@this)),
			ScalarType.Decimal => Unsafe.BitCast<decimal, T>(new decimal(@this.Cast<byte, int>())),
			var scalarType => throw new UnreachableException(Invariant($"Cannot convert bytes to {scalarType.Name()}."))
		};

	/// <inheritdoc cref="BitConverter.ToString(byte[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.ToArray());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this ReadOnlySpan<byte> @this)
		=> BitConverter.ToString(@this.ToArray());

	/// <inheritdoc cref="BitConverter.ToString(byte[], int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.ToArray(), <paramref name="startIndex"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this ReadOnlySpan<byte> @this, int startIndex)
		=> BitConverter.ToString(@this.ToArray(), startIndex);

	/// <inheritdoc cref="BitConverter.ToString(byte[], int, int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="BitConverter"/>.ToString(@<paramref name="this"/>.Span, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this ReadOnlySpan<byte> @this, int startIndex, int length)
		=> BitConverter.ToString(@this.ToArray(), startIndex, length);

	/// <inheritdoc cref="Convert.TryFromBase64Chars(ReadOnlySpan{char}, Span{byte}, out int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.TryFromBase64Chars(@<paramref name="this"/>, <paramref name="bytes"/>, <see langword="out"/> <paramref name="bytesWritten"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryFromBase64(this ReadOnlySpan<char> @this, Span<byte> bytes, out int bytesWritten)
		=> Convert.TryFromBase64Chars(@this, bytes, out bytesWritten);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>(this ReadOnlySpan<char> @this, [MaybeNullWhen(false)] out T value)
		where T : ISpanParsable<T>
		=> T.TryParse(@this, InvariantCulture, out value);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>(this ReadOnlySpan<char> @this, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : ISpanParsable<T>
		=> T.TryParse(@this, formatProvider ?? InvariantCulture, out value);

	/// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, System.Globalization.NumberStyles, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>(this ReadOnlySpan<char> @this, NumberStyles style, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : INumberBase<T>
		=> T.TryParse(@this, style, formatProvider ?? InvariantCulture, out value);
}
