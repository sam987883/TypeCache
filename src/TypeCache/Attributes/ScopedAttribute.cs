// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

/// <inheritdoc cref="ServiceLifetime.Scoped"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ScopedAttribute() : ServiceLifetimeAttribute(ServiceLifetime.Scoped)
{
}
