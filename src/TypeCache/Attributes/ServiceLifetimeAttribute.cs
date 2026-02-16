// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

public abstract class ServiceLifetimeAttribute(ServiceLifetime serviceLifetime) : Attribute
{
	public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;
}
