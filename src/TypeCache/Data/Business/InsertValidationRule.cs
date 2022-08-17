// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class InsertValidationRule : IValidationRule<InsertCommand>
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public InsertValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(InsertCommand command)
	{
		var validator = new Validator();
		validator.AssertNotNull(command);

		if (validator.Success)
		{
			validator.AssertNotBlank(command.DataSource);
			validator.AssertNotBlank(command.Table);
			validator.AssertNotEmpty(command.Columns);
			validator.AssertNotEmpty(command.Select);
			validator.AssertEquals(command.Columns.Length, command.Select.Length);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(command.DataSource, command.Table)).Result;
			schema.Type.AssertEquals(ObjectType.Table);
			if (validator.Success)
			{
				var schemaColumns = schema.Columns.Map(column => column.Name).ToArray();
				validator.AssertEmpty(command.Columns.Without(schemaColumns));
			}

			if (validator.Success)
			{
				command.Table = schema.Name;
				command.From = this._SchemaRule.ApplyAsync(new(command.DataSource, command.From)).Result.Name;
				command.Columns = schema.Columns.If(column => command.Columns.Has(column.Name)).Map(column => column.Name).ToArray();
			}
		}

		return validator.Fails;
	}
}
