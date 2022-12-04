// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Business;

internal sealed class DefaultRuleIntermediary<REQUEST> : IRuleIntermediary<REQUEST>
{
	private readonly IRule<REQUEST> _Rule;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;

	public DefaultRuleIntermediary(IRule<REQUEST> rule, IEnumerable<IValidationRule<REQUEST>> validationRules)
	{
		this._Rule = rule;
		this._ValidationRules = validationRules ?? Array<IValidationRule<REQUEST>>.Empty;
	}

	public async ValueTask<RuleResponse> GetAsync(REQUEST request, CancellationToken token = default)
	{
		try
		{
			if (this._ValidationRules.Any())
			{
				var validationMessages = this._ValidationRules.SelectMany(_ => _.Validate(request)).ToArray();
				if (validationMessages.Any())
					return new()
					{
						Error = null,
						ValidationMessages = validationMessages,
						Status = RuleResponseStatus.FailValidation
					};
			}

			await this._Rule.ApplyAsync(request, token);
			return new()
			{
				Error = null,
				ValidationMessages = Array<string>.Empty,
				Status = RuleResponseStatus.Success
			};
		}
		catch (Exception error)
		{
			return new()
			{
				Error = error,
				ValidationMessages = Array<string>.Empty,
				Status = RuleResponseStatus.Error
			};
		}
	}
}

internal sealed class DefaultRuleIntermediary<REQUEST, RESPONSE> : IRuleIntermediary<REQUEST, RESPONSE>
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
				var validationMessages = this._ValidationRules.SelectMany(_ => _.Validate(request)).ToArray();
				if (validationMessages.Any())
					return new()
					{
						Error = null,
						ValidationMessages = validationMessages,
						Response = default,
						Status = RuleResponseStatus.FailValidation
					};
			}

			return new()
			{
				Error = null,
				ValidationMessages = Array<string>.Empty,
				Response = await this._Rule.ApplyAsync(request, token),
				Status = RuleResponseStatus.Success
			};
		}
		catch (Exception error)
		{
			return new()
			{
				Error = error,
				ValidationMessages = Array<string>.Empty,
				Response = default,
				Status = RuleResponseStatus.Error
			};
		}
	}
}
