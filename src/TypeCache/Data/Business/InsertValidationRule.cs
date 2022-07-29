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

	public IEnumerable<string> Validate(InsertCommand request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request);

		if (validator.Success)
		{
			validator.AssertNotBlank(request.DataSource);
			validator.AssertNotBlank(request.Table);
			validator.AssertNotEmpty(request.Columns);
			validator.AssertNotEmpty(request.Select);
			validator.AssertEquals(request.Columns.Length, request.Select.Length);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(request.DataSource, request.Table)).Result;
			schema.Type.AssertEquals(ObjectType.Table);
			if (validator.Success)
			{
				var schemaColumns = schema.Columns.Map(column => column.Name).ToArray();
				validator.AssertEmpty(request.Columns.Without(schemaColumns));
			}

			if (validator.Success)
			{
				request.Table = schema.Name;
				request.From = this._SchemaRule.ApplyAsync(new(request.DataSource, request.From)).Result.Name;
				request.Columns = schema.Columns.If(column => request.Columns.Has(column.Name)).Map(column => column.Name).ToArray();
			}
		}

		return validator.Fails;
	}
}
