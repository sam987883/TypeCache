// Copyright (c) 2021 Samuel Abraham

using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ActionExtensions
{
	/// <summary>
	/// Retry a failed <see cref="Action"/>. The # of <c><paramref name="retryDelays"/></c> dictates the # of retry attempts.<br/>
	/// Some built-in interval sequences to use for retry delays:<br/>
	/// <list type="table">
	/// <item><c><see cref="Sequence.ExponentialSeconds()"/></c></item>
	/// <item><c><see cref="Sequence.ExponentialSeconds(uint)"/></c></item>
	/// <item><c><see cref="Sequence.LinearTime(TimeSpan)"/></c></item>
	/// </list>
	/// These are increasing infinite sequences, hence an infinite # of retries will be attempted.<br/>
	/// To limit the number of retries, call Linq's Take(...) method on the returned collection.
	/// </summary>
	public static async Task Retry(this Action @this, IEnumerable<TimeSpan> retryDelays, TimeProvider? timeProvider = default, CancellationToken token = default)
	{
		@this.ThrowIfNull();

		try
		{
			await Task.Run(@this, token);
		}
		catch (Exception lastError)
		{
			timeProvider ??= TimeProvider.System;

			foreach (var delay in retryDelays)
			{
				await Task.Delay(delay, timeProvider, token);
				try
				{
					await Task.Run(@this, token);
					return;
				}
				catch (Exception ex)
				{
					lastError = ex;
				}
			}

			await Task.FromException(lastError);
		}
	}
}
