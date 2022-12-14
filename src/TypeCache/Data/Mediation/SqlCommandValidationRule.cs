// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

internal sealed class SqlCommandValidationRule : IValidationRule<SqlCommand>
{
	public IEnumerable<string> Validate(SqlCommand request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request?.SQL);
		return validator.Fails;
	}
}
