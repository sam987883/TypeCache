// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class InsertDataValidationRule<T> : IValidationRule<InsertDataCommand<T>>
{
	private readonly IMediator _Mediator;

	public InsertDataValidationRule(IMediator mediator, IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._Mediator = mediator;
	}

	public IEnumerable<string> Validate(InsertDataCommand<T> command)
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
			this._Mediator.ApplyRuleAsync<SchemaRequest, ObjectSchema>(new(command.DataSource, command.Table), schema =>
			{
				validator.AssertEquals(schema.Type, ObjectType.Table);
				validator.AssertEquals(command.Columns.Without(schema.Columns.If(column => !column.Identity && !column.ReadOnly).Map(column => column.Name)).Any(), false);
				if (validator.Success)
					command.Table = schema.Name;
			}, validator.IncludeError).GetAwaiter().GetResult();
		}

		return validator.Fails;
	}
}
