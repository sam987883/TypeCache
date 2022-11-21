// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class EnumExtensions
{
	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Member"/>[@<paramref name="this"/>]?.Attributes ?? Array&lt;Attribute&gt;.Empty;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IReadOnlyList<Attribute> Attributes<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Member[@this]?.Attributes ?? Array<Attribute>.Empty;

	/// <summary>
	/// <c>=&gt; <paramref name="flags"/>.Any(flag =&gt; @<paramref name="this"/>.HasFlag(flag));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool HasAnyFlag<T>(this T @this, params T[] flags)
		where T : struct, Enum
		=> flags.Any(flag => @this.HasFlag(flag));

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Member"/>[@<paramref name="this"/>]?.Hex ?? @<paramref name="this"/>.ToString("X");</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Member[@this]?.Hex ?? @this.ToString("X");

	/// <summary>
	/// <c>=&gt; <paramref name="tokens"/>.Any(token =&gt; <see cref="EnumOf{T}.Comparer"/>.EqualTo(@<paramref name="this"/>, token));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsAny<T>(this T @this, params T[] tokens)
		where T : struct, Enum
		=> tokens.Any(token => EnumOf<T>.Comparer.EqualTo(@this, token));

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}"/>.IsDefined(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsDefined<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.IsDefined(@this);

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}"/>.Parse(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Name<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Parse(@this);

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Member"/>[@<paramref name="this"/>]?.Number ?? @<paramref name="this"/>.ToString("D");</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Number<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Member[@this]?.Number ?? @this.ToString("D");

	/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static StringComparer ToStringComparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);
}
