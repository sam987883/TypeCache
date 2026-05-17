// Copyright (c) 2021 Samuel Abraham

using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Caching;
using Xunit;

namespace TypeCache.Tests.Caching;

public class CacheItemTests
{
	[Fact]
	public void Value_SetAndGet()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();
		var provider = services.BuildServiceProvider();
		var cache = provider.GetRequiredService<IMemoryCache>();

		var cacheItem = new CacheItem(cache, "test-key");

		cacheItem.Value = "test-value";
		Assert.Equal("test-value", cacheItem.Value);
	}

	[Fact]
	public void Value_GetWithCreateValue()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();
		var provider = services.BuildServiceProvider();
		var cache = provider.GetRequiredService<IMemoryCache>();

		var cacheItem = new CacheItem(cache, "test-key");
		cacheItem.CreateValue = () => "created-value";

		var value = cacheItem.Value;
		Assert.Equal("created-value", value);
	}

	[Fact]
	public void Duration_SetAndGet()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();
		var provider = services.BuildServiceProvider();
		var cache = provider.GetRequiredService<IMemoryCache>();

		var cacheItem = new CacheItem(cache, "test-key");
		var duration = TimeSpan.FromSeconds(30);

		cacheItem.Duration = duration;
		Assert.Equal(duration, cacheItem.Duration);
	}

	[Fact]
	public void Duration_ClearsExpiration()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();
		var provider = services.BuildServiceProvider();
		var cache = provider.GetRequiredService<IMemoryCache>();

		var cacheItem = new CacheItem(cache, "test-key");
		var expiration = DateTime.UtcNow.AddSeconds(30);

		cacheItem.Expiration = expiration;
		cacheItem.Duration = TimeSpan.FromSeconds(30);

		Assert.Null(cacheItem.Expiration);
	}

	[Fact]
	public void Expiration_SetAndGet()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();
		var provider = services.BuildServiceProvider();
		var cache = provider.GetRequiredService<IMemoryCache>();

		var cacheItem = new CacheItem(cache, "test-key");
		var expiration = DateTime.UtcNow.AddSeconds(30);

		cacheItem.Expiration = expiration;
		Assert.Equal(expiration, cacheItem.Expiration);
	}

	[Fact]
	public void Expiration_ClearsDurationAndSliding()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();
		var provider = services.BuildServiceProvider();
		var cache = provider.GetRequiredService<IMemoryCache>();

		var cacheItem = new CacheItem(cache, "test-key");
		cacheItem.Duration = TimeSpan.FromSeconds(30);
		cacheItem.Sliding = true;

		var expiration = DateTime.UtcNow.AddSeconds(60);
		cacheItem.Expiration = expiration;

		Assert.Null(cacheItem.Duration);
		Assert.False(cacheItem.Sliding);
	}

	[Fact]
	public void Sliding_SetAndGet()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();
		var provider = services.BuildServiceProvider();
		var cache = provider.GetRequiredService<IMemoryCache>();

		var cacheItem = new CacheItem(cache, "test-key");
		cacheItem.Duration = TimeSpan.FromSeconds(30);

		cacheItem.Sliding = true;
		Assert.True(cacheItem.Sliding);

		cacheItem.Sliding = false;
		Assert.False(cacheItem.Sliding);
	}
}
