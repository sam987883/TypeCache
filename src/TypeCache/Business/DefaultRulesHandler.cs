// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business.Extensions;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	public class DefaultRulesHandler<T, R> : IRulesHandler<T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRulesHandler(IServiceProvider serviceProvider)
			=> this._ServiceProvider = serviceProvider;

		public async ValueTask<Response<R[]>> HandleAsync(T request, CancellationToken cancellationToken)
		{
			var validationResponses = await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken);
			if (!validationResponses.Any(_ => _.IsError))
			{
				var rules = this._ServiceProvider.GetServices<IRule<T, R>>();
				return rules.Any()
					? new Response<R[]>(await rules.To(async rule => await rule.ApplyAsync(request, cancellationToken)).AllAsync())
					: Response<R[]>.Empty;
			}
			return new Response<R[]>(validationResponses);
		}
	}

	public class DefaultRulesHandler<M, T, R> : IRulesHandler<M, T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRulesHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<R[]>> HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
				.Union(await this.ApplyValidationRules(this._ServiceProvider, metadata, request, cancellationToken))
				.ToArray();
			if (!validationResponses.Any(_ => _.IsError))
			{
				var rules = this._ServiceProvider.GetServices<IRule<M, T, R>>();
				return rules.Any()
					? new Response<R[]>(await rules.To(async rule => await rule.ApplyAsync(metadata, request, cancellationToken)).AllAsync())
					: Response<R[]>.Empty;
			}
			return new Response<R[]>(validationResponses);
		}
	}
}
