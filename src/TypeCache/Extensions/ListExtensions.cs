// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class ListExtensions
{
	public static void AddIfNotBlank(this IList<string> @this, string? item)
	{
		@this.ThrowIfNull();

		if (item.IsNotBlank())
			@this.Add(item);
	}

	public static void AddIfHasValue<T>(this IList<T> @this, T? item)
		where T : struct
	{
		@this.ThrowIfNull();

		if (item.HasValue)
			@this.Add(item.Value);
	}

	public static void AddIfNotNull<T>(this IList<T> @this, T? item)
		where T : class
	{
		@this.ThrowIfNull();

		if (item is not null)
			@this.Add(item);
	}

	public static void InsertIfNotBlank(this IList<string> @this, int index, string? item)
	{
		@this.ThrowIfNull();

		if (item.IsNotBlank())
			@this.Insert(index, item);
	}

	public static void InsertIfHasValue<T>(this IList<T> @this, int index, T? item)
		where T : struct
	{
		@this.ThrowIfNull();

		if (item.HasValue)
			@this.Insert(index, item.Value);
	}

	public static void InsertIfNotNull<T>(this IList<T> @this, int index, T? item)
		where T : class
	{
		@this.ThrowIfNull();

		if (item is not null)
			@this.Insert(index, item);
	}
}
