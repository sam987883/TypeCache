﻿// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Attributes;

/// <summary>
/// Used to automatically register types.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class ServiceLifetimeAttribute(ServiceLifetime serviceLifetime, object? key, Type? serviceType = null) : Attribute
{
	public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;

	public Type? ServiceType { get; } = serviceType;

	public object? Key { get; } = key;
}

/// <summary>
/// Used to automatically register types.
/// </summary>
/// <typeparam name="T">The contract that this type implements, or specify the current type to register itself.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ServiceLifetimeAttribute<T>(ServiceLifetime serviceLifetime, object? key = null) : ServiceLifetimeAttribute(serviceLifetime, key, typeof(T))
{
}
