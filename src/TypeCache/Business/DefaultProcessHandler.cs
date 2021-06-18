// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;

namespace TypeCache.Business
{
	public class DefaultProcessHandler<T> : IProcessHandler<T>
	{
		private readonly IProcess<T> _Process;
		private readonly IEnumerable<IValidationRule<T>> _ValidationRules;

		public DefaultProcessHandler(IProcess<T> process, IEnumerable<IValidationRule<T>> validationRules)
		{
			this._Process = process;
			this._ValidationRules = validationRules;
		}

		public async ValueTask HandleAsync(T request, CancellationToken cancellationToken)
		{
			if (this._ValidationRules.Any())
			{
				var validationResponses = await this._ValidationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				var exception = validationResponses.First(_ => _.HasError)?.Exception;
				if (exception is not null)
					throw exception;
			}

			await this._Process.RunAsync(request, cancellationToken);
		}
	}

	public class DefaultProcessHandler<M, T> : IProcessHandler<M, T>
	{
		private readonly IProcess<M, T> _Process;
		private readonly IEnumerable<IValidationRule<M, T>> _ValidationRulesWithMetadata;
		private readonly IEnumerable<IValidationRule<T>> _ValidationRules;

		public DefaultProcessHandler(IProcess<M, T> process, IEnumerable<IValidationRule<M, T>> validationRulesWithMetadata, IEnumerable<IValidationRule<T>> validationRules)
		{
			this._Process = process;
			this._ValidationRulesWithMetadata = validationRulesWithMetadata;
			this._ValidationRules = validationRules;
		}

		public async ValueTask HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			if (this._ValidationRulesWithMetadata.Any())
			{
				var validationResponses = await this._ValidationRulesWithMetadata.To(async validationRule => await validationRule.ApplyAsync(metadata, request, cancellationToken)).AllAsync();
				var exception = validationResponses.First(_ => _.HasError)?.Exception;
				if (exception is not null)
					throw exception;
			}

			if (this._ValidationRules.Any())
			{
				var validationResponses = await this._ValidationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				var exception = validationResponses.First(_ => _.HasError)?.Exception;
				if (exception is not null)
					throw exception;
			}

			await this._Process.RunAsync(metadata, request, cancellationToken);
		}
	}
}
