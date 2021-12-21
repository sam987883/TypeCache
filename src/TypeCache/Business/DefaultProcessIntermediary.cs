// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;

namespace TypeCache.Business;

public class DefaultProcessIntermediary<I> : IProcessIntermediary<I>
{
	private readonly IProcess<I> _Process;
	private readonly IEnumerable<IValidationRule<I>> _ValidationRules;

	public DefaultProcessIntermediary(IProcess<I> process, IEnumerable<IValidationRule<I>> validationRules)
	{
		this._Process = process;
		this._ValidationRules = validationRules;
	}

	public async ValueTask HandleAsync(I request, CancellationToken cancellationToken)
	{
		if (this._ValidationRules.Any())
			await this._ValidationRules.DoAsync(async validationRule => await validationRule.ValidateAsync(request));

		await this._Process.RunAsync(request, cancellationToken);
	}
}
