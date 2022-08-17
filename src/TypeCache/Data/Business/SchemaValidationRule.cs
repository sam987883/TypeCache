// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Data.Domain;

namespace TypeCache.Data.Business;

internal class SchemaValidationRule : IValidationRule<SchemaRequest>
{
	private readonly IAccessor<DataSource> _DataSourceAccessor;

	public SchemaValidationRule(IAccessor<DataSource> dataSourceAccessor)
	{
		this._DataSourceAccessor = dataSourceAccessor;
	}

	public IEnumerable<string> Validate(SchemaRequest command)
	{
		var validator = new Validator();
		validator.AssertNotNull(command);

		if (validator.Success)
			validator.AssertNotNull(command.DataSource);

		if (validator.Success)
			validator.AssertEquals(this._DataSourceAccessor.Has(command.DataSource), true);

		if (validator.Success)
			validator.AssertNotNull(this._DataSourceAccessor[command.DataSource]);

		return validator.Fails;
	}
}
