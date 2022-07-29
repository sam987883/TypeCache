// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;

namespace TypeCache.Business;

public class DefaultProcessIntermediary<REQUEST> : IProcessIntermediary<REQUEST>
{
	private readonly IProcess<REQUEST> _Process;
	private readonly IEnumerable<IValidationRule<REQUEST>> _ValidationRules;

	public DefaultProcessIntermediary(IProcess<REQUEST> process, IEnumerable<IValidationRule<REQUEST>> validationRules)
	{
		this._Process = process;
		this._ValidationRules = validationRules;
	}

	public async ValueTask RunAsync(REQUEST request, CancellationToken token)
	{
		try
		{
			var validationMessages = this._ValidationRules.Map(_ => _.Validate(request)).Gather().ToArray();
			if (validationMessages.Any())
				await ValueTask.FromException(new ValidationException(validationMessages));
			else
				await this._Process.PublishAsync(request, token);
		}
		catch (Exception error)
		{
			await ValueTask.FromException(error);
		}
	}
}
