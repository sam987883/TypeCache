// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;

namespace TypeCache.Data.Business;

internal class CountValidationRule : IValidationRule<CountCommand>
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public CountValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(CountCommand request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request);

		if (validator.Success)
		{
			validator.AssertNotBlank(request.DataSource);
			validator.AssertNotBlank(request.Table);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(request.DataSource, request.Table)).Result;
			request.Table = schema.Name;
		}

		return validator.Fails;
	}
}
