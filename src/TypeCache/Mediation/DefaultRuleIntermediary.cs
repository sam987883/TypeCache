// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class DefaultRuleIntermediary<REQUEST> : IRuleIntermediary<REQUEST>
	where REQUEST : IRequest
{
	private readonly ILogger<DefaultRuleIntermediary<REQUEST>> _Logger;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;
	private readonly IRule<REQUEST> _Rule;
	private readonly IEnumerable<IAfterRule<REQUEST>> _AfterRules;

	public DefaultRuleIntermediary(
		ILogger<DefaultRuleIntermediary<REQUEST>> logger
		, IEnumerable<IValidationRule<REQUEST>> validationRules
		, IRule<REQUEST> rule
		, IEnumerable<IAfterRule<REQUEST>> afterRules)
	{
		rule.AssertNotNull();

		this._Logger = logger;
		this._ValidationRules = validationRules ?? Array<IValidationRule<REQUEST>>.Empty;
		this._Rule = rule;
		this._AfterRules = afterRules ?? Array<IAfterRule<REQUEST>>.Empty;
	}

	public async ValueTask HandleAsync(REQUEST request, CancellationToken token = default)
	{
		if (this._ValidationRules.Any())
		{
			var validationMessages = this._ValidationRules.SelectMany(_ => _.Validate(request)).ToArray();
			if (validationMessages.Any())
				throw new ValidationException(validationMessages);
		}

		await this._Rule.ApplyAsync(request, token);

		if (this._AfterRules.Any())
		{
			try
			{
				Task.WaitAll(this._AfterRules.Select(rule => rule.ApplyAsync(request, token).AsTask()).ToArray(), token);
			}
			catch(Exception error)
			{
				if (this._Logger is not null)
					this._Logger.LogError(error, "AfterRule failure.");
			}
		}
	}
}

internal sealed class DefaultRuleIntermediary<REQUEST, RESPONSE> : IRuleIntermediary<REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	private readonly ILogger<DefaultRuleIntermediary<REQUEST, RESPONSE>> _Logger;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;
	private readonly IRule<REQUEST, RESPONSE> _Rule;
	private readonly IEnumerable<IAfterRule<REQUEST, RESPONSE>> _AfterRules;

	public DefaultRuleIntermediary(
		ILogger<DefaultRuleIntermediary<REQUEST, RESPONSE>> logger
		, IEnumerable<IValidationRule<REQUEST>> validationRules
		, IRule<REQUEST, RESPONSE> rule
		, IEnumerable<IAfterRule<REQUEST, RESPONSE>> afterRules)
	{
		rule.AssertNotNull();

		this._Logger = logger;
		this._ValidationRules = validationRules ?? Array<IValidationRule<REQUEST>>.Empty;
		this._Rule = rule;
		this._AfterRules = afterRules ?? Array<IAfterRule<REQUEST, RESPONSE>>.Empty;
	}

	public async ValueTask<RESPONSE> HandleAsync(REQUEST request, CancellationToken token = default)
	{
		if (this._ValidationRules.Any())
		{
			var validationMessages = this._ValidationRules.SelectMany(_ => _.Validate(request)).ToArray();
			if (validationMessages.Any())
				throw new ValidationException(validationMessages);
		}

		var response = await this._Rule.ApplyAsync(request, token);

		if (this._AfterRules.Any())
		{
			try
			{
				Task.WaitAll(this._AfterRules.Select(rule => rule.ApplyAsync(request, response, token).AsTask()).ToArray(), token);
			}
			catch (Exception error)
			{
				if (this._Logger is not null)
					this._Logger.LogError(error, "AfterRule failure.");
			}
		}

		return response;
	}
}
