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

	public IEnumerable<string> Validate(UpdateDataCommand<T> request)
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
			validator.AssertEquals(schema.Type, ObjectType.Table);
			validator.AssertEquals(request.Columns.Without(schema.Columns.If(column => !column.Identity && !column.ReadOnly).Map(column => column.Name)).Any(), false);

			if (validator.Success)
			{
				request.Table = schema.Name;
				if (!request.On.Any())
					request.On = schema.Columns.If(column => column.PrimaryKey).Map(column => column.Name).ToArray();
			}
		}

		return validator.Fails;
	}
}
