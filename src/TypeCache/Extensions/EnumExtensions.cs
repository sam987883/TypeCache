// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;
using TypeCache.Utilities;
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
		where T : struct, Enum
	{
		if (Enum<T>.Equals(@this, value))
			Throw(caller!, (argument1!, argument2!), (@this, value), message, logger);
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
		where T : struct, Enum
	{
		if (!Enum<T>.Equals(@this, value))
			Throw(caller!, (argument1!, argument2!), (@this, value), message, logger);
	}

	private static void Throw(string method, (string, string) arguments, (object?, object?) items, string? message, ILogger? logger,
		[CallerMemberName] string? caller = null)
	{
		var exception = new ArgumentOutOfRangeException(
			paramName: arguments.ToString(),
			actualValue: items,
			message: message ?? Invariant($"{method}: {caller}"));

		logger?.LogError(exception, exception.Message);

		throw exception;
	}

	/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringComparer ToComparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);
}
