// Copyright (c) 2021 Samuel Abraham

using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class Mediator
	: IMediator
{
	private readonly IServiceProvider _ServiceProvider;
	private readonly ILogger<IMediator>? _Logger;

	public Mediator(IServiceProvider serviceProvider, ILogger<IMediator>? logger = null)
	{
		this._ServiceProvider = serviceProvider;
		this._Logger = logger;
	}

	public async Task Execute<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest
	{
		await using var scope = this._ServiceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST>>();
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST>>();

		await this.ExecuteRules(request, rule, afterRules, token);
	}

	public Task<RESPONSE> Map<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default)
		=> (Task<RESPONSE>)this.GetType().InvokeMethod(nameof(Mediator._Map), new[] { request.GetType(), typeof(RESPONSE) }, this, new object[] { request, token })!;

	public void Validate<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		using var scope = this._ServiceProvider.CreateScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);
	}

	private void ExecuteRules<REQUEST>(
		REQUEST request
		, IEnumerable<IValidationRule<REQUEST>> rules
		, CancellationToken token)
		where REQUEST : notnull
	{
		try
		{
			Task.WaitAll(rules.Select(rule => rule.Validate(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			this._Logger?.LogAggregateException(error, "{Mediator} executing {Count} validation rules", new object[] { nameof(Mediator), rules.Count() });
			if (error.InnerExceptions.Count == 1)
				throw error.InnerException!;

			throw;
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{Mediator} executing {Count} validation rules", nameof(Mediator), rules.Count());
			throw;
		}
	}

	private async Task ExecuteRules<REQUEST>(
		REQUEST request
		, IRule<REQUEST> rule
		, IEnumerable<IAfterRule<REQUEST>> afterRules
		, CancellationToken token)
		where REQUEST : IRequest
	{
		try
		{
			this._Logger?.LogInformation("{Mediator} executing rule: {Rule}", nameof(Mediator), rule.GetType().Name);
			await rule.Execute(request, token);
			if (afterRules?.Any() is true)
				Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			this._Logger?.LogAggregateException(error, "{Mediator} executing rule: {Rule} - FAIL", new object[] { nameof(Mediator), rule.GetType().Name });
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{Mediator} executing rule: {Rule} - FAIL", nameof(Mediator), rule.GetType().Name);
			await Task.FromException(error);
		}
	}

	private async Task<RESPONSE> ExecuteRules<REQUEST, RESPONSE>(
		REQUEST request
		, IRule<REQUEST, RESPONSE> rule
		, IEnumerable<IAfterRule<REQUEST, RESPONSE>> afterRules
		, CancellationToken token)
		where REQUEST : IRequest<RESPONSE>
	{
		try
		{
			var response = await rule.Map(request, token);
			if (afterRules.Any())
				Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, response, token)).ToArray(), token);

			return response;
		}
		catch (AggregateException error)
		{
			this._Logger?.LogAggregateException(error, "{Mediator} executing rule: {Rule} - FAIL", new object[] { nameof(Mediator), rule.GetType().Name });
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{Mediator} executing rule: {Rule} - FAIL", nameof(Mediator), rule.GetType().Name);
			await Task.FromException(error);
		}

		return default!;
	}

	private async Task<RESPONSE> _Map<REQUEST, RESPONSE>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest<RESPONSE>
	{
		await using var scope = this._ServiceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST, RESPONSE>>();
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST, RESPONSE>>();

		return await this.ExecuteRules(request, rule, afterRules, token);
	}
}
