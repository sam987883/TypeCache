// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class TaskExtensions
{
	public static async Task<T> Cast<T>(this Task task)
	{
		task.ThrowIfNull();
		task.GetType().Is(typeof(Task<>)).ThrowIfFalse();

		await task.ConfigureAwait(false);

		object result = task.GetType().GetProperty(nameof(Task<T>.Result))!.GetValueEx(task)!;
		return (T)result;
	}
}
