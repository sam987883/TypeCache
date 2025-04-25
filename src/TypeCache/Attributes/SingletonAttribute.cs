// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

/// <inheritdoc cref="ServiceLifetime.Singleton"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class SingletonAttribute(object? key = null) : ServiceLifetimeAttribute(ServiceLifetime.Singleton, null, key)
{
}

/// <inheritdoc cref="ServiceLifetime.Singleton"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class SingletonAttribute<T>(object? key = null) : ServiceLifetimeAttribute(ServiceLifetime.Singleton, typeof(T), key)
{
}
