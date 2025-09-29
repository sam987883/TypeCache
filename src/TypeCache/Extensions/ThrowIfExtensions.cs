// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TypeCache.Reflection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ThrowIfExtensions
{
	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfBlank([NotNull] this string? @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
	{
		if (@this.IsBlank())
			Throw(caller!, expression, @this, message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEmpty<T>([NotNull] this IEnumerable<T> @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
		where T : notnull
	{
		if (!@this.Any())
			Throw(caller!, expression, @this, message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEmpty<T>([NotNull] this T[] @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
		where T : notnull
	{
		if (@this.Length is 0)
			Throw(caller!, expression, @this, message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEnumIs<T>(this T @this, T value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		where T : struct, Enum
	{
		var comparer = new EnumComparer<T>();
		if (comparer.Equals(@this, value))
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEnumIsNot<T>(this T @this, T value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		where T : struct, Enum
	{
		var comparer = new EnumComparer<T>();
		if (!comparer.Equals(@this, value))
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual(this object? @this, object? value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
	{
		if (@this?.Equals(value) is true)
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual<T>(this IEquatable<T> @this, IEquatable<T> value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		where T : notnull
	{
		if ((@this?.Equals(value) ?? (value is null)) is true)
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual<T>(this T @this, T value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		where T : unmanaged, IEqualityOperators<T, T, bool>
	{
		if (@this == value)
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom message or omit to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfFalse(this bool @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
	{
		if (!@this)
			Throw(caller!, expression, @this, message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNot<T>(this object @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
	{
		if (@this is not T)
			Throw(caller!, expression, (@this, Invariant($"typeof({Type<T>.CodeName})")), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqual(this object? @this, object? value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
	{
		if (@this?.Equals(value) is not true)
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqual<T>(this IEquatable<T> @this, IEquatable<T> value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		where T : notnull
	{
		if ((@this?.Equals(value) ?? (value is null)) is not true)
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqual<T>(this T @this, T value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		where T : unmanaged, IEqualityOperators<T, T, bool>
	{
		if (@this != value)
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom message or omit to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotTrue(this bool? @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
	{
		if (@this is not true)
			Throw(caller!, expression, @this, message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNull([NotNull] this object? @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
	{
		if (@this is null)
			Throw(caller!, expression, @this, message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfReferenceEqual(this object? @this, object? value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
	{
		if (object.ReferenceEquals(@this, value))
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotReferenceEqual(this object? @this, object? value, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
		[CallerArgumentExpression(nameof(value))] string? expression2 = null)
	{
		if (!object.ReferenceEquals(@this, value))
			Throw(caller!, (expression1, expression2), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom message or omit to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfTrue(this bool @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
	{
		if (@this)
			Throw(caller!, expression, @this, message, logger);
	}

	/// <param name="message">Pass in a custom message or omit to use a default message.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfTrue(this bool? @this, Func<string>? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression(nameof(@this))] string? expression = null)
	{
		if (@this is true)
			Throw(caller!, expression, @this, message, logger);
	}

	[DoesNotReturn]
	private static void Throw(string caller, string? expression, object? value, Func<string>? message, ILogger? logger,
		[CallerMemberName] string? throwMethod = null)
	{
		message ??= () => Invariant($"{caller}: ({expression}).{throwMethod}()");
		var exception = new ArgumentOutOfRangeException(paramName: expression, actualValue: value, message: message());

		logger?.LogError(exception, "{Caller}: ({Expression}).{ThrowMethod}()",
			caller, expression, throwMethod);

		throw exception;
	}

	[DoesNotReturn]
	private static void Throw(string caller, (string?, string?) expression, (object?, object?) value, Func<string>? message, ILogger? logger,
		[CallerMemberName] string? throwMethod = null)
	{
		message ??= () => Invariant($"{caller}: ({expression.Item1}).{throwMethod}({expression.Item2})");
		var exception = new ArgumentOutOfRangeException(paramName: expression.ToString(), actualValue: value, message: message());

		logger?.LogError(exception, "{Caller}: ({Expression1}).{ThrowMethod}({Expression2})",
			caller, expression.Item1, throwMethod, expression.Item2);

		throw exception;
	}
}
