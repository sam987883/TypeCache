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

	public IEnumerable<string> Validate(DeleteDataCommand<T> request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request);
		if (validator.Success)
		{
			validator.AssertNotBlank(request.DataSource);
			validator.AssertNotBlank(request.Table);
		}

		var schema = this._SchemaRule.ApplyAsync(new(request.DataSource, request.Table)).Result;
		validator.AssertEquals(schema.Type, ObjectType.Table);
		if (validator.Success)
		{
			validator.AssertNotEmpty(request.Input);
			validator.AssertEquals(request.Input.Any(row => row is null), false);

			request.Table = schema.Name;
			request.PrimaryKeys = schema.Columns.If(column => column.PrimaryKey).Map(column => column.Name).ToArray();
		}

		if (validator.Success)
		{
			var type = TypeOf<T>.Member;
			if (type.SystemType is SystemType.Tuple || type.SystemType is SystemType.ValueTuple)
				validator.AssertEquals(request.Input.All(row => ((ITuple)row!).Length == request.PrimaryKeys.Length), true);
			else if (type.SystemType is SystemType.IDictionary || type.SystemType is SystemType.Dictionary)
			{
				validator.AssertEquals(request.Input.If<IDictionary<string, object?>>().Count(), request.PrimaryKeys.Length);
				validator.AssertEquals(request.Input.If<IDictionary<string, object?>>().All(row => request.PrimaryKeys.All(row.ContainsKey)), true);
			}
			else if (!type.SystemType.IsSQLType() && request.PrimaryKeys.Length == 1)
				validator.AssertEquals(type.Properties.Any(_ => _.Name.Is(request.PrimaryKeys[0])), true);
			else
				validator.AssertEquals(request.PrimaryKeys.All(primaryKey => type.Properties.Any(_ => _.Name.Is(primaryKey))), true);
		}

		return validator.Fails;
	}
}
