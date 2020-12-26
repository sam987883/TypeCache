// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	internal class Mediator : IMediator
	{
		private readonly IServiceProvider _ServiceProvider;

		public Mediator(IServiceProvider serviceProvider)
			=> this._ServiceProvider = serviceProvider;

		public async ValueTask<Response<R>> ApplyRule<T, R>(T request, Func<T, R> rule, CancellationToken cancellationToken = default)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
					return new Response<R>(validationRules);
			}

			return new Response<R>(rule(request));
		}

		public async ValueTask<Response<R>> ApplyRuleAsync<T, R>(T request, CancellationToken cancellationToken = default)
		{
			var ruleHandler = this._ServiceProvider.GetService<IRuleHandler<T, R>>()
				?? this._ServiceProvider.GetRequiredService<DefaultRuleHandler<T, R>>();
			return await ruleHandler.HandleAsync(request, cancellationToken);
		}

		public async ValueTask<Response<R>> ApplyRuleAsync<M, T, R>(M metadata, T request, CancellationToken cancellationToken = default)
		{
			var ruleHandler = this._ServiceProvider.GetService<IRuleHandler<M, T, R>>()
				?? this._ServiceProvider.GetRequiredService<DefaultRuleHandler<M, T, R>>();
			return await ruleHandler.HandleAsync(metadata, request, cancellationToken);
		}

		public async ValueTask<Response<R[]>> ApplyRulesAsync<T, R>(T request, CancellationToken cancellationToken = default)
		{
			var rulesHandler = this._ServiceProvider.GetService<IRulesHandler<T, R>>()
				?? this._ServiceProvider.GetRequiredService<DefaultRulesHandler<T, R>>();
			return await rulesHandler.HandleAsync(request, cancellationToken);
		}

		public async ValueTask<Response<R[]>> ApplyRulesAsync<M, T, R>(M metadata, T request, CancellationToken cancellationToken = default)
		{
			var rulesHandler = this._ServiceProvider.GetService<IRulesHandler<M, T, R>>()
				?? this._ServiceProvider.GetRequiredService<DefaultRulesHandler<M, T, R>>();
			return await rulesHandler.HandleAsync(metadata, request, cancellationToken);
		}

		public async ValueTask RunProcessAsync<T>(T request, CancellationToken cancellationToken = default)
		{
			var processHandler = this._ServiceProvider.GetService<IProcessHandler<T>>()
				?? this._ServiceProvider.GetRequiredService<DefaultProcessHandler<T>>();
			await processHandler.HandleAsync(request, cancellationToken);
		}

		public async ValueTask RunProcessAsync<M, T>(M metadata, T request, CancellationToken cancellationToken = default)
		{
			var processHandler = this._ServiceProvider.GetService<IProcessHandler<M, T>>()
				?? this._ServiceProvider.GetRequiredService<DefaultProcessHandler<M, T>>();
			await processHandler.HandleAsync(metadata, request, cancellationToken);
		}
	}
}
