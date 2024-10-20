using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public interface IFieldResolver
{
	/// <summary>
	/// Returns an <see cref="ValueTask{TResult}"/> wrapping an object or <see langword="null"/> for the specified field.
	/// </summary>
	ValueTask<object?> ResolveAsync(IResolveFieldContext context);
}
