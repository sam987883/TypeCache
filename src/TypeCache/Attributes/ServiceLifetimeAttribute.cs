// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

/// <summary>
/// Used to automatically register types.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ServiceLifetimeAttribute : Attribute
{
	public ServiceLifetimeAttribute(ServiceLifetime serviceLifetime)
	{
		this.ServiceLifetime = serviceLifetime;
		this.ServiceType = null;
	}

	public ServiceLifetimeAttribute(ServiceLifetime serviceLifetime, Type serviceType)
	{
		this.ServiceLifetime = serviceLifetime;
		this.ServiceType = serviceType;
	}

	public ServiceLifetime ServiceLifetime { get; }

	public Type? ServiceType { get; }
}
