// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	internal class DefaultProcessHandler<T> : IProcessHandler<T>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultProcessHandler(IServiceProvider serviceProvider)
			=> this._ServiceProvider = serviceProvider;

		public async ValueTask HandleAsync(T request, CancellationToken cancellationToken)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
				{
					var errors = validationRules.If(rule => rule.IsError).ToMany(rule => rule.Messages).ToArray();
					throw new ValidationRuleException(errors);
				}
			}

			var processes = this._ServiceProvider.GetServices<IProcess<T>>();
			await processes.To(async process => await process.RunAsync(request, cancellationToken)).AllAsync<IEnumerable<ValueTask>>();
		}
	}

	internal class DefaultProcessHandler<M, T> : IProcessHandler<M, T>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultProcessHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await Task.WhenAll(validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)));
				if (!results.All(true))
				{
					var errors = validationRules.If(rule => rule.IsError).ToMany(rule => rule.Messages).ToArray();
					throw new ValidationRuleException(errors);
				}
			}

			var processes = this._ServiceProvider.GetServices<IProcess<T>>();
			await processes.To(async process => await process.RunAsync(request, cancellationToken)).AllAsync<IEnumerable<ValueTask>>();
		}
	}
}
