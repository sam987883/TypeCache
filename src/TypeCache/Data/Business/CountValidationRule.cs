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

	public IEnumerable<string> Validate(CountCommand command)
	{
		var validator = new Validator();
		validator.AssertNotNull(command);

		if (validator.Success)
		{
			validator.AssertNotBlank(command.DataSource);
			validator.AssertNotBlank(command.Table);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(command.DataSource, command.Table)).Result;
			command.Table = schema.Name;
		}

		return validator.Fails;
	}
}
