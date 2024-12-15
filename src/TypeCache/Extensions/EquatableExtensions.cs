// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class EquatableExtensions
{
	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual<T>(this IEquatable<T> @this, IEquatable<T> value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
		where T : notnull
	{
		if ((@this?.Equals(value) ?? (value is null)) is true)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1!, argument2!).ToString(),
				actualValue: (@this, value),
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
	public static void ThrowIfNotEqual<T>(this IEquatable<T> @this, IEquatable<T> value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
		where T : notnull
	{
		if ((@this?.Equals(value) ?? (value is null)) is not true)
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: (argument1!, argument2!).ToString(),
				actualValue: (@this, value),
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfNotEqual)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}
}
