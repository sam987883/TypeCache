// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	internal class DefaultRuleHandler<T, R> : IRuleHandler<T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRuleHandler(IServiceProvider serviceProvider)
			=> this._ServiceProvider = serviceProvider;

		public async ValueTask<Response<R>> HandleAsync(T request, CancellationToken cancellationToken)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
					return new Response<R>(validationRules);
			}

			var rule = this._ServiceProvider.GetRequiredService<IRule<T, R>>();
			return new Response<R>(await rule.ApplyAsync(request, cancellationToken));
		}
	}

	internal class DefaultRuleHandler<M, T, R> : IRuleHandler<M, T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<R>> HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
					return new Response<R>(validationRules);
			}

			var rule = this._ServiceProvider.GetRequiredService<IRule<M, T, R>>();
			return new Response<R>(await rule.ApplyAsync(metadata, request, cancellationToken));
		}
	}
}
