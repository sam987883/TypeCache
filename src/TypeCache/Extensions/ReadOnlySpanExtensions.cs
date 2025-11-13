// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using TypeCache.Reflection;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static class ReadOnlySpanExtensions
{
	extension<T>(ReadOnlySpan<T> @this)
	{
		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T> action)
		{
			action.ThrowIfNull();

			if (@this.IsEmpty)
				return;

			foreach (var item in @this)
				action(item);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T, int> action)
		{
			action.ThrowIfNull();

			if (@this.IsEmpty)
				return;

			var i = -1;
			foreach (var item in @this)
				action(item, ++i);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T> action, Action between)
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
		public void ForEach(Action<T, int> action, Action between)
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
	}

	extension<T>(ReadOnlySpan<T> @this) where T : struct
	{
		/// <inheritdoc cref="MemoryMarshal.AsBytes{T}(ReadOnlySpan{T})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes&lt;<typeparamref name="T"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ReadOnlySpan<byte> AsBytes()
			=> MemoryMarshal.AsBytes(@this);

		/// <inheritdoc cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ReadOnlySpan<R> Cast<R>()
			where R : struct
			=> MemoryMarshal.Cast<T, R>(@this);
	}

	extension(ReadOnlySpan<byte> @this)
	{
		/// <inheritdoc cref="MemoryMarshal.AsRef{T}(ReadOnlySpan{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see langword="ref"/> <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ref readonly T AsRef<T>()
			where T : struct
			=> ref MemoryMarshal.AsRef<T>(@this);

		/// <inheritdoc cref="MemoryMarshal.Read{T}(ReadOnlySpan{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Read<T>()
			where T : struct
			=> MemoryMarshal.Read<T>(@this);

		/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToBase64String(@this, <paramref name="options"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToBase64(Base64FormattingOptions options = Base64FormattingOptions.None)
			=> Convert.ToBase64String(@this, options);

		public char[] ToBase64Chars(Base64FormattingOptions options = Base64FormattingOptions.None)
		{
			Span<char> chars = stackalloc char[@this.Length * sizeof(int)];
			return Convert.TryToBase64Chars(@this, chars, out var length, options) ? chars.Slice(0, length).ToArray() : [];
		}

		/// <inheritdoc cref="Base64Url.EncodeToString(ReadOnlySpan{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Base64Url"/>.EncodeToString(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToBase64Url()
			=> Base64Url.EncodeToString(@this);

		/// <inheritdoc cref="Base64Url.EncodeToChars(ReadOnlySpan{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Base64Url"/>.EncodeToChars(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public char[] ToBase64UrlChars()
			=> Base64Url.EncodeToChars(@this);

		/// <inheritdoc cref="BitConverter.ToBoolean(ReadOnlySpan{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="BitConverter"/>.ToBoolean(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ToBoolean()
			=> BitConverter.ToBoolean(@this);

		/// <inheritdoc cref="Convert.ToHexString(byte[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToHexString(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToHexString()
			=> Convert.ToHexString(@this);

		/// <inheritdoc cref="Convert.ToHexStringLower(byte[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.ToHexStringLower(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToHexStringLower()
			=> Convert.ToHexStringLower(@this);

		public T ToNumber<T>()
			where T : struct, INumber<T>
			=> Type<T>.ScalarType switch
			{
				ScalarType.Char => Unsafe.BitCast<char, T>(BitConverter.ToChar(@this)),
				ScalarType.SByte => Unsafe.BitCast<sbyte, T>((sbyte)@this[0]),
				ScalarType.Int16 => Unsafe.BitCast<short, T>(BitConverter.ToInt16(@this)),
				ScalarType.Int32 => Unsafe.BitCast<int, T>(BitConverter.ToInt32(@this)),
				ScalarType.IntPtr => Unsafe.BitCast<nint, T>(BitConverter.ToInt32(@this)),
				ScalarType.Int64 => Unsafe.BitCast<long, T>(BitConverter.ToInt64(@this)),
				ScalarType.Int128 => Unsafe.BitCast<Int128, T>(BitConverter.ToInt128(@this)),
				ScalarType.BigInteger => Unsafe.BitCast<BigInteger, T>(new BigInteger(@this)),
				ScalarType.Byte => Unsafe.BitCast<byte, T>(@this[0]),
				ScalarType.UInt16 => Unsafe.BitCast<ushort, T>(BitConverter.ToUInt16(@this)),
				ScalarType.UInt32 => Unsafe.BitCast<uint, T>(BitConverter.ToUInt32(@this)),
				ScalarType.UIntPtr => Unsafe.BitCast<nuint, T>(BitConverter.ToUInt32(@this)),
				ScalarType.UInt64 => Unsafe.BitCast<ulong, T>(BitConverter.ToUInt64(@this)),
				ScalarType.UInt128 => Unsafe.BitCast<UInt128, T>(BitConverter.ToUInt128(@this)),
				ScalarType.Half => Unsafe.BitCast<Half, T>(BitConverter.ToHalf(@this)),
				ScalarType.Single => Unsafe.BitCast<float, T>(BitConverter.ToSingle(@this)),
				ScalarType.Double => Unsafe.BitCast<double, T>(BitConverter.ToDouble(@this)),
				ScalarType.Decimal => Unsafe.BitCast<decimal, T>(new decimal(@this.Cast<byte, int>())),
				var scalarType => throw new UnreachableException(Invariant($"Cannot convert bytes to {Type<T>.CodeName}."))
			};

		/// <inheritdoc cref="BitConverter.ToString(byte[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="BitConverter"/>.ToString(@this.ToArray());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText()
			=> BitConverter.ToString(@this.ToArray());

		/// <inheritdoc cref="BitConverter.ToString(byte[], int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="BitConverter"/>.ToString(@this.ToArray(), <paramref name="startIndex"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText(int startIndex)
			=> BitConverter.ToString(@this.ToArray(), startIndex);

		/// <inheritdoc cref="BitConverter.ToString(byte[], int, int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="BitConverter"/>.ToString(@this.Span, <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText(int startIndex, int length)
			=> BitConverter.ToString(@this.ToArray(), startIndex, length);
	}

	extension(ReadOnlySpan<char> @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.Equals(<paramref name="other"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool EqualsIgnoreCase(ReadOnlySpan<char> other)
			=> @this.Equals(other, StringComparison.OrdinalIgnoreCase);

		/// <param name="encoding">Defaults to <see cref="Encoding.UTF8"/></param>
		public string FromBase64Url(Encoding? encoding = null)
		{
			encoding ??= Encoding.UTF8;
			return encoding.GetString(Base64Url.DecodeFromChars(@this));
		}

		/// <inheritdoc cref="Base64.IsValid(ReadOnlySpan{char})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Base64"/>.IsValid(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsBase64()
			=> Base64.IsValid(@this);

		public string Join(IEnumerable<string> values)
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

		public string Join(string[] values)
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
		/// <c>=&gt; @this.Slice(0, <paramref name="length"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ReadOnlySpan<char> Left(int length)
			=> @this.Slice(0, length);

		/// <summary>
		/// Mask letter or numbers in a string.
		/// </summary>
		public string Mask(char mask = '*')
		{
			if (@this.IsEmpty)
				return string.Empty;

			Span<char> span = stackalloc char[@this.Length];
			@this.CopyTo(span);
			span.Mask(mask);
			return new(span);
		}

		/// <summary>
		/// Mask letter or numbers in a string.
		/// </summary>
		public string Mask(char mask, string[] terms, StringComparison comparison)
		{
			if (@this.IsEmpty)
				return string.Empty;

			Span<char> span = stackalloc char[@this.Length];
			@this.CopyTo(span);
			span.Mask(mask, terms, comparison);
			return new(span);
		}

		/// <summary>
		/// Mask <param name="terms"/> in a string.
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string MaskIgnoreCase(char mask, string[] terms)
		{
			if (@this.IsEmpty)
				return string.Empty;

			Span<char> span = stackalloc char[@this.Length];
			@this.CopyTo(span);
			span.MaskIgnoreCase(mask, terms);
			return new(span);
		}

		/// <summary>
		/// Mask <param name="terms"/> in a string.
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string MaskOrdinal(char mask, string[] terms)
		{
			if (@this.IsEmpty)
				return string.Empty;

			Span<char> span = stackalloc char[@this.Length];
			@this.CopyTo(span);
			span.MaskOrdinal(mask, terms);
			return new(span);
		}

		/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.Parse(@this, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Parse<T>(IFormatProvider? formatProvider)
			where T : ISpanParsable<T>
			=> T.Parse(@this, formatProvider ?? InvariantCulture);

		/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.Parse(@this, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Parse<T>(NumberStyles style, IFormatProvider? formatProvider = null)
			where T : INumberBase<T>
			=> T.Parse(@this, style, formatProvider ?? InvariantCulture);

		/// <remarks>
		/// <c>=&gt; @this.StartsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool StartsWithIgnoreCase(string text)
			=> @this.StartsWith(text, StringComparison.OrdinalIgnoreCase);

		/// <remarks>
		/// <c>=&gt; @this.StartsWith(<paramref name="text"/>, <see cref="StringComparison.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool StartsWithOrdinal(string text)
			=> @this.StartsWith(text, StringComparison.Ordinal);

		/// <inheritdoc cref="Enum.TryParse{TEnum}(ReadOnlySpan{char}, out TEnum)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Enum"/>.TryParse(@this, <see langword="out var"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
		/// </remarks>
		[DebuggerHidden]
		public T? ToEnum<T>()
			where T : struct, Enum
			=> Enum.TryParse<T>(@this, out var result) ? (T?)result : null;

		/// <inheritdoc cref="Enum.TryParse{TEnum}(ReadOnlySpan{char}, bool, out TEnum)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Enum"/>.TryParse(@this, <see langword="true"/>, <see langword="out var"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
		/// </remarks>
		[DebuggerHidden]
		public T? ToEnumIgnoreCase<T>()
			where T : struct, Enum
			=> Enum.TryParse<T>(@this, true, out var result) ? (T?)result : null;

		/// <summary>
		/// <code>
		/// "^2" ---&gt; ^2
		/// "1"  ---&gt; 1
		/// </code>
		/// </summary>
		public Index? ToIndex()
			=> @this switch
			{
				{ IsEmpty: true } => null,
				_ when @this.StartsWith('^') && int.TryParse(@this.Slice(1), out var index) => Index.FromEnd(index),
				_ when int.TryParse(@this, out var index) => Index.FromStart(index),
				_ => null
			};

		/// <inheritdoc cref="Convert.TryFromBase64Chars(ReadOnlySpan{char}, Span{byte}, out int)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Convert"/>.TryFromBase64Chars(@this, <paramref name="bytes"/>, <see langword="out"/> <paramref name="bytesWritten"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryFromBase64(Span<byte> bytes, out int bytesWritten)
			=> Convert.TryFromBase64Chars(@this, bytes, out bytesWritten);

		/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider, out TSelf)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.TryParse(@this, <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryParse<T>([MaybeNullWhen(false)] out T value)
			where T : ISpanParsable<T>
			=> T.TryParse(@this, InvariantCulture, out value);

		/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider, out TSelf)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.TryParse(@this, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryParse<T>(IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
			where T : ISpanParsable<T>
			=> T.TryParse(@this, formatProvider, out value);

		/// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out TSelf)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.TryParse(@this, <paramref name="style"/>, <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryParse<T>(NumberStyles style, [MaybeNullWhen(false)] out T value)
			where T : INumberBase<T>
			=> T.TryParse(@this, style, InvariantCulture, out value);

		/// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out TSelf)"/>
		/// <remarks>
		/// <c>=&gt; <typeparamref name="T"/>.TryParse(@this, <paramref name="style"/>, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryParse<T>(NumberStyles style, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
			where T : INumberBase<T>
			=> T.TryParse(@this, style, formatProvider, out value);
	}

	extension<T>(ReadOnlyMemory<T> @this) where T : struct
	{
		/// <inheritdoc cref="MemoryMarshal.ToEnumerable{T}(ReadOnlyMemory{T})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.ToEnumerable(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IEnumerable<T> ToEnumerable()
			=> MemoryMarshal.ToEnumerable(@this);
	}
}
