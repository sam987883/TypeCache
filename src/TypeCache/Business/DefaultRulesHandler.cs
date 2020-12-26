// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	internal class DefaultRulesHandler<T, R> : IRulesHandler<T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRulesHandler(IServiceProvider serviceProvider)
			=> this._ServiceProvider = serviceProvider;

		public async ValueTask<Response<R[]>> HandleAsync(T request, CancellationToken cancellationToken)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
					return new Response<R[]>(validationRules);
			}

			var rules = this._ServiceProvider.GetServices<IRule<T, R>>();
			return rules.Any()
				? new Response<R[]>(await rules.To(async rule => await rule.ApplyAsync(request, cancellationToken)).AllAsync())
				: Response<R[]>.Empty;
		}
	}

	internal class DefaultRulesHandler<M, T, R> : IRulesHandler<M, T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRulesHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<R[]>> HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
					return new Response<R[]>(validationRules);
			}

			var rules = this._ServiceProvider.GetServices<IRule<M, T, R>>();
			return rules.Any()
				? new Response<R[]>(await rules.To(async rule => await rule.ApplyAsync(metadata, request, cancellationToken)).AllAsync())
				: Response<R[]>.Empty;
		}
	}
}
