// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.GraphQL.Resolvers;

public abstract class SourceStreamResolver : ISourceStreamResolver
{
	ValueTask<IObservable<object?>> ISourceStreamResolver.ResolveAsync(IResolveFieldContext context)
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
			return ValueTask.FromResult<IObservable<object?>>(default!);
		}
	}

	protected abstract ValueTask<IObservable<object?>> ResolveAsync(IResolveFieldContext context);
}
