// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class ObjectExtensions
{
	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual(this object? @this, object? value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
	{
		if (@this?.Equals(value) is true)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1, argument2).ToString(),
				actualValue: (@this, value),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfEqual)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNot<T>(this object @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
	{
		if (@this is not T)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: argument,
				actualValue: @this,
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfNot)}<{typeof(T).GetTypeName()}>"));

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
	public static void ThrowIfNotEqual(this object? @this, object? value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
	{
		if (@this?.Equals(value) is not true)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1, argument2).ToString(),
				actualValue: (@this, value),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfNotEqual)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentNullException"/>
	public static void ThrowIfNull([NotNull] this object? @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
	{
		if (@this is null)
		{
			var exception = new ArgumentNullException(
				paramName: argument,
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfNull)}"));

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
	public static void ThrowIfReferenceEqual(this object? @this, object? value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
	{
		if (object.ReferenceEquals(@this, value))
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1, argument2).ToString(),
				actualValue: (@this, value),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfReferenceEqual)}"));

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
	public static void ThrowIfNotReferenceEqual(this object? @this, object? value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
	{
		if (!object.ReferenceEquals(@this, value))
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1, argument2).ToString(),
				actualValue: (@this, value),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfNotReferenceEqual)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}
}
