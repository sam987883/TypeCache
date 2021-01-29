// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business.Extensions;
using TypeCache.Collections.Extensions;

namespace TypeCache.Business
{
	public class DefaultRuleHandler<T, R> : IRuleHandler<T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRuleHandler(IServiceProvider serviceProvider)
			=> this._ServiceProvider = serviceProvider;

		public async ValueTask<Response<R>> HandleAsync(T request, CancellationToken cancellationToken)
		{
			var validationResponses = await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken);
			if (!validationResponses.Any(_ => _.IsError))
			{
				var rule = this._ServiceProvider.GetRequiredService<IRule<T, R>>();
				return new Response<R>(await rule.ApplyAsync(request, cancellationToken));
			}
			return new Response<R>(validationResponses);
		}
	}

	public class DefaultRuleHandler<M, T, R> : IRuleHandler<M, T, R>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<R>> HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
				.Union(await this.ApplyValidationRules(this._ServiceProvider, metadata, request, cancellationToken))
				.ToArray();
			if (!validationResponses.Any(_ => _.IsError))
			{
				var rule = this._ServiceProvider.GetRequiredService<IRule<M, T, R>>();
				return new Response<R>(await rule.ApplyAsync(metadata, request, cancellationToken));
			}
			return new Response<R>(validationResponses);
		}
	}
}
