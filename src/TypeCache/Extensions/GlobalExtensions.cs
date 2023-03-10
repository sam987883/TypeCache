﻿// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Extensions;

public static class GlobalExtensions
{
	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>, <paramref name="value"/>) = (<paramref name="value"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void Swap<T>(this ref T @this, ref T value)
		where T : struct
		=> (@this, value) = (value, @this);

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>.Item2, @<paramref name="this"/>.Item1);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static (T, T) Swap<T>(this (T, T) @this)
		=> (@this.Item2, @this.Item1);

	/// <inheritdoc cref="Expression.Constant(object?, Type)"/>
	public static ConstantExpression ToConstantExpression<T>(this T? @this)
		=> @this is not null ? Expression.Constant(@this, @this.GetType()) : Expression.Constant(null);

	/// <inheritdoc cref="ToWeakReference{T}(T)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new "/> <see cref="WeakReference{T}"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	public static WeakReference<T> ToWeakReference<T>(this T @this)
		where T : class
		=> new WeakReference<T>(@this);
}
