using TypeCache.Extensions;

namespace GraphQL;

/// <summary>
/// Activator.CreateInstance based service provider.
/// </summary>
/// <seealso cref="IServiceProvider" />
public sealed class DefaultServiceProvider : IServiceProvider
{
	/// <summary>
	/// Gets an instance of the specified type. Returns <see langword="null"/> for interfaces.
	/// Can not return <see langword="null"/> for classes but may throw exception.
	/// </summary>
	/// <param name="serviceType">Desired type</param>
	/// <returns>An instance of <paramref name="serviceType"/>.</returns>
	public object? GetService(Type serviceType)
	{
		serviceType.ThrowIfNull();

		if (serviceType == typeof(IServiceProvider) || serviceType == typeof(DefaultServiceProvider))
			return this;

		if (serviceType.IsInterface || serviceType.IsAbstract || serviceType.IsGenericTypeDefinition)
			return null;

		try
		{
			return serviceType.Create();
		}
		catch (Exception exception)
		{
			throw new Exception($"Failed to call Activator.CreateInstance. Type: {serviceType.FullName}", exception);
		}
	}
}
