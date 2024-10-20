namespace GraphQL.Utilities.Federation;

public class FuncFederatedResolver<T>(Func<FederatedResolveContext, Task<T?>> resolver) : IFederatedResolver
{
	public async Task<object?> Resolve(FederatedResolveContext context)
		=> await resolver(context);
}
