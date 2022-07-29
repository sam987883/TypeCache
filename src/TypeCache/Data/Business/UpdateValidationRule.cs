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

	public IEnumerable<string> Validate(UpdateCommand request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request);
		if (validator.Success)
		{
			validator.AssertNotBlank(request.DataSource);
			validator.AssertNotBlank(request.Table);
			validator.AssertEquals(request.Columns.Any(), true);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(request.DataSource, request.Table)).Result;
			schema.Type.AssertEquals(ObjectType.Table);

			if (validator.Success)
				request.Table = schema.Name;
		}

		return validator.Fails;
	}
}
