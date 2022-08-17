// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Data.Business;

internal class DeleteDataValidationRule<T> : IValidationRule<DeleteDataCommand<T>>
	where T : new()
{
	private readonly IRule<SchemaRequest, ObjectSchema> _SchemaRule;

	public DeleteDataValidationRule(IRule<SchemaRequest, ObjectSchema> rule)
	{
		this._SchemaRule = rule;
	}

	public IEnumerable<string> Validate(DeleteDataCommand<T> command)
	{
		var validator = new Validator();
		validator.AssertNotNull(command);
		if (validator.Success)
		{
			validator.AssertNotBlank(command.DataSource);
			validator.AssertNotBlank(command.Table);
		}

		var schema = this._SchemaRule.ApplyAsync(new(command.DataSource, command.Table)).Result;
		validator.AssertEquals(schema.Type, ObjectType.Table);
		if (validator.Success)
		{
			validator.AssertNotEmpty(command.Input);
			validator.AssertEquals(command.Input.Any(row => row is null), false);

			command.Table = schema.Name;
			command.PrimaryKeys = schema.Columns.If(column => column.PrimaryKey).Map(column => column.Name).ToArray();
		}

		if (validator.Success)
		{
			var type = TypeOf<T>.Member;
			if (type.SystemType is SystemType.Tuple || type.SystemType is SystemType.ValueTuple)
				validator.AssertEquals(command.Input.All(row => ((ITuple)row!).Length == command.PrimaryKeys.Length), true);
			else if (type.SystemType is SystemType.IDictionary || type.SystemType is SystemType.Dictionary)
			{
				validator.AssertEquals(command.Input.If<IDictionary<string, object?>>().Count(), command.PrimaryKeys.Length);
				validator.AssertEquals(command.Input.If<IDictionary<string, object?>>().All(row => command.PrimaryKeys.All(row.ContainsKey)), true);
			}
			else if (!type.SystemType.IsSQLType() && command.PrimaryKeys.Length == 1)
				validator.AssertEquals(type.Properties.Any(_ => _.Name.Is(command.PrimaryKeys[0])), true);
			else
				validator.AssertEquals(command.PrimaryKeys.All(primaryKey => type.Properties.Any(_ => _.Name.Is(primaryKey))), true);
		}

		return validator.Fails;
	}
}
