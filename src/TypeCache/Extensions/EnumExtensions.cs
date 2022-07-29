﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// <c>=&gt; <see cref="EnumOf{T}.Member"/>[@<paramref name="this"/>]?.Hex ?? @<paramref name="this"/>.ToString("X");</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Member[@this]?.Hex ?? @this.ToString("X");

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
