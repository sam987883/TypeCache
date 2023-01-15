// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class DefaultProcessIntermediary<REQUEST> : IProcessIntermediary<REQUEST>
	where REQUEST : IRequest
{
	private readonly ILogger<IProcessIntermediary<REQUEST>> _Logger;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;
	private readonly IProcess<REQUEST> _Process;
	private readonly IEnumerable<IAfterRule<REQUEST>> _AfterRules;

	public DefaultProcessIntermediary(
		ILogger<IProcessIntermediary<REQUEST>> logger
		, IEnumerable<IValidationRule<REQUEST>> validationRules
		, IProcess<REQUEST> process
		, IEnumerable<IAfterRule<REQUEST>> afterRules)
	{
		this._Logger = logger;
		this._ValidationRules = validationRules;
		this._Process = process;
		this._AfterRules = afterRules ?? Array<IAfterRule<REQUEST>>.Empty;
	}

	public ValueTask RunAsync(REQUEST request, Action onComplete, CancellationToken token)
	{
		var validationMessages = this._ValidationRules.SelectMany(_ => _.Validate(request)).ToArray();
		if (validationMessages.Any())
		{
			validationMessages.ForEach(message => this._Logger?.LogWarning(Invariant($"{this._Process.GetType().Name()} validation rule failure: {message}")));
			throw new ValidationException(validationMessages);
		}

		var task = this._Process.PublishAsync(request, token);
		task.GetAwaiter().OnCompleted(onComplete);

		if (this._AfterRules.Any())
		{
			try
			{
				Task.WaitAll(this._AfterRules.Select(rule => rule.ApplyAsync(request, token).AsTask()).ToArray(), token);
			}
			catch (Exception error)
			{
				this._Logger?.LogError(error, error.Message);
			}
		}

		return task;
	}
}
