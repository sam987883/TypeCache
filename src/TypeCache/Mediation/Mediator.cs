// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class Mediator(IServiceProvider serviceProvider, ILogger<IMediator> logger)
	: IMediator
{
	private readonly IServiceProvider _ServiceProvider = serviceProvider ?? serviceProvider.ThrowArgumentNullException();

	public async Task Execute<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest
	{
		serviceProvider.AssertNotNull();

		await using var scope = this._ServiceProvider.CreateAsyncScope();

		this.Validate(request, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST>>();
		var afterRules = scope.ServiceProvider.GetService<IEnumerable<IAfterRule<REQUEST>>>();

		try
		{
			await rule.Execute(request, token);
			if (afterRules?.Any() is true)
				Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", nameof(Mediator), nameof(Mediator.Execute));
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.Execute), error.Message);
			await Task.FromException(error);
		}
	}

	public async Task<RESPONSE> Map<REQUEST, RESPONSE>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest<RESPONSE>
	{
		serviceProvider.AssertNotNull();

		await using var scope = this._ServiceProvider.CreateAsyncScope();

		this.Validate(request, token);

		var rule = scope.ServiceProvider.GetRequiredService<IRule<REQUEST, RESPONSE>>();
		var afterRules = scope.ServiceProvider.GetService<IEnumerable<IAfterRule<REQUEST, RESPONSE>>>();

		try
		{
			var response = await rule.Map(request, token);
			if (afterRules?.Any() is true)
				Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, response, token)).ToArray(), token);

			return response;
		}
		catch (AggregateException error)
		{
			logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", nameof(Mediator), nameof(Mediator.Map));
			await Task.FromException(error.InnerExceptions.Count == 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.Map), error.Message);
			await Task.FromException(error);
		}

		return default!;
	}

	public Task<RESPONSE> Map<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default)
		=> (Task<RESPONSE>)this.GetType().InvokeMethod(nameof(Mediator.Map), [request.GetType(), typeof(RESPONSE)], this, request, token)!;

	public void Validate<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		serviceProvider.AssertNotNull();

		using var scope = this._ServiceProvider.CreateScope();

		var validationRules = scope.ServiceProvider.GetService<IEnumerable<IValidationRule<REQUEST>>>();
		if (validationRules?.Any() is not true)
			return;

		try
		{
			Task.WaitAll(validationRules!.Select(validationRule => validationRule.Validate(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			logger?.LogAggregateException(error, "{mediator}.{function} aggregate failure.", nameof(Mediator), nameof(Mediator.Validate));
			if (error.InnerExceptions.Count == 1)
				throw error.InnerException!;

			throw;
		}
		catch (Exception error)
		{
			logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.Validate), error.Message);
			throw;
		}
	}
}
