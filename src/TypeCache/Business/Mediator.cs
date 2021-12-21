// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Business;

internal class Mediator : IMediator
{
	private readonly IServiceProvider _ServiceProvider;

	public Mediator(IServiceProvider serviceProvider)
	{
		this._ServiceProvider = serviceProvider;
	}

	public async ValueTask<O> ApplyRulesAsync<I, O>(I request, CancellationToken cancellationToken = default)
	{
		var ruleHandler = this._ServiceProvider.GetService<IRuleIntermediary<I, O>>()
			?? this._ServiceProvider.GetRequiredService<DefaultRuleIntermediary<I, O>>();
		return await ruleHandler.HandleAsync(request, cancellationToken);
	}

	public async ValueTask RunProcessAsync<I>(I request, CancellationToken cancellationToken = default)
	{
		var processHandler = this._ServiceProvider.GetService<IProcessIntermediary<I>>()
			?? this._ServiceProvider.GetRequiredService<DefaultProcessIntermediary<I>>();
		await processHandler.HandleAsync(request, cancellationToken);
	}
}
