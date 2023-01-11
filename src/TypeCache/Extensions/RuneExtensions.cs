// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Text;

namespace TypeCache.Extensions;

public static class RuneExtensions
{
	/// <inheritdoc cref="Rune.GetUnicodeCategory(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.GetUnicodeCategory(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static UnicodeCategory GetUnicodeCategory(this Rune @this)
		=> Rune.GetUnicodeCategory(@this);

	/// <inheritdoc cref="Rune.IsControl(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsControl(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsControl(this Rune @this)
		=> Rune.IsControl(@this);

	/// <inheritdoc cref="Rune.IsDigit(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsDigit(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsDigit(this Rune @this)
		=> Rune.IsDigit(@this);

	/// <inheritdoc cref="Rune.IsLetter(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsLetter(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsLetter(this Rune @this)
		=> Rune.IsLetter(@this);

	/// <inheritdoc cref="Rune.IsLetterOrDigit(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsLetterOrDigit(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsLetterOrDigit(this Rune @this)
		=> Rune.IsLetterOrDigit(@this);

	/// <inheritdoc cref="Rune.IsLower(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsLower(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsLower(this Rune @this)
		=> Rune.IsLower(@this);

	/// <inheritdoc cref="Rune.IsNumber(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsNumber(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsNumber(this Rune @this)
		=> Rune.IsNumber(@this);

	/// <inheritdoc cref="Rune.IsPunctuation(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsPunctuation(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsPunctuation(this Rune @this)
		=> Rune.IsPunctuation(@this);

	/// <inheritdoc cref="Rune.IsSeparator(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsSeparator(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsSeparator(this Rune @this)
		=> Rune.IsSeparator(@this);

	/// <inheritdoc cref="Rune.IsSymbol(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsSymbol(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsSymbol(this Rune @this)
		=> Rune.IsSymbol(@this);

	/// <inheritdoc cref="Rune.IsUpper(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsUpper(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsUpper(this Rune @this)
		=> Rune.IsUpper(@this);

	/// <inheritdoc cref="Rune.IsWhiteSpace(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.IsWhiteSpace(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsWhiteSpace(this Rune @this)
		=> Rune.IsWhiteSpace(@this);

	/// <inheritdoc cref="Rune.ToLower(Rune, CultureInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.ToLower(@<paramref name="this"/>, <paramref name="culture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Rune ToLower(this Rune @this, CultureInfo culture)
		=> Rune.ToLower(@this, culture);

	/// <inheritdoc cref="Rune.ToLowerInvariant(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.ToLowerInvariant(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Rune ToLowerInvariant(this Rune @this)
		=> Rune.ToLowerInvariant(@this);

	/// <inheritdoc cref="Rune.GetNumericValue(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.GetNumericValue(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double ToNumber(this Rune @this)
		=> Rune.GetNumericValue(@this);

	/// <inheritdoc cref="Rune.ToUpper(Rune, CultureInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.ToUpper(@<paramref name="this"/>, <paramref name="culture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Rune ToUpper(this Rune @this, CultureInfo culture)
		=> Rune.ToUpper(@this, culture);

	/// <inheritdoc cref="Rune.ToUpperInvariant(Rune)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Rune"/>.ToUpperInvariant(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Rune ToUpperInvariant(this Rune @this)
		=> Rune.ToUpperInvariant(@this);
}
