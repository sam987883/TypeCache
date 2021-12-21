// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class EnumExtensions
{
	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Tokens"/>[@<paramref name="this"/>].Attributes;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IImmutableList<Attribute> Attributes<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens[@this].Attributes;

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Tokens"/>[@<paramref name="this"/>].Hex;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens[@this].Hex;

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Tokens"/>.Keys.Has(@<paramref name="this"/>, <see cref="EnumOf{T}.Comparer"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsValid<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.Keys.Has(@this, EnumOf<T>.Comparer);

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Tokens"/>[@<paramref name="this"/>].Name;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Name<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens[@this].Name;

	/// <summary>
	/// <c>=&gt; <see cref="EnumOf{T}.Tokens"/>[@<paramref name="this"/>].Number;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Number<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens[@this].Number;

	/// <summary>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static StringComparer ToStringComparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);
}
