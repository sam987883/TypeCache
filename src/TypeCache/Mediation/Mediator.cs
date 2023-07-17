// Copyright (c) 2021 Samuel Abraham

using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;
using TypeCache.Utilities;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		var scope = this._ServiceProvider.CreateAsyncScope();

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
			if (this._Logger is not null)
				error.InnerExceptions
					.AsArray()
					.ForEach(exception => this._Logger.LogError(exception, "{mediator}.{function} after rule failure: {message}", nameof(Mediator), nameof(Mediator.ExecuteAsync), exception.Message));

			throw error.InnerExceptions.Count == 1 ? error.InnerException! : error;
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.ExecuteAsync), error.Message);
			throw;
		}
	}

	public async Task<RESPONSE> MapAsync<REQUEST, RESPONSE>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest<RESPONSE>
	{
		var scope = this._ServiceProvider.CreateAsyncScope();

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
			if (this._Logger is not null)
				error.InnerExceptions
					.AsArray()
					.ForEach(exception => this._Logger.LogError(exception, "{mediator}.{function} after rule failure: {message}", nameof(Mediator), nameof(Mediator.MapAsync), exception.Message));

			throw error.InnerExceptions.Count == 1 ? error.InnerException! : error;
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.MapAsync), error.Message);
			throw;
		}
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

		var scope = this._ServiceProvider.CreateAsyncScope();
		var validationRules = scope.ServiceProvider.GetService<IEnumerable<IValidationRule<REQUEST>>>();
		if (validationRules?.Any() is not true)
			await Task.CompletedTask;

		try
		{
			Task.WaitAll(validationRules!.Select(validationRule => validationRule.ValidateAsync(request, token)).ToArray(), token);
		}
		catch (AggregateException error)
		{
			if (this._Logger is not null)
				error.InnerExceptions
					.AsArray()
					.ForEach(exception => this._Logger.LogError(exception, "{mediator}.{function} after rule failure: {message}", nameof(Mediator), nameof(Mediator.ValidateAsync), exception.Message));
			throw error.InnerExceptions.Count == 1 ? error.InnerException! : error;
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, "{mediator}.{function} failure: {message}", nameof(Mediator), nameof(Mediator.ValidateAsync), error.Message);
			throw;
		}
	}
}
