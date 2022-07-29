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

	public IEnumerable<string> Validate(SchemaRequest request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request);

		if (validator.Success)
			validator.AssertNotNull(request.DataSource);

		if (validator.Success)
			validator.AssertEquals(this._DataSourceAccessor.Has(request.DataSource), true);

		if (validator.Success)
			validator.AssertNotNull(this._DataSourceAccessor[request.DataSource]);

		return validator.Fails;
	}
}
