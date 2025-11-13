// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Caching.Memory;

namespace TypeCache.Extensions;

public static class MemoryCacheExtensions
{
	extension(IMemoryCache @this)
	{
		public T GetValue<T>(object key, [NotNullIfNotNull(nameof(defaultValue))] T defaultValue, TimeSpan duration)
			=> @this.GetOrCreate(key, entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = duration;
				entry.Value = defaultValue;
				return defaultValue;
			})!;

		public T GetValue<T>(object key, [NotNullIfNotNull(nameof(defaultValue))] T defaultValue, DateTimeOffset expiration)
			=> @this.GetOrCreate(key, entry =>
			{
				entry.AbsoluteExpiration = expiration;
				entry.Value = defaultValue;
				return defaultValue;
			})!;

		public T GetSlidingValue<T>(object key, [NotNullIfNotNull(nameof(defaultValue))] T defaultValue, TimeSpan duration)
			=> @this.GetOrCreate(key, entry =>
			{
				entry.SlidingExpiration = duration;
				entry.Value = defaultValue;
				return defaultValue;
			})!;
	}
}
