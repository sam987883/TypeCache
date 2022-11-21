// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Resolvers;

public abstract class FieldResolver<T> : IFieldResolver
{
	async ValueTask<object?> IFieldResolver.ResolveAsync(IResolveFieldContext context)
	{
		try
		{
			return await this.ResolveAsync(context);
		}
		catch (Exception error)
		{
			if (error is ValidationException exception)
				exception.ValidationMessages.ForEach(message => context.Errors.Add(new ExecutionError(message)));
			else
				context.Errors.Add(new ExecutionError(error.Message, error));
			return default;
		}
	}

	protected abstract ValueTask<T?> ResolveAsync(IResolveFieldContext context);
}
