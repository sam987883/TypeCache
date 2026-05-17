// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Caching;
using Xunit;

namespace TypeCache.Tests.Caching;

public class CachingExtensionsTests
{
	[Fact]
	public void AddCacheItem()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();

		var cacheKey = "test-cache-key";
		services.AddCacheItem(cacheKey, cacheItem =>
		{
			cacheItem.Duration = TimeSpan.FromSeconds(60);
			cacheItem.CreateValue = () => "cached-value";
		});

		var provider = services.BuildServiceProvider();
		var cacheItem = provider.GetKeyedService<CacheItem>(cacheKey);

		Assert.NotNull(cacheItem);
		Assert.Equal(TimeSpan.FromSeconds(60), cacheItem.Duration);
		Assert.Equal("cached-value", cacheItem.Value);
	}

	[Fact]
	public void AddCacheItem_MultipleKeys()
	{
		var services = new ServiceCollection();
		services.AddMemoryCache();

		services.AddCacheItem("key1", cacheItem => cacheItem.CreateValue = () => "value1");
		services.AddCacheItem("key2", cacheItem => cacheItem.CreateValue = () => "value2");

		var provider = services.BuildServiceProvider();

		var cacheItem1 = provider.GetKeyedService<CacheItem>("key1");
		var cacheItem2 = provider.GetKeyedService<CacheItem>("key2");

		Assert.NotNull(cacheItem1);
		Assert.NotNull(cacheItem2);
		Assert.Equal("value1", cacheItem1.Value);
		Assert.Equal("value2", cacheItem2.Value);
	}
}
