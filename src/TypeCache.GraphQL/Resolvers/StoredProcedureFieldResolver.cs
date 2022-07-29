// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Resolvers;

public class StoredProcedureFieldResolver : IFieldResolver
{
	private readonly IMediator _Mediator;
	private readonly ObjectSchema _ObjectSchema;
	private readonly Func<DbDataReader, CancellationToken, ValueTask<object>> _ReadDate;

	public StoredProcedureFieldResolver(
		ObjectSchema objectSchema,
		Func<DbDataReader, CancellationToken, ValueTask<object>> readData,
		IMediator mediator)
	{
		this._Mediator = mediator;
		this._ObjectSchema = objectSchema;
		this._ReadDate = readData;
	}

	public async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var request = new StoredProcedureCommand(this._ObjectSchema.Name)
		{
			DataSource = this._ObjectSchema.DataSource,
			ReadData = this._ReadDate
		};
		this._ObjectSchema.Parameters
			.If(parameter => parameter.Direction is ParameterDirection.Input || parameter.Direction is ParameterDirection.InputOutput)
			.Do(parameter => request.InputParameters.Add(parameter.Name, context.GetArgument(parameter.Type.ToType(), parameter.Name)));
		object? value = null;
		await this._Mediator.ApplyRuleAsync<StoredProcedureCommand, object>(request, response => value = response, error => throw error, context.CancellationToken);
		return value;
	}
}
