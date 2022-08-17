// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class UpdateValidationRule : IValidationRule<UpdateCommand>
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public UpdateValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(UpdateCommand command)
	{
		var validator = new Validator();
		validator.AssertNotNull(command);
		if (validator.Success)
		{
			validator.AssertNotBlank(command.DataSource);
			validator.AssertNotBlank(command.Table);
			validator.AssertEquals(command.Set.Any(), true);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(command.DataSource, command.Table)).Result;
			schema.Type.AssertEquals(ObjectType.Table);

			if (validator.Success)
				command.Table = schema.Name;
		}

		return validator.Fails;
	}
}
