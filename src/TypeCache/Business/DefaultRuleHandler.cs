// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;

namespace TypeCache.Business
{
	public class DefaultRuleHandler<T, R> : IRuleHandler<T, R>
	{
		private readonly IRule<T, R> _Rule;
		private readonly IEnumerable<IValidationRule<T>> _ValidationRules;

		public DefaultRuleHandler(IRule<T, R> rule, IEnumerable<IValidationRule<T>> validationRules)
		{
			this._Rule = rule;
			this._ValidationRules = validationRules;
		}

		public async ValueTask<Response<R>> HandleAsync(T request, CancellationToken cancellationToken)
		{
			try
			{
				if (this._ValidationRules.Any())
				{
					var validationResponses = await this._ValidationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
					var exception = validationResponses.First(_ => _!.HasError)?.Exception;
					if (exception is not null)
						return new Response<R>(exception);
				}

				var result = await this._Rule.ApplyAsync(request, cancellationToken);
				return new Response<R>(result);
			}
			catch (Exception exception)
			{
				return new Response<R>(exception);
			}
		}
	}

	public class DefaultRuleHandler<M, T, R> : IRuleHandler<M, T, R>
	{
		private readonly IRule<M, T, R> _Rule;
		private readonly IEnumerable<IValidationRule<M, T>> _ValidationRulesWithMetadata;
		private readonly IEnumerable<IValidationRule<T>> _ValidationRules;

		public DefaultRuleHandler(IRule<M, T, R> rule, IEnumerable<IValidationRule<M, T>> validationRulesWithMetadata, IEnumerable<IValidationRule<T>> validationRules)
		{
			this._Rule = rule;
			this._ValidationRulesWithMetadata = validationRulesWithMetadata;
			this._ValidationRules = validationRules;
		}

		public async ValueTask<Response<R>> HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			try
			{
				if (this._ValidationRulesWithMetadata.Any())
				{
					var validationResponses = await this._ValidationRulesWithMetadata.To(async validationRule => await validationRule.ApplyAsync(metadata, request, cancellationToken)).AllAsync();
					var exception = validationResponses.First(_ => _!.HasError)?.Exception;
					if (exception is not null)
						return new Response<R>(exception);
				}

				if (this._ValidationRules.Any())
				{
					var validationResponses = await this._ValidationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
					var exception = validationResponses.First(_ => _!.HasError)?.Exception;
					if (exception is not null)
						return new Response<R>(exception);
				}

				var result = await this._Rule.ApplyAsync(metadata, request, cancellationToken);
				return new Response<R>(result);
			}
			catch (Exception exception)
			{
				return new Response<R>(exception);
			}
		}
	}
}
