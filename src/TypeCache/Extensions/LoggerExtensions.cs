// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Collections;

namespace TypeCache.Extensions;

public static class LoggerExtensions
{
	extension<T>(ILogger<T> @this)
	{
		/// <summary>
		/// Logs the <c><see cref="AggregateException"/></c> and each inner exception.
		/// </summary>
		public void LogAggregateException(EventId eventId, AggregateException error, string message, object?[]? args = null)
		{
			@this.ThrowIfNull();
			error.ThrowIfNull();

			args ??= Array<object?>.Empty;

			if (error.InnerExceptions.Count > 1)
				error.InnerExceptions.ForEach(exception => @this.LogError(eventId, exception, message, args));
			else if (error.InnerException is not null)
				@this.LogError(eventId, error.InnerException, message, args);
			else
				@this.LogError(eventId, error, message, args);
		}

		/// <summary>
		/// Logs the <c><see cref="AggregateException"/></c> and each inner exception.
		/// </summary>
		public void LogAggregateException(AggregateException error, string message, object?[]? args = null)
		{
			@this.ThrowIfNull();
			error.ThrowIfNull();

			args ??= Array<object?>.Empty;

			if (error.InnerExceptions.Count > 1)
				error.InnerExceptions.ForEach(exception => @this.LogError(exception, message, args));
			else if (error.InnerException is not null)
				@this.LogError(error.InnerException, message, args);
			else
				@this.LogError(error, message, args);
		}
	}
}
