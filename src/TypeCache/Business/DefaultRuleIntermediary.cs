// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;

namespace TypeCache.Business;

public class DefaultRuleIntermediary<I, O> : IRuleIntermediary<I, O>
{
	private readonly IRule<I, O> _Rule;
	private readonly IEnumerable<IValidationRule<I>> _ValidationRules;

	public DefaultRuleIntermediary(IRule<I, O> rule, IEnumerable<IValidationRule<I>> validationRules)
	{
		this._Rule = rule;
		this._ValidationRules = validationRules;
	}

	public async ValueTask<O> HandleAsync(I request, CancellationToken cancellationToken)
	{
		if (this._ValidationRules.Any())
			await this._ValidationRules.DoAsync(async validationRule => await validationRule.ValidateAsync(request));

		return await this._Rule.ApplyAsync(request, cancellationToken);
	}
}
