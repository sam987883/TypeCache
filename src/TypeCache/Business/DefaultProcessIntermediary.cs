// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business;

internal sealed class DefaultProcessIntermediary<REQUEST> : IProcessIntermediary<REQUEST>
{
	private readonly IProcess<REQUEST> _Process;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;

	public DefaultProcessIntermediary(IProcess<REQUEST> process, IEnumerable<IValidationRule<REQUEST>> validationRules)
	{
		this._Process = process;
		this._ValidationRules = validationRules;
	}

	public async ValueTask RunAsync(REQUEST request, Action onComplete, CancellationToken token)
	{
		try
		{
			var validationMessages = this._ValidationRules.SelectMany(_ => _.Validate(request)).ToArray();
			if (validationMessages.Any())
				await ValueTask.FromException(new ValidationException(validationMessages));
			else
				this._Process.PublishAsync(request, token).GetAwaiter().OnCompleted(onComplete);
		}
		catch (Exception error)
		{
			await ValueTask.FromException(error);
		}
	}
}
