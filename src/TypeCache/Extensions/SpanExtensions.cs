// Copyright (c) 2021 Samuel Abraham

using System.Runtime.InteropServices;

namespace TypeCache.Extensions;

public static class SpanExtensions
{
	extension<T>(Span<T> @this)
	{
		/// <remarks>
		/// <c>=&gt; (<see cref="ReadOnlySpan{T}"/>)@this;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ReadOnlySpan<T> AsReadOnly()
			=> (ReadOnlySpan<T>)@this;
	}

	extension<T>(Span<T> @this) where T : struct
	{
		/// <inheritdoc cref="MemoryMarshal.AsBytes{T}(Span{T})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.AsBytes(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Span<byte> AsBytes()
			=> MemoryMarshal.AsBytes(@this);


		/// <inheritdoc cref="MemoryMarshal.Cast{TFrom, TTo}(Span{TFrom})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.Cast&lt;<typeparamref name="T"/>, <typeparamref name="R"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Span<R> Cast<R>()
			where R : struct
			=> MemoryMarshal.Cast<T, R>(@this);
	}

	extension(Span<byte> @this)
	{
		/// <inheritdoc cref="MemoryMarshal.AsRef{T}(Span{byte})"/>
		/// <remarks>
		/// <c>=&gt; <see langword="ref"/> <see cref="MemoryMarshal"/>.AsRef&lt;<typeparamref name="T"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ref readonly T AsRef<T>()
			where T : struct
			=> ref MemoryMarshal.AsRef<T>(@this);

		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.Read&lt;<typeparamref name="T"/>&gt;(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T Read<T>()
			where T : struct
			=> MemoryMarshal.Read<T>(@this);

		/// <inheritdoc cref="MemoryMarshal.TryRead{T}(ReadOnlySpan{byte}, out T)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.TryRead(@this, <see langword="out"/> <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryRead<T>(out T value)
			where T : struct
			=> MemoryMarshal.TryRead(@this, out value);

		/// <inheritdoc cref="MemoryMarshal.TryWrite{T}(Span{byte}, in T)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.TryWrite(@this, <see langword="ref"/> <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryWrite<T>(in T value)
			where T : struct
			=> MemoryMarshal.TryWrite(@this, in value);

		/// <inheritdoc cref="MemoryMarshal.Write{T}(Span{byte}, in T)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="MemoryMarshal"/>.Write(@this, <see langword="ref"/> <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void Write<T>(in T value)
			where T : struct
			=> MemoryMarshal.Write(@this, in value);
	}

	extension(Span<char> @this)
	{
		/// <summary>
		/// Mask letter or numbers in a string.
		/// </summary>
		public void Mask(char mask = '*')
		{
			if (@this.IsEmpty)
				return;

			for (var i = 0; i < @this.Length; ++i)
				if (@this[i].IsLetterOrDigit())
					@this[i] = mask;
		}

		/// <summary>
		/// Mask letter or numbers within a string.
		/// </summary>
		public void Mask(char mask, string[] terms, StringComparison comparison)
		{
			if (@this.IsEmpty)
				return;

			foreach (var term in terms)
			{
				var index = @this.AsReadOnly().IndexOf(term, comparison);
				if (index is -1)
					continue;

				do
				{
					@this.Slice(index, term.Length).Fill(mask);
					index = @this.AsReadOnly().IndexOf(term, comparison);
				}
				while (index > -1);
			}
		}

		/// <summary>
		/// Mask the specified terms within a string.
		/// </summary>
		/// <remarks>
		/// <c>=&gt; @this.Mask(<paramref name="mask"/>, <paramref name="terms"/>, <see cref="StringComparison.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void MaskIgnoreCase(char mask, string[] terms)
			=> @this.Mask(mask, terms, StringComparison.OrdinalIgnoreCase);

		/// <summary>
		/// Mask the specified terms within a string.
		/// </summary>
		/// <remarks>
		/// <c>=&gt; @this.Mask(<paramref name="mask"/>, <paramref name="terms"/>, <see cref="StringComparison.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void MaskOrdinal(char mask, string[] terms)
			=> @this.Mask(mask, terms, StringComparison.Ordinal);

		public Span<char> Replace(char existing, char replacement)
		{
			for (var i = 0; i < @this.Length; ++i)
				if (@this[i] == existing)
					@this[i] = replacement;

			return @this;
		}
	}
}
