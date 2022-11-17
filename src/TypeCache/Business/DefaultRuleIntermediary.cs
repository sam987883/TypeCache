// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Business;

public sealed class DefaultRuleIntermediary<REQUEST, RESPONSE> : IRuleIntermediary<REQUEST, RESPONSE>
{
	private readonly IRule<REQUEST, RESPONSE> _Rule;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;

	public DefaultRuleIntermediary(IRule<REQUEST, RESPONSE> rule, IEnumerable<IValidationRule<REQUEST>> validationRules)
	{
		this._Rule = rule;
		this._ValidationRules = validationRules ?? Array<IValidationRule<REQUEST>>.Empty;
	}

	public async ValueTask<RuleResponse<RESPONSE>> GetAsync(REQUEST request, CancellationToken token = default)
	{
		try
		{
			if (this._ValidationRules.Any())
			{
				var failedValidation = this._ValidationRules.ToMany(_ => _.Validate(request)).ToArray();
				if (failedValidation.Any())
					return new(failedValidation);
			}

			return new(await this._Rule.ApplyAsync(request, token));
		}
		catch (Exception error)
		{
			return new(error);
		}
	}
}
