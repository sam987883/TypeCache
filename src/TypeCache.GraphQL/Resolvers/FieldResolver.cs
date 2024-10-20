// Copyright (c) 2021 Samuel Abraham

using GraphQL;

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
		catch (AggregateException ex)
		{
			foreach (var error in ex.InnerExceptions)
				context.Errors.Add(new(error.Message, error));

			return null;
		}
		catch (Exception ex)
		{
			context.Errors.Add(new(ex.Message, ex));
			return null;
		}
	}

	protected abstract Task<object?> ResolveAsync(IResolveFieldContext context);
}
