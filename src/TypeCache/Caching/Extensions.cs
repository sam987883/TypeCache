// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Caching;

public static class Extensions
{
	extension(IServiceCollection @this)
	{
		public IServiceCollection AddCacheItem(object key, Action<CacheItem> configure)
			=> @this.AddKeyedSingleton(typeof(CacheItem), key, (provider, key) =>
			{
				var cache = provider.GetRequiredService<IMemoryCache>();
				var cacheItem = new CacheItem(cache, key!);
				configure(cacheItem);
				return cacheItem;
			});
	}
}
