// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static partial class EnumExtensions
{
	[DebuggerHidden]
	public static Attribute[] Attributes<T>(this T @this)
		where T : struct, Enum
		=> Enum<T>.IsDefined(@this)
			? typeof(T).GetField(@this.Name(), Public | Static)!.GetCustomAttributes(false).Cast<Attribute>().ToArray()
			: Array<Attribute>.Empty;

	[DebuggerHidden]
	public static bool HasAnyFlag<T>(this T @this, T[] flags)
		where T : struct, Enum
		=> flags?.Any(flag => @this.HasFlag(flag)) is true;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("X");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Hex(this Enum @this)
		=> @this.ToString("X");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("X");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> @this.ToString("X");

	/// <remarks>
	/// <c>=&gt; <see cref="Enum{T}"/>.IsDefined(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsDefined<T>(this T @this)
		where T : struct, Enum
		=> Enum<T>.IsDefined(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("F");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Name(this Enum @this)
		=> @this.ToString("F");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("F");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Name<T>(this T @this)
		where T : struct, Enum
		=> @this.ToString("F");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Number(this Enum @this)
		=> @this.ToString("D");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Number<T>(this T @this)
		where T : struct, Enum
		=> @this.ToString("D");

	/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringComparer ToComparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);
}
