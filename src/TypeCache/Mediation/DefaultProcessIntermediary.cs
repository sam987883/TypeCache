// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Collections;

namespace TypeCache.Mediation;

internal sealed class DefaultProcessIntermediary<REQUEST> : IProcessIntermediary<REQUEST>
	where REQUEST : IRequest
{
	private readonly ILogger<DefaultProcessIntermediary<REQUEST>> _Logger;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;
	private readonly IProcess<REQUEST> _Process;
	private readonly IEnumerable<IAfterRule<REQUEST>> _AfterRules;

	public DefaultProcessIntermediary(
		ILogger<DefaultProcessIntermediary<REQUEST>> logger
		, IEnumerable<IValidationRule<REQUEST>> validationRules
		, IProcess<REQUEST> process
		, IEnumerable<IAfterRule<REQUEST>> afterRules)
	{
		this._Logger = logger;
		this._ValidationRules = validationRules;
		this._Process = process;
		this._AfterRules = afterRules ?? Array<IAfterRule<REQUEST>>.Empty;
	}

	public async ValueTask RunAsync(REQUEST request, Action onComplete, CancellationToken token)
	{
		var validationMessages = this._ValidationRules.SelectMany(_ => _.Validate(request)).ToArray();
		if (validationMessages.Any())
			await ValueTask.FromException(new ValidationException(validationMessages));

		this._Process.PublishAsync(request, token).GetAwaiter().OnCompleted(onComplete);

		if (this._AfterRules.Any())
		{
			try
			{
				Task.WaitAll(this._AfterRules.Select(rule => rule.ApplyAsync(request, token).AsTask()).ToArray(), token);
			}
			catch (Exception error)
			{
				if (this._Logger is not null)
					this._Logger.LogError(error, null);
			}
		}
	}
}
