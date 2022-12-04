// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Business;

internal sealed class Mediator : IMediator
{
	private readonly IServiceProvider _ServiceProvider;

	public Mediator(IServiceProvider serviceProvider)
	{
		this._ServiceProvider = serviceProvider;
	}

	public async ValueTask ApplyRuleAsync<REQUEST>(REQUEST request, CancellationToken token = default)
	{
		var result = await this.GetRuleIntermediary<REQUEST>().GetAsync(request, token);
		switch (result.Status)
		{
			case RuleResponseStatus.Error:
				throw result.Error!;
			case RuleResponseStatus.FailValidation:
				throw new ValidationException(result.ValidationMessages);
		}
	}

	public async ValueTask<RESPONSE> ApplyRuleAsync<REQUEST, RESPONSE>(REQUEST request, CancellationToken token = default)
	{
		var result = await this.GetRuleIntermediary<REQUEST, RESPONSE>().GetAsync(request, token);
		return result.Status switch
		{
			RuleResponseStatus.Success => result.Response!,
			RuleResponseStatus.FailValidation => throw new ValidationException(result.ValidationMessages),
			_ => throw result.Error!
		};
	}

	[DebuggerHidden]
	public async ValueTask ApplyRuleAsync<REQUEST, RESPONSE>(REQUEST request, Action<RESPONSE> onSuccess, CancellationToken token = default)
		=> await this.ApplyRuleAsync(request, onSuccess, async error => await ValueTask.FromException(error), token);

	public async ValueTask<RuleResponseStatus> ApplyRuleAsync<REQUEST, RESPONSE>(REQUEST request, Action<RESPONSE> onSuccess, Action<Exception> onError, CancellationToken token = default)
	{
		var result = await this.GetRuleIntermediary<REQUEST, RESPONSE>().GetAsync(request, token);
		switch (result.Status)
		{
			case RuleResponseStatus.Success:
				onSuccess(result.Response!);
				break;
			case RuleResponseStatus.FailValidation:
				onError(new ValidationException(result.ValidationMessages));
				break;
			case RuleResponseStatus.Error:
				onError(result.Error!);
				break;
		}
		return result.Status;
	}

	public async ValueTask<RESULT> ApplyRuleAsync<REQUEST, RESPONSE, RESULT>(REQUEST request, Func<RESPONSE, RESULT> onSuccess, CancellationToken token = default)
	{
		var result = await this.GetRuleIntermediary<REQUEST, RESPONSE>().GetAsync(request, token);
		return result.Status switch
		{
			RuleResponseStatus.Success => onSuccess(result.Response!),
			RuleResponseStatus.FailValidation => throw new ValidationException(result.ValidationMessages),
			_ => throw result.Error!
		};
	}

	public async ValueTask<RESULT> ApplyRuleAsync<REQUEST, RESPONSE, RESULT>(REQUEST request, Func<RESPONSE, RESULT> onSuccess, Func<Exception, RESULT> onError, CancellationToken token = default)
	{
		var result = await this.GetRuleIntermediary<REQUEST, RESPONSE>().GetAsync(request, token);
		return result.Status switch
		{
			RuleResponseStatus.Success => onSuccess(result.Response!),
			RuleResponseStatus.FailValidation => onError(new ValidationException(result.ValidationMessages)),
			_ => onError(result.Error!)
		};
	}

	[DebuggerHidden]
	public async ValueTask RunProcessAsync<REQUEST>(REQUEST request, Action onComplete, CancellationToken token = default)
		=> await this.GetProcessIntermediary<REQUEST>().RunAsync(request, onComplete, token);

	private IProcessIntermediary<REQUEST> GetProcessIntermediary<REQUEST>()
		=> this._ServiceProvider.GetService<IProcessIntermediary<REQUEST>>()
			?? this._ServiceProvider.GetRequiredService<DefaultProcessIntermediary<REQUEST>>();

	private IRuleIntermediary<REQUEST> GetRuleIntermediary<REQUEST>()
		=> this._ServiceProvider.GetService<IRuleIntermediary<REQUEST>>()
			?? this._ServiceProvider.GetRequiredService<DefaultRuleIntermediary<REQUEST>>();

	private IRuleIntermediary<REQUEST, RESPONSE> GetRuleIntermediary<REQUEST, RESPONSE>()
		=> this._ServiceProvider.GetService<IRuleIntermediary<REQUEST, RESPONSE>>()
			?? this._ServiceProvider.GetRequiredService<DefaultRuleIntermediary<REQUEST, RESPONSE>>();
}
