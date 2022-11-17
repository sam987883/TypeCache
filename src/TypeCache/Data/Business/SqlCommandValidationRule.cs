// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;

namespace TypeCache.Data.Business;

internal sealed class SqlCommandValidationRule : IValidationRule<SqlCommand>
{
	public IEnumerable<string> Validate(SqlCommand request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request);
		validator.AssertNotNull(request?.SQL);
		return validator.Fails;
	}
}
