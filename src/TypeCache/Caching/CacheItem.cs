// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Caching;

public sealed class CacheItem(IMemoryCache cache, [ServiceKey] object key)
{
	public TimeSpan? Duration
	{
		get;
		set
		{
			field = value;
			if (value.HasValue)
				this.Expiration = null;
		}
	}

	public DateTime? Expiration
	{
		get;
		set
		{
			field = value;
			if (value.HasValue)
			{
				this.Duration = null;
				this.Sliding = false;
			}
		}
	}

	public bool Sliding { get; set; }

	public Func<object>? CreateValue { get; set; }

	public object? Value
	{
		get
		{
			if (cache.TryGetValue(key, out var value))
				return value;

			value = this.CreateValue?.Invoke();
			if (value is not null)
				this.Value = value;

			return value;
		}
		set
		{
			using var cacheEntry = cache.CreateEntry(key);
			cacheEntry.Value = value;
			_ = (this.Duration, this.Expiration) switch
			{
				(null, not null) => cacheEntry.SetAbsoluteExpiration(this.Expiration.Value),
				(not null, null) when this.Sliding => cacheEntry.SetSlidingExpiration(this.Duration.Value),
				(not null, null) => cacheEntry.SetAbsoluteExpiration(this.Duration.Value),
				_ => cacheEntry
			};
		}
	}
}
