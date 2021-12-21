// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Responses;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Resolvers;

public class ProcedureFieldResolver : IFieldResolver
{
	private readonly IDictionary<string, RuntimeTypeHandle> _Arguments;
	private readonly string _DataSource;
	private readonly IMediator _Mediator;
	private readonly string _Procedure;

	public ProcedureFieldResolver(string dataSource, string procedure, IDictionary<string, RuntimeTypeHandle> arguments, IMediator mediator)
	{
		this._Arguments = arguments;
		this._DataSource = dataSource;
		this._Mediator = mediator;
		this._Procedure = procedure;
	}

	public object? Resolve(IResolveFieldContext context)
	{
		var request = new StoredProcedureRequest
		{
			DataSource = this._DataSource,
			Procedure = this._Procedure
		};
		this._Arguments.Do(argument => request.Parameters.Add(argument.Key, context.GetArgument(argument.Value.ToType(), argument.Key)));
		return this._Mediator.ApplyRulesAsync<StoredProcedureRequest, StoredProcedureResponse>(request);
	}
}
