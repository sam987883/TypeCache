// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

/// <inheritdoc cref="ServiceLifetime.Transient"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TransientAttribute<T>(object? key = null) : ServiceLifetimeAttribute(ServiceLifetime.Transient, typeof(T), key)
{
}
