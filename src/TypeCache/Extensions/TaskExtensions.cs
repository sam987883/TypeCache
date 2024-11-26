// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class TaskExtensions
{
	public static async Task<T> Cast<T>(this Task task)
	{
		task.ThrowIfNull();
		task.GetType().Is(typeof(Task<>)).ThrowIfFalse();

		await task;

		object result = task.GetType().GetProperty(nameof(Task<T>.Result))!.GetValueEx(task)!;
		return (T)result;
	}
}
