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

	public IEnumerable<string> Validate(StoredProcedureCommand command)
	{
		var validator = new Validator();
		validator.AssertNotNull(command);
		if (validator.Success)
		{
			validator.AssertNotBlank(command.DataSource);
			validator.AssertNotBlank(command.Procedure);
		}

		if (validator.Success)
		{
			var schema = this._SchemaRule.ApplyAsync(new(command.DataSource, command.Procedure)).Result;
			schema.Type.AssertEquals(ObjectType.StoredProcedure);

			if (validator.Success)
			{
				command.Procedure = schema.Name;

				var inputParameters = schema.Parameters
					.If(parameter => parameter.Direction is ParameterDirection.Input || parameter.Direction is ParameterDirection.InputOutput)
					.Map(parameter => parameter.Name);
				validator.AssertEquals(inputParameters.Without(command.InputParameters.Keys).Any(), false);

				var outputParameters = schema.Parameters
					.If(parameter => parameter.Direction is ParameterDirection.Output || parameter.Direction is ParameterDirection.InputOutput)
					.Map(parameter => parameter.Name);
				validator.AssertEquals(outputParameters.Without(command.OutputParameters.Keys).Any(), false);
			}
		}

		return validator.Fails;
	}
}
