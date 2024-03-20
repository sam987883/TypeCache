// Copyright (c) 2021 Samuel Abraham

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
		catch (ExecutionError error)
		{
			context.Errors.Add(error);
			return null;
		}
		catch (AggregateException error)
		{
			var executionErrors = error.InnerExceptions.Select(exception =>
				exception as ExecutionError ?? new ExecutionError(exception.Message, exception));
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
