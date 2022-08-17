// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;

namespace TypeCache.Data.Business;

internal class DeleteValidationRule : IValidationRule<DeleteCommand>
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public DeleteValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(DeleteCommand command)
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
			validator.AssertEquals(schema.Type, ObjectType.Table);
			if (validator.Success)
				command.Table = schema.Name;
		}

		return validator.Fails;
	}
}
