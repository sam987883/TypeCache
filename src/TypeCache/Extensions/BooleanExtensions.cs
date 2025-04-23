// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class BooleanExtensions
{
	[DebuggerHidden]
	public static bool Else(this bool @this, Action doIfFalse)
	{
		if (!@this)
			doIfFalse();

		return @this;
	}

	[DebuggerHidden]
	public static bool Then(this bool @this, Action doIfTrue)
	{
		if (@this)
			doIfTrue();

		return @this;
	}

	/// <param name="message">Pass in a custom message or omitt to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfFalse(this bool @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
	{
		if (!@this)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: argument,
				actualValue: @this.ToString(),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfFalse)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <param name="message">Pass in a custom message or omitt to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotTrue(this bool? @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
	{
		if (@this is not true)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: argument,
				actualValue: @this.ToString(),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfFalse)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <param name="message">Pass in a custom message or omitt to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfTrue(this bool @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
	{
		if (@this)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: argument,
				actualValue: @this.ToString(),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfFalse)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <param name="message">Pass in a custom message or omitt to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfTrue(this bool? @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
	{
		if (@this is true)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: argument,
				actualValue: @this.ToString(),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfFalse)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> ? [(<see cref="byte"/>)1] : [(<see cref="byte"/>)0];</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] ToBytes(this bool @this)
		=> @this ? [(byte)1] : [(byte)0];
}
