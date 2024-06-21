// Copyright (c) 2021 Samuel Abraham

using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using TypeCache.Utilities;
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

	/// <inheritdoc cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ReadOnlySpan<R> Cast<T, R>(this ReadOnlySpan<T> @this)
		where T : struct
		where R : struct
		=> MemoryMarshal.Cast<T, R>(@this);

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T> action)
	{
		action.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			action(@this[i]);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T, int> action)
	{
		action.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		var count = @this.Length;
		for (var i = 0; i < count; ++i)
			action(@this[i], i);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T> action, Action between)
	{
		action.ThrowIfNull();
		between.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		action(@this[0]);

		var count = @this.Length;
		for (var i = 1; i < count; ++i)
		{
			between();
			action(@this[i]);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, Action<T, int> action, Action between)
	{
		action.ThrowIfNull();
		between.ThrowIfNull();

		if (@this.IsEmpty)
			return;

		action(@this[0], 0);

		var count = @this.Length;
		for (var i = 1; i < count; ++i)
		{
			between();
			action(@this[i], i);
		}
	}

	public static string Join(this scoped ReadOnlySpan<char> @this, IEnumerable<string> values)
	{
		if (!values.Any())
			return new(@this);

		var totalLength = (int)values.Select(value => value.Length).Sum() + @this.Length * (values.Count() - 1);
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

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Join((<see cref="IEnumerable{T}"/>)<paramref name="values"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Join(this scoped ReadOnlySpan<char> @this, string[] values)
		=> @this.Join((IEnumerable<string>)values);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>[0] == <paramref name="text"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Left(this scoped ReadOnlySpan<char> @this, char text)
		=> @this[0] == text;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Slice(0, <paramref name="length"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ReadOnlySpan<char> Left(this ReadOnlySpan<char> @this, int length)
		=> @this.Slice(0, length);

	private static string Mask(this scoped ReadOnlySpan<char> @this, char mask, string[]? terms, StringComparison comparison)
	{
		if (@this.IsEmpty)
			return new(@this);

		terms ??= Array<string>.Empty;

		Span<char> span = stackalloc char[@this.Length];
		@this.CopyTo(span);

		var i = -1;
		if (terms.Length > 0)
		{
			var count = 0;
			while (++i < span.Length)
			{
				foreach (var term in terms)
				{
					if (term.Length > count && ((ReadOnlySpan<char>)span[i..]).StartsWith(term.AsSpan(), comparison))
						count = term.Length;
				}

				if (count > 0)
				{
					--count;
					if (span[i].IsLetterOrDigit())
						span[i] = mask;
				}
			}
		}
		else
		{
			while (++i < span.Length)
				if (span[i].IsLetterOrDigit())
					span[i] = mask;
		}

		return new(span);
	}

	/// <summary>
	/// Mask letter or numbers in a string.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Mask(this scoped ReadOnlySpan<char> @this, char mask = '*', string[]? terms = null)
		=> @this.Mask(mask, terms, StringComparison.Ordinal);

	/// <summary>
	/// Mask letter or numbers in a string.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string MaskIgnoreCase(this scoped ReadOnlySpan<char> @this, char mask = '*', string[]? terms = null)
		=> @this.Mask(mask, terms, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Parse<T>(this scoped ReadOnlySpan<char> @this, IFormatProvider? formatProvider)
		where T : ISpanParsable<T>
		=> T.Parse(@this, formatProvider ?? InvariantCulture);

	/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Parse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Parse<T>(this scoped ReadOnlySpan<char> @this, NumberStyles style, IFormatProvider? formatProvider = null)
		where T : INumberBase<T>
		=> T.Parse(@this, style, formatProvider ?? InvariantCulture);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.StartsWith(<paramref name="text"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool StartsWithIgnoreCase(this scoped ReadOnlySpan<char> @this, string text)
		=> @this.StartsWith(text, StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc cref="MemoryMarshal.Read{T}(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T To<T>(this scoped ReadOnlySpan<byte> @this)
		where T : struct
		=> MemoryMarshal.Read<T>(@this);

	/// <inheritdoc cref="Convert.ToBase64String(ReadOnlySpan{byte}, Base64FormattingOptions)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Convert"/>.ToBase64String(@<paramref name="this"/>, <paramref name="options"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToBase64(this scoped ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
		=> Convert.ToBase64String(@this, options);

	public static char[] ToBase64Chars(this scoped ReadOnlySpan<byte> @this, Base64FormattingOptions options = Base64FormattingOptions.None)
	{
		Span<char> chars = stackalloc char[@this.Length * sizeof(int)];
		return Convert.TryToBase64Chars(@this, chars, out var length, options)
			? chars.Slice(0, length).ToArray()
			: Array<char>.Empty;
	}

	/// <inheritdoc cref="Enum.TryParse(Type, ReadOnlySpan{char}, bool, out object?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>.TryParse(@<paramref name="this"/>, <paramref name="ignoreCase"/>, <see langword="out"/> <typeparamref name="T"/> result) ? (<typeparamref name="T"/>?)result : <see langword="null"/>;</c>
	/// </remarks>
	[DebuggerHidden]
	public static T? ToEnum<T>(this ReadOnlySpan<char> @this, bool ignoreCase = true)
		where T : struct, Enum
		=> Enum.TryParse(@this, ignoreCase, out T result) ? (T?)result : null;

	/// <inheritdoc cref="MemoryMarshal.ToEnumerable{T}(ReadOnlyMemory{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MemoryMarshal"/>.ToEnumerable(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> @this)
		where T : struct
		=> MemoryMarshal.ToEnumerable(@this);

	public static string ToHexString(this scoped ReadOnlySpan<byte> @this)
	{
		const string HEX_CHARS = "0123456789ABCDEF";

		Span<char> chars = stackalloc char[@this.Length * sizeof(char)];
		for (var i = 0; i < @this.Length; ++i)
		{
			var c = i * 2;
			chars[c] = HEX_CHARS[(@this[i] & 0xf0) >> 4];
			chars[c + 1] = HEX_CHARS[@this[i] & 0x0f];
		}
		return new string(chars);
	}

	/// <inheritdoc cref="MemoryMarshal.AsRef{T}(ReadOnlySpan{byte})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="ref"/> <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ref readonly T ToRef<T>(this ReadOnlySpan<byte> @this)
		where T : struct
		=> ref MemoryMarshal.AsRef<T>(@this);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <see cref="InvariantCulture"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>(this scoped ReadOnlySpan<char> @this, [MaybeNullWhen(false)] out T value)
		where T : ISpanParsable<T>
		=> T.TryParse(@this, InvariantCulture, out value);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>(this scoped ReadOnlySpan<char> @this, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : ISpanParsable<T>
		=> T.TryParse(@this, formatProvider ?? InvariantCulture, out value);

	/// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, System.Globalization.NumberStyles, IFormatProvider?, out TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.TryParse(@<paramref name="this"/>, <paramref name="style"/>, <paramref name="formatProvider"/>, <see langword="out"/> <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryParse<T>(this scoped ReadOnlySpan<char> @this, NumberStyles style, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
		where T : INumberBase<T>
		=> T.TryParse(@this, style, formatProvider ?? InvariantCulture, out value);
}
