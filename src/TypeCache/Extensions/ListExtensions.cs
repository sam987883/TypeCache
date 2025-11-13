// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class ListExtensions
{
	extension<T>(IList<T> @this) where T : class
	{
		public void AddIfNotNull(T? item)
		{
			@this.ThrowIfNull();

			if (item is not null)
				@this.Add(item);
		}

		public void InsertIfNotNull(int index, T? item)
		{
			@this.ThrowIfNull();

			if (item is not null)
				@this.Insert(index, item);
		}
	}

	extension<T>(IList<T> @this) where T : struct
	{
		public void AddIfHasValue(T? item)
		{
			@this.ThrowIfNull();

			if (item.HasValue)
				@this.Add(item.Value);
		}

		public void InsertIfHasValue(int index, T? item)
		{
			@this.ThrowIfNull();

			if (item.HasValue)
				@this.Insert(index, item.Value);
		}
	}

	extension(IList<string> @this)
	{
		public void AddIfNotBlank(string? item)
		{
			@this.ThrowIfNull();

			if (item.IsNotBlank)
				@this.Add(item);
		}

		public void InsertIfNotBlank(int index, string? item)
		{
			@this.ThrowIfNull();

			if (item.IsNotBlank)
				@this.Insert(index, item);
		}
	}
}
