// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class StoredProcedureValidationRule : IValidationRule<StoredProcedureCommand>
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public StoredProcedureValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(StoredProcedureCommand request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request);
		if (validator.Success)
		{
			validator.AssertNotBlank(request.DataSource);
			validator.AssertNotBlank(request.Procedure);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(request.DataSource, request.Procedure)).Result;
			schema.Type.AssertEquals(ObjectType.StoredProcedure);

			if (validator.Success)
			{
				request.Procedure = schema.Name;

				var inputParameters = schema.Parameters
					.If(parameter => parameter.Direction is ParameterDirection.Input || parameter.Direction is ParameterDirection.InputOutput)
					.Map(parameter => parameter.Name);
				validator.AssertEquals(inputParameters.Without(request.InputParameters.Keys).Any(), false);

				var outputParameters = schema.Parameters
					.If(parameter => parameter.Direction is ParameterDirection.Output || parameter.Direction is ParameterDirection.InputOutput)
					.Map(parameter => parameter.Name);
				validator.AssertEquals(outputParameters.Without(request.OutputParameters.Keys).Any(), false);
			}
		}

		return validator.Fails;
	}
}
