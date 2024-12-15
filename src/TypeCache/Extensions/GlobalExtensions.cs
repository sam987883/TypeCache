// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class GlobalExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> &gt;= <paramref name="minimum"/> &amp;&amp; @<paramref name="this"/> &lt;= <paramref name="maximum"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Between<T>(this T @this, T minimum, T maximum)
		where T : IComparisonOperators<T, T, bool>
		=> @this >= minimum && @this <= maximum;

	/// <remarks>
	/// <c>=&gt; <see cref="ValueBox{T}"/>.GetValue(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object Box<T>(this T @this)
		where T : struct
		=> ValueBox<T>.GetValue(@this);

	/// <inheritdoc cref="Expression.Constant(object?, Type)"/>
	public static ConstantExpression ConstantExpression<T>(this T? @this)
		=> @this is not null ? Expression.Constant(@this, @this.GetType()) : Expression.Constant(null);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> &gt; <paramref name="minimum"/> &amp;&amp; @<paramref name="this"/> &lt; <paramref name="maximum"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool InBetween<T>(this T @this, T minimum, T maximum)
		where T : IComparisonOperators<T, T, bool>
		=> @this > minimum && @this < maximum;

	public static IEnumerable<T> Repeat<T>(this T @this, int count)
	{
		while (--count > -1)
			yield return @this;
	}

	/// <inheritdoc cref="StrongBox{T}.StrongBox(T)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StrongBox<T> StrongBox<T>(this T @this)
		where T : struct
		=> new(@this);

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

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual<T>(this T @this, T value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
		where T : unmanaged, IEqualityOperators<T, T, bool>
	{
		if (@this == value)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1!, argument2!).ToString(),
				actualValue: (@this.ToString(), value.ToString()),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfEqual)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqual<T>(this T @this, T value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
		where T : unmanaged, IEqualityOperators<T, T, bool>
	{
		if (@this != value)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1!, argument2!).ToString(),
				actualValue: (@this.ToString(), value.ToString()),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfNotEqual)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <inheritdoc cref="WeakReference{T}(T)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static WeakReference<T> WeakReference<T>(this T @this)
		where T : class
		=> new(@this);
}
