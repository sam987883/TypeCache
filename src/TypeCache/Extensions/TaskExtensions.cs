// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class TaskExtensions
{
	public static async Task<T> Cast<T>(this Task task)
	{
		task.ThrowIfNull();
		task.GetType().Is(typeof(Task<>)).ThrowIfFalse();

		await task;

		object result = task.GetType().Properties()[nameof(Task<T>.Result)].GetValue(task)!;
		return (T)result;
	}
}
