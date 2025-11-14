// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class Mediator(IServiceProvider serviceProvider, ILogger<IMediator>? logger = null)
	: IMediator
{
	internal readonly struct Sender<RESPONSE>(Mediator mediator, ILogger<IMediator>? logger, object? key)
		: ISend<RESPONSE>
	{
		internal ValueTask<RESPONSE> InvokeSend(IRequest<RESPONSE> request, CancellationToken token)
			=> (ValueTask<RESPONSE>)this.GetType().Methods[nameof(Send)]
				.Find([request.GetType(), typeof(RESPONSE)], (request, token))!
				.Invoke(this, (request, token))!;

		public async ValueTask<RESPONSE> Send<REQUEST>(REQUEST request, CancellationToken token = default)
			where REQUEST : notnull
		{
			mediator.Validate(request);

			var rule = mediator.GetRule<REQUEST, RESPONSE>(key);
			var afterRules = mediator.GetAfterRules<REQUEST>();
			var ruleName = rule.GetType().CodeName;

			try
			{
				logger?.LogInformation("START: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
				var response = await rule.Send(request, token);
				if (afterRules.Any())
					Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, token)), token);

				return response;
			}
			catch (AggregateException error)
			{
				logger?.LogAggregateException(error, "ERROR: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
				if (error.InnerExceptions.Count is 1)
					throw error.InnerException!;

				throw;
			}
			catch (Exception error)
			{
				logger?.LogError(error, "ERROR: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
				throw;
			}
			finally
			{
				logger?.LogInformation("END: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
			}
		}
	}

	public Task Dispatch<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		this.Validate(request);

		return this._Dispatch(null, request, token);
	}

	public Task Dispatch<REQUEST>(object key, REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		this.Validate(key, request);

		return this._Dispatch(key, request, token);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ISend<RESPONSE> Request<RESPONSE>(object? key = null)
		=> new Sender<RESPONSE>(this, logger, key);

	public ValueTask<RESPONSE> Send<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default)
	{
		this.Validate(request);

		var sender = new Sender<RESPONSE>(this, logger, null);
		return sender.InvokeSend(request, token);
	}

	public ValueTask<RESPONSE> Send<RESPONSE>(object key, IRequest<RESPONSE> request, CancellationToken token = default)
	{
		this.Validate(key, request);

		var sender = new Sender<RESPONSE>(this, logger, key);
		return sender.InvokeSend(request, token);
	}

	public void Validate<REQUEST>(REQUEST request)
		where REQUEST : notnull
	{
		request.ThrowIfNull();

		this._Validate(null, request);
	}

	public void Validate<REQUEST>(object key, REQUEST request)
		where REQUEST : notnull
	{
		key.ThrowIfNull();
		request.ThrowIfNull();

		this._Validate(key, request);
	}

	public Task _Dispatch<REQUEST>(object? key, REQUEST request, CancellationToken token = default)
		where REQUEST : notnull
	{
		var rule = this.GetRule<REQUEST>(key);
		var afterRules = this.GetAfterRules<REQUEST>(key);
		var ruleName = rule.GetType().CodeName;

		try
		{
			logger?.LogInformation("START: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
			if (afterRules.Any())
				return rule.Execute(request, token).ContinueWith(_ => Task.WaitAll(afterRules.Select(afterRule => afterRule.Handle(request, token)), token), token);

			return rule.Execute(request, token);
		}
		catch (AggregateException error)
		{
			logger?.LogAggregateException(error, "ERROR: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
			return Task.FromException(error.InnerExceptions.Count is 1 ? error.InnerException! : error);
		}
		catch (Exception error)
		{
			logger?.LogError(error, "ERROR: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
			return Task.FromException(error);
		}
		finally
		{
			logger?.LogInformation("END: {Mediator} rule {Rule}", [nameof(Mediator), ruleName]);
		}
	}

	private IEnumerable<IAfterRule<REQUEST>> GetAfterRules<REQUEST>(object? key = null)
		where REQUEST : notnull
	{
		var afterRules = serviceProvider.GetServices<IAfterRule<REQUEST>>();
		if (key is not null)
			afterRules = afterRules.Concat(serviceProvider.GetKeyedServices<IAfterRule<REQUEST>>(key));

		return afterRules;
	}

	private IRule<REQUEST> GetRule<REQUEST>(object? key = null)
		where REQUEST : notnull
		=> key is not null
			? serviceProvider.GetRequiredKeyedService<IRule<REQUEST>>(key)
			: serviceProvider.GetRequiredService<IRule<REQUEST>>();

	private IRule<REQUEST, RESPONSE> GetRule<REQUEST, RESPONSE>(object? key = null)
		where REQUEST : notnull
		=> key is not null
			? serviceProvider.GetRequiredKeyedService<IRule<REQUEST, RESPONSE>>(key)
			: serviceProvider.GetRequiredService<IRule<REQUEST, RESPONSE>>();

	private IEnumerable<IValidationRule<REQUEST>> GetValidationRules<REQUEST>(object? key = null)
		where REQUEST : notnull
	{
		var validationRules = serviceProvider.GetServices<IValidationRule<REQUEST>>();
		if (key is not null)
			validationRules = validationRules.Concat(serviceProvider.GetKeyedServices<IValidationRule<REQUEST>>(key));

		return validationRules;
	}

	private void _Validate<REQUEST>(object? key, REQUEST request)
		where REQUEST : notnull
	{
		var validationRules = this.GetValidationRules<REQUEST>(key);
		if (!validationRules.Any())
			return;

		var requestName = request.GetType().CodeName;

		try
		{
			logger?.LogInformation("START: {Mediator} validate {Request}", [nameof(Mediator), requestName]);
			Task.WaitAll(validationRules.Select(_ => Task.Run(() => _.Validate(request))));
		}
		catch (AggregateException error)
		{
			logger?.LogAggregateException(error, "ERROR: {Mediator} validate {Request}", [nameof(Mediator), requestName]);
			if (error.InnerExceptions.Count is 1)
				throw error.InnerException!;

			throw;
		}
		catch (Exception error)
		{
			logger?.LogError(error, "ERROR: {Mediator} validate {Request}", [nameof(Mediator), requestName]);
			throw;
		}
		finally
		{
			logger?.LogInformation("END: {Mediator} validate {Request}", [nameof(Mediator), requestName]);
		}
	}
}
