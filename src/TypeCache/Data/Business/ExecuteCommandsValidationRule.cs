// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Data.Domain;

namespace TypeCache.Data.Business;

internal class ExecuteCommandsValidationRule : IValidationRule<ExecuteCommands>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public ExecuteCommandsValidationRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	public IEnumerable<string> Validate(ExecuteCommands commands)
	{
		var validator = new Validator();
		validator.AssertNotNull(commands);

		if (validator.Success)
		{
			validator.AssertNotBlank(commands.DataSource);
			validator.AssertNotNull(commands.Execute);
			validator.AssertEquals(this._DataSourceAccessor.Has(commands.DataSource), true);
		}

		return validator.Fails;
	}
}
