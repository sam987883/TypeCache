// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;

namespace TypeCache.GraphQL.Resolvers;

public abstract class FieldResolver : IFieldResolver
{
	async ValueTask<object?> IFieldResolver.ResolveAsync(IResolveFieldContext context)
	{
		try
		{
			return await this.ResolveAsync(context);
		}
		catch (AggregateException error)
		{
			var executionErrors = error.InnerExceptions.Select(exception => new ExecutionError(exception.Message, exception));
			context.Errors.AddRange(executionErrors);
			return null;
		}
		catch (Exception error)
		{
			context.Errors.Add(new ExecutionError(error.Message, error));
			return null;
		}
	}

	protected abstract ValueTask<object?> ResolveAsync(IResolveFieldContext context);
}
