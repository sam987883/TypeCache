// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class SelectValidationRule : IValidationRule<SelectCommand>
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public SelectValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(SelectCommand command)
	{
		var validator = new Validator();
		validator.AssertNotNull(command);
		if (validator.Success)
		{
			validator.AssertNotBlank(command.DataSource);
			validator.AssertNotBlank(command.From);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(command.DataSource, command.From)).Result;
			validator.AssertEquals(schema.Type is ObjectType.Table || schema.Type is ObjectType.View || schema.Type is ObjectType.Function, true);

			if (validator.Success)
			{
				command.From = schema.Name;
				command.Select = schema.Columns.Map(column => column.Name).If(name => command.Select.Has(name, StringComparison.OrdinalIgnoreCase)).ToArray();
			}
		}

		return validator.Fails;
	}
}
