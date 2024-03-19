// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class Mediator(IServiceProvider serviceProvider, ILogger<IMediator>? logger = null)
	: IMediator
{
	public async Task Execute<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest
	{
		await using var scope = serviceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST>>();
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST>>();

		await this.ExecuteRules(request, rule, afterRules, token);
	}

	public async Task Execute<REQUEST>(string name, REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest
	{
		await using var scope = serviceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredKeyedService<IRule<REQUEST>>(name);
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IAfterRule<REQUEST>>(name));

		await this.ExecuteRules(request, rule, afterRules, token);
	}

	public Task<RESPONSE> Map<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default)
		=> (Task<RESPONSE>)this.GetType().InvokeMethod(nameof(Mediator._Map), [request.GetType(), typeof(RESPONSE)], this, [request, token])!;

	public Task<RESPONSE> Map<RESPONSE>(string name, IRequest<RESPONSE> request, CancellationToken token = default)
		=> (Task<RESPONSE>)this.GetType().InvokeMethod(nameof(Mediator._Map), [request.GetType(), typeof(RESPONSE)], this, [name, request, token])!;

	public void Validate<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		using var scope = serviceProvider.CreateScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);
	}

	public void Validate<REQUEST>(string name, REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		using var scope = serviceProvider.CreateScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IValidationRule<REQUEST>>(name));
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
			logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", [nameof(Mediator), nameof(Mediator.Validate)]);
			if (error.InnerExceptions.Count == 1)
				throw error.InnerException!;

			throw;
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{mediator}.{function} failure: {message}", [nameof(Mediator), nameof(Mediator.Validate), error.Message]);
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
			await rule.Execute(request, token);
			if (afterRules?.Any() is true)
				Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", [nameof(Mediator), nameof(Mediator.Execute)]);
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{mediator}.{function} failure: {message}", [nameof(Mediator), nameof(Mediator.Execute), error.Message]);
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
			logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", [nameof(Mediator), nameof(Mediator.Map)]);
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{mediator}.{function} failure: {message}", [nameof(Mediator), nameof(Mediator.Map), error.Message]);
			await Task.FromException(error);
		}

		return default!;
	}

	private async Task<RESPONSE> _Map<REQUEST, RESPONSE>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest<RESPONSE>
	{
		await using var scope = serviceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST, RESPONSE>>();
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST, RESPONSE>>();

		return await this.ExecuteRules(request, rule, afterRules, token);
	}

	private async Task<RESPONSE> _Map<REQUEST, RESPONSE>(string name, REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest<RESPONSE>
	{
		await using var scope = serviceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IValidationRule<REQUEST>>(name));
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredKeyedService<IRule<REQUEST, RESPONSE>>(name);
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST, RESPONSE>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IAfterRule<REQUEST, RESPONSE>>(name));

		return await this.ExecuteRules(request, rule, afterRules, token);
	}
}
