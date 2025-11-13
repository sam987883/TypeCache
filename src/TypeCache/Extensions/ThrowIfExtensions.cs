// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TypeCache.Reflection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ThrowIfExtensions
{
	extension(object? @this)
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfEqual(object? value, Func<string>? message = null, ILogger? logger = null,
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
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfNot<T>(Func<string>? message = null, ILogger? logger = null,
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
		public void ThrowIfNotEqual(object? value, Func<string>? message = null, ILogger? logger = null,
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
		public void ThrowIfReferenceEqual(object? value, Func<string>? message = null, ILogger? logger = null,
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
		public void ThrowIfNotReferenceEqual(object? value, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		{
			if (!object.ReferenceEquals(@this, value))
				Throw(caller!, (expression1, expression2), (@this, value), message, logger);
		}
	}

	extension([NotNull] object? @this)
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfNull(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (@this is null)
				Throw(caller!, expression, @this, message, logger);
		}
	}

	extension([NotNull] string? @this)
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfBlank(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (@this.IsBlank)
				Throw(caller!, expression, @this, message, logger);
		}
	}

	extension(bool @this)
	{
		/// <param name="message">Pass in a custom message or omit to use a default message.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfFalse(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (!@this)
				Throw(caller!, expression, @this, message, logger);
		}

		/// <param name="message">Pass in a custom message or omit to use a default message.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfTrue(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (@this)
				Throw(caller!, expression, @this, message, logger);
		}
	}

	extension(bool? @this)
	{
		/// <param name="message">Pass in a custom message or omit to use a default message.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfNotTrue(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (@this is not true)
				Throw(caller!, expression, @this, message, logger);
		}

		/// <param name="message">Pass in a custom message or omit to use a default message.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfTrue(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (@this is true)
				Throw(caller!, expression, @this, message, logger);
		}
	}

	extension<T>(T? @this) where T : unmanaged, IEqualityOperators<T, T, bool>
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfEqual(T value, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		{
			if (@this == value)
				Throw(caller!, (expression1, expression2), (@this, value), message, logger);
		}

		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfNotEqual(T value, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		{
			if (@this != value)
				Throw(caller!, (expression1, expression2), (@this, value), message, logger);
		}
	}

	extension<T>([NotNull] IEnumerable<T> @this) where T : notnull
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfEmpty(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (!@this.Any())
				Throw(caller!, expression, @this, message, logger);
		}
	}

	extension<T>([NotNull] T[] @this) where T : notnull
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfEmpty(Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression = null)
		{
			if (@this.Length is 0)
				Throw(caller!, expression, @this, message, logger);
		}
	}

	extension<T>([NotNull] T @this) where T : struct, Enum
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfEnumIs(T value, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression(nameof(value))] string? expression2 = null)
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
		public void ThrowIfEnumIsNot(T value, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		{
			var comparer = new EnumComparer<T>();
			if (!comparer.Equals(@this, value))
				Throw(caller!, (expression1, expression2), (@this, value), message, logger);
		}
	}

	extension<T>(IEquatable<T> @this) where T : notnull
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression1">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="expression2">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfEqual(IEquatable<T> value, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression(nameof(value))] string? expression2 = null)
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
		public void ThrowIfNotEqual(IEquatable<T> value, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression(nameof(value))] string? expression2 = null)
		{
			if ((@this?.Equals(value) ?? (value is null)) is not true)
				Throw(caller!, (expression1, expression2), (@this, value), message, logger);
		}
	}

	extension<T>(IEqualityComparer<T> @this) where T : notnull
	{
		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfEqual(T? x, T? y, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression("x")] string? argument1 = null,
			[CallerArgumentExpression("y")] string? argument2 = null)
		{
			@this.ThrowIfNull();
			if (@this.Equals(x, y))
				Throw(caller!, (expression1!, Invariant($"{argument1 ?? "null"}, {argument2 ?? "null"}")), (x, y), message, logger);
		}

		/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
		/// <param name="logger">Pass a logger to log exception if thrown.</param>
		/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public void ThrowIfNotEqual(T? x, T? y, Func<string>? message = null, ILogger? logger = null,
			[CallerMemberName] string? caller = null,
			[CallerArgumentExpression(nameof(@this))] string? expression1 = null,
			[CallerArgumentExpression("x")] string? argument1 = null,
			[CallerArgumentExpression("y")] string? argument2 = null)
		{
			@this.ThrowIfNull();
			if (!@this.Equals(x, y))
				Throw(caller!, (expression1!, Invariant($"{argument1 ?? "null"}, {argument2 ?? "null"}")), (x, y), message, logger);
		}
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
