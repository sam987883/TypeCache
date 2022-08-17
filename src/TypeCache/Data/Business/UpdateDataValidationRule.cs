// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class UpdateDataValidationRule<T> : IValidationRule<UpdateDataCommand<T>>
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public UpdateDataValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(UpdateDataCommand<T> command)
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
			validator.AssertEquals(command.Columns.Without(schema.Columns.If(column => !column.Identity && !column.ReadOnly).Map(column => column.Name)).Any(), false);

			if (validator.Success)
			{
				command.Table = schema.Name;
				if (!command.On.Any())
					command.On = schema.Columns.If(column => column.PrimaryKey).Map(column => column.Name).ToArray();
			}
		}

		return validator.Fails;
	}
}
