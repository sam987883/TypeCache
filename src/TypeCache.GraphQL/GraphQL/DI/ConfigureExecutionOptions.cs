﻿using System.Threading.Tasks;

namespace GraphQL.DI; // TODO: think about namespaces!

internal sealed class ConfigureExecutionOptions : IConfigureExecution
{
	private readonly Func<ExecutionOptions, Task> _action;

	public ConfigureExecutionOptions(Func<ExecutionOptions, Task> action)
	{
		_action = action;
	}

	public ConfigureExecutionOptions(Action<ExecutionOptions> action)
	{
		_action = opt => { action(opt); return Task.CompletedTask; };
	}

	public Task<ExecutionResult> ExecuteAsync(ExecutionOptions options, ExecutionDelegate next)
	{
		var task = _action(options);
		if (task.IsCompleted && task.Status is TaskStatus.RanToCompletion)
			return next(options);

		return Continuer(task, options, next);

		static async Task<ExecutionResult> Continuer(Task task, ExecutionOptions options, ExecutionDelegate next)
		{
			await task;
			return await next(options);
		}
	}

	public float SortOrder { get; } = 100F;
}
