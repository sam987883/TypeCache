// Copyright (c) 2021 Samuel Abraham

using GraphQL;

namespace TypeCache.GraphQL.Resolvers;

public abstract class SourceStreamResolver : ISourceStreamResolver
{
	async ValueTask<IObservable<object?>> ISourceStreamResolver.ResolveAsync(IResolveFieldContext context)
	{
		try
		{
			return await this.ResolveAsync(context);
		}
		catch (AggregateException ex)
		{
			foreach (var error in ex.InnerExceptions)
				context.Errors.Add(new(error.Message, error));

			return null!;
		}
		catch (Exception ex)
		{
			context.Errors.Add(new(ex.Message, ex));
			return null!;
		}
	}

	protected abstract ValueTask<IObservable<object?>> ResolveAsync(IResolveFieldContext context);
}
