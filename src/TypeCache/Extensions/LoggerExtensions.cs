// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class LoggerExtensions
{
	/// <summary>
	/// Logs the <c><see cref="AggregateException"/></c> and each inner exception.
	/// </summary>
	public static void LogAggregateException<T>(this ILogger<T> @this, EventId eventId, AggregateException error, string message, params object?[] args)
	{
		@this.AssertNotNull();
		error.AssertNotNull();

		@this.LogError(eventId, error, message, args);

		if (error.InnerExceptions.Count > 1)
			error.InnerExceptions.AsArray().ForEach((exception, i) => @this.LogError(eventId, exception, "Aggregate InnerException #{position}: {message}", i + 1, exception.Message));
		else if (error.InnerException is not null)
			@this.LogError(eventId, error.InnerException, "Aggregate InnerException: {message}", error.InnerException.Message);
		else
			@this.LogError(eventId, error, "Aggregate Exception: {message}", error.Message);
	}

	/// <summary>
	/// Logs the <c><see cref="AggregateException"/></c> and each inner exception.
	/// </summary>
	public static void LogAggregateException<T>(this ILogger<T> @this, AggregateException error, string message, params object?[] args)
	{
		@this.AssertNotNull();
		error.AssertNotNull();

		@this.LogError(error, message, args);

		if (error.InnerExceptions.Count > 1)
			error.InnerExceptions.AsArray().ForEach((exception, i) => @this.LogError(exception, "Aggregate InnerException #{position}: {message}", i + 1, exception.Message));
		else if (error.InnerException is not null)
			@this.LogError(error.InnerException, "Aggregate InnerException: {message}", error.InnerException.Message);
		else
			@this.LogError(error, "Aggregate Exception: {message}", error.Message);
	}
}
