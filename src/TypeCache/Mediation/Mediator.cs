// Copyright (c) 2021 Samuel Abraham

using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class Mediator : IMediator
{
	private readonly IServiceProvider _ServiceProvider;
	private readonly ILogger<IMediator> _Logger;

	public Mediator(IServiceProvider serviceProvider, ILogger<IMediator> logger)
	{
		this._ServiceProvider = serviceProvider;
		this._Logger = logger;
	}

	public async Task ExecuteAsync<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest
	{
		await using var scope = this._ServiceProvider.CreateAsyncScope();

		await this.ValidateAsync<REQUEST>(request, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST>>();
		var afterRules = scope.ServiceProvider.GetService<IEnumerable<IAfterRule<REQUEST>>>();

		try
		{
			await rule.ExecuteAsync(request, token);
			if (afterRules?.Any() is true)
				Task.WaitAll(afterRules.Select(afterRule => afterRule.HandleAsync(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			this._Logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", nameof(Mediator), nameof(Mediator.ExecuteAsync));
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.ExecuteAsync), error.Message);
			await Task.FromException(error);
		}
	}

	public async Task<RESPONSE> MapAsync<REQUEST, RESPONSE>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest<RESPONSE>
	{
		await using var scope = this._ServiceProvider.CreateAsyncScope();

		await this.ValidateAsync<REQUEST>(request, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST, RESPONSE>>();
		var afterRules = scope.ServiceProvider.GetService<IEnumerable<IAfterRule<REQUEST, RESPONSE>>>();

		try
		{
			var response = await rule.MapAsync(request, token);

			if (afterRules?.Any() is true)
				Task.WaitAll(afterRules.Select(afterRule => afterRule.HandleAsync(request, response, token)).ToArray(), token);

			return response;
		}
		catch (AggregateException error)
		{
			this._Logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", nameof(Mediator), nameof(Mediator.MapAsync));
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.MapAsync), error.Message);
			await Task.FromException(error);
		}

		return default!;
	}

	public Task<RESPONSE> MapAsync<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default)
	{
		request.AssertNotNull();

		return (Task<RESPONSE>)this.GetType().InvokeMethod(nameof(Mediator.MapAsync), new[] { request.GetType(), typeof(RESPONSE) }, this, request, token)!;
	}

	public async Task ValidateAsync<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		request.AssertNotNull();

		await using var scope = this._ServiceProvider.CreateAsyncScope();

		var validationRules = scope.ServiceProvider.GetService<IEnumerable<IValidationRule<REQUEST>>>();
		if (validationRules?.Any() is not true)
			await Task.CompletedTask;

		try
		{
			Task.WaitAll(validationRules!.Select(validationRule => validationRule.ValidateAsync(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			this._Logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", nameof(Mediator), nameof(Mediator.ValidateAsync));
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.ValidateAsync), error.Message);
			await Task.FromException(error);
		}
	}
}
