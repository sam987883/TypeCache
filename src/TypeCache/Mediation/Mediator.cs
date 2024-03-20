// Copyright (c) 2021 Samuel Abraham

using System.Data;
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

	public async Task Execute<REQUEST>(object? key, REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest
	{
		await using var scope = serviceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredKeyedService<IRule<REQUEST>>(key);
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IAfterRule<REQUEST>>(key));

		await this.ExecuteRules(request, rule, afterRules, token);
	}

	public Task<RESPONSE> Map<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default)
		=> (Task<RESPONSE>)this.GetType().InvokeMethod(nameof(Mediator._Map), [request.GetType(), typeof(RESPONSE)], this, [request, token])!;

	public Task<RESPONSE> Map<RESPONSE>(object? key, IRequest<RESPONSE> request, CancellationToken token = default)
		=> (Task<RESPONSE>)this.GetType().InvokeMethod(nameof(Mediator._Map), [request.GetType(), typeof(RESPONSE)], this, [key, request, token])!;

	public void Validate<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		using var scope = serviceProvider.CreateScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>();
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);
	}

	public void Validate<REQUEST>(object? key, REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		using var scope = serviceProvider.CreateScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IValidationRule<REQUEST>>(key));
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
			logger?.LogAggregateException(error, "{Mediator} executing {Count} validation rules", [nameof(Mediator), rules.Count()]);
			if (error.InnerExceptions.Count == 1)
				throw error.InnerException!;

			throw;
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{Mediator} executing {Count} validation rules", [nameof(Mediator), rules.Count()]);
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
			logger?.LogInformation("{Mediator} executing rule: {Rule}", [nameof(Mediator), rule.GetType().Name]);
			await rule.Execute(request, token);
			if (afterRules?.Any() is true)
				Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			logger?.LogAggregateException(error, "{Mediator} executing rule: {Rule} - FAIL", [nameof(Mediator), rule.GetType().Name]);
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{Mediator} executing rule: {Rule} - FAIL", [nameof(Mediator), rule.GetType().Name]);
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
			logger?.LogAggregateException(error, "{Mediator} executing rule: {Rule} - FAIL", [nameof(Mediator), rule.GetType().Name]);
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{Mediator} executing rule: {Rule} - FAIL", [nameof(Mediator), rule.GetType().Name]);
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

	private async Task<RESPONSE> _Map<REQUEST, RESPONSE>(object? key, REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest<RESPONSE>
	{
		await using var scope = serviceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetServices<IValidationRule<REQUEST>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IValidationRule<REQUEST>>(key));
		if (validationRules.Any())
			this.ExecuteRules(request, validationRules, token);

		var rule = scope.ServiceProvider.GetRequiredKeyedService<IRule<REQUEST, RESPONSE>>(key);
		var afterRules = scope.ServiceProvider.GetServices<IAfterRule<REQUEST, RESPONSE>>()
			.Concat(scope.ServiceProvider.GetKeyedServices<IAfterRule<REQUEST, RESPONSE>>(key));

		return await this.ExecuteRules(request, rule, afterRules, token);
	}
}
