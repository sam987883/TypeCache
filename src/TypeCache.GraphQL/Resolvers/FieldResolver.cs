﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.GraphQL.Resolvers;

public abstract class FieldResolver : IFieldResolver
{
	ValueTask<object?> IFieldResolver.ResolveAsync(IResolveFieldContext context)
	{
		try
		{
			return this.ResolveAsync(context);
		}
		catch (Exception error)
		{
			if (error is ValidationException exception)
				exception.ValidationMessages.ForEach(message => context.Errors.Add(new ExecutionError(message)));
			else
				context.Errors.Add(new ExecutionError(error.Message, error));
			return ValueTask.FromResult<object?>(null);
		}
	}

	protected abstract ValueTask<object?> ResolveAsync(IResolveFieldContext context);
}
