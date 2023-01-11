// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class Mediator : IMediator
{
	private readonly IServiceProvider _ServiceProvider;

	public Mediator(IServiceProvider serviceProvider)
	{
		this._ServiceProvider = serviceProvider;
	}

	public ValueTask ExecuteAsync(IRequest request, CancellationToken token = default)
	{
		request.AssertNotNull();

		var requestType = request.GetType();
		var ruleIntermediary = this._ServiceProvider.GetService(typeof(IRuleIntermediary<>).MakeGenericType(requestType))
			?? this._ServiceProvider.GetRequiredService(typeof(DefaultRuleIntermediary<>).MakeGenericType(requestType));

		var response = ruleIntermediary.GetType().InvokeMethod(nameof(IRuleIntermediary<IRequest>.HandleAsync), ruleIntermediary, request, token)!;
		response.AssertNotNull();

		return (ValueTask)response;
	}

	public ValueTask<RESPONSE> MapAsync<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default)
	{
		request.AssertNotNull();

		var requestType = request.GetType();
		var ruleIntermediary = this._ServiceProvider.GetService(typeof(IRuleIntermediary<,>).MakeGenericType(requestType, typeof(RESPONSE)))
			?? this._ServiceProvider.GetRequiredService(typeof(DefaultRuleIntermediary<,>).MakeGenericType(requestType, typeof(RESPONSE)));

		var response = ruleIntermediary.GetType().InvokeMethod(nameof(IRuleIntermediary<IRequest<RESPONSE>, RESPONSE>.HandleAsync), ruleIntermediary, request, token)!;
		response.AssertNotNull();

		return (ValueTask<RESPONSE>)response;
	}

	public ValueTask PublishAsync(IRequest request, Action onComplete, CancellationToken token = default)
	{
		request.AssertNotNull();

		var requestType = request.GetType();
		var processIntermediary = this._ServiceProvider.GetService(typeof(IProcessIntermediary<>).MakeGenericType(requestType))
			?? this._ServiceProvider.GetRequiredService(typeof(DefaultProcessIntermediary<>).MakeGenericType(requestType));

		var response = processIntermediary.GetType().InvokeMethod(nameof(IProcessIntermediary<IRequest>.RunAsync), processIntermediary, request, token)!;
		response.AssertNotNull();

		return (ValueTask)response;
	}

	public IEnumerable<string> Validate<REQUEST>(REQUEST request)
		where REQUEST : notnull
	{
		request.AssertNotNull();

		return this._ServiceProvider.GetServices<IValidationRule<REQUEST>>().SelectMany(_ => _.Validate(request));
	}
}
