// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Resolvers;

namespace TypeCache.GraphQL.Resolvers;

public abstract class SourceStreamResolver : ISourceStreamResolver
{
	async ValueTask<IObservable<object?>> ISourceStreamResolver.ResolveAsync(IResolveFieldContext context)
	{
		try
		{
			return await this.ResolveAsync(context);
		}
		catch (AggregateException error)
		{
			var executionErrors = error.InnerExceptions.Select(exception => new ExecutionError(exception.Message, exception));
			context.Errors.AddRange(executionErrors);
			return null!;
		}
		catch (Exception error)
		{
			context.Errors.Add(new ExecutionError(error.Message, error));
			return null!;
		}
	}

	protected abstract ValueTask<IObservable<object?>> ResolveAsync(IResolveFieldContext context);
}
