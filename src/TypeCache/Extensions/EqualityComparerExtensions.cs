// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class EqualityComparerExtensions
{
	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual<T>(this IEqualityComparer<T> @this, T? x, T? y, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? comparerArgument = null,
		[CallerArgumentExpression("x")] string? argument1 = null,
		[CallerArgumentExpression("y")] string? argument2 = null)
		where T : notnull
	{
		@this.ThrowIfNull();
		if (@this.Equals(x, y))
			Throw(caller!, (argument1!, argument2!), (x, y), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqual<T>(this IEqualityComparer<T> @this, T? x, T? y, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("x")] string? argument1 = null,
		[CallerArgumentExpression("y")] string? argument2 = null)
		where T : notnull
	{
		@this.ThrowIfNull();
		if (!@this.Equals(x, y))
			Throw(caller!, (argument1!, argument2!), (x, y), message, logger);
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
}
