// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Text;

namespace TypeCache.Extensions;

public static class RuneExtensions
{
	extension(Rune @this)
	{
		/// <inheritdoc cref="Rune.GetUnicodeCategory(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.GetUnicodeCategory(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public UnicodeCategory GetUnicodeCategory()
			=> Rune.GetUnicodeCategory(@this);

		/// <inheritdoc cref="Rune.IsControl(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsControl(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsControl()
			=> Rune.IsControl(@this);

		/// <inheritdoc cref="Rune.IsDigit(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsDigit(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsDigit()
			=> Rune.IsDigit(@this);

		/// <inheritdoc cref="Rune.IsLetter(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsLetter(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsLetter()
			=> Rune.IsLetter(@this);

		/// <inheritdoc cref="Rune.IsLetterOrDigit(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsLetterOrDigit(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsLetterOrDigit()
			=> Rune.IsLetterOrDigit(@this);

		/// <inheritdoc cref="Rune.IsLower(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsLower(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsLower()
			=> Rune.IsLower(@this);

		/// <inheritdoc cref="Rune.IsNumber(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsNumber(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsNumber()
			=> Rune.IsNumber(@this);

		/// <inheritdoc cref="Rune.IsPunctuation(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsPunctuation(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsPunctuation()
			=> Rune.IsPunctuation(@this);

		/// <inheritdoc cref="Rune.IsSeparator(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsSeparator(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsSeparator()
			=> Rune.IsSeparator(@this);

		/// <inheritdoc cref="Rune.IsSymbol(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsSymbol(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsSymbol()
			=> Rune.IsSymbol(@this);

		/// <inheritdoc cref="Rune.IsUpper(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsUpper(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsUpper()
			=> Rune.IsUpper(@this);

		/// <inheritdoc cref="Rune.IsWhiteSpace(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.IsWhiteSpace(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsWhiteSpace()
			=> Rune.IsWhiteSpace(@this);

		/// <inheritdoc cref="Rune.ToLower(Rune, CultureInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.ToLower(@this, <paramref name="culture"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Rune ToLower(CultureInfo culture)
			=> Rune.ToLower(@this, culture);

		/// <inheritdoc cref="Rune.ToLowerInvariant(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.ToLowerInvariant(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Rune ToLowerInvariant()
			=> Rune.ToLowerInvariant(@this);

		/// <inheritdoc cref="Rune.GetNumericValue(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.GetNumericValue(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public double ToNumber()
			=> Rune.GetNumericValue(@this);

		/// <inheritdoc cref="Rune.ToUpper(Rune, CultureInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.ToUpper(@this, <paramref name="culture"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Rune ToUpper(CultureInfo culture)
			=> Rune.ToUpper(@this, culture);

		/// <inheritdoc cref="Rune.ToUpperInvariant(Rune)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Rune"/>.ToUpperInvariant(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Rune ToUpperInvariant()
			=> Rune.ToUpperInvariant(@this);
	}
}
