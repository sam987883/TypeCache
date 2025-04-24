// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

public abstract class ServiceLifetimeAttribute(ServiceLifetime serviceLifetime, Type? serviceType = null, object? key = null) : Attribute
{
	public object? Key { get; } = key;

	public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;

	public Type? ServiceType { get; } = serviceType;
}
