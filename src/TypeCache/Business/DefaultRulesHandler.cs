// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	public class DefaultRulesHandler<T, R> : IRulesHandler<T, R>
	{
		private readonly IEnumerable<IRule<T, R>> _Rules;
		private readonly IEnumerable<IValidationRule<T>> _ValidationRules;

		public DefaultRulesHandler(IEnumerable<IRule<T, R>> rules, IEnumerable<IValidationRule<T>> validationRules)
		{
			this._Rules = rules;
			this._ValidationRules = validationRules;
		}

		public async ValueTask<Response<R[]>> HandleAsync(T request, CancellationToken cancellationToken)
		{
			try
			{
				if (this._ValidationRules.Any())
				{
					var validationResponses = await this._ValidationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
					var exception = validationResponses.First(_ => _!.HasError)?.Exception;
					if (exception is not null)
						return new Response<R[]>(exception);
				}

				if (this._Rules.Any())
				{
					var result = await this._Rules.To(async rule => await rule.ApplyAsync(request, cancellationToken)).AllAsync();
					return new Response<R[]>(result);
				}

				return Response<R[]>.Empty;
			}
			catch (Exception exception)
			{
				return new Response<R[]>(exception);
			}
		}
	}

	public class DefaultRulesHandler<M, T, R> : IRulesHandler<M, T, R>
	{
		private readonly IEnumerable<IRule<M, T, R>> _Rules;
		private readonly IEnumerable<IValidationRule<M, T>> _ValidationRulesWithMetadata;
		private readonly IEnumerable<IValidationRule<T>> _ValidationRules;

		public DefaultRulesHandler(IEnumerable<IRule<M, T, R>> rules, IEnumerable<IValidationRule<M, T>> validationRulesWithMetadata, IEnumerable<IValidationRule<T>> validationRules)
		{
			this._Rules = rules;
			this._ValidationRulesWithMetadata = validationRulesWithMetadata;
			this._ValidationRules = validationRules;
		}

		public async ValueTask<Response<R[]>> HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			try
			{
				if (this._ValidationRulesWithMetadata.Any())
				{
					var validationResponses = await this._ValidationRulesWithMetadata.To(async validationRule => await validationRule.ApplyAsync(metadata, request, cancellationToken)).AllAsync();
					var exception = validationResponses.First(_ => _!.HasError)?.Exception;
					if (exception is not null)
						return new Response<R[]>(exception);
				}

				if (this._ValidationRules.Any())
				{
					var validationResponses = await this._ValidationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
					var exception = validationResponses.First(_ => _!.HasError)?.Exception;
					if (exception is not null)
						return new Response<R[]>(exception);
				}

				if (this._Rules.Any())
				{
					var result = await this._Rules.To(async rule => await rule.ApplyAsync(metadata, request, cancellationToken)).AllAsync();
					return new Response<R[]>(result);
				}

				return Response<R[]>.Empty;
			}
			catch (Exception exception)
			{
				return new Response<R[]>(exception);
			}
		}
	}
}
