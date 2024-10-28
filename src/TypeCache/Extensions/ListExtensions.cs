// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class ListExtensions
{
	public static void AddIfNotNull<T>(this IList<T> @this, T? value)
	{
		@this.ThrowIfNull();

		if (value is not null)
			@this.Add(value);
	}

	public static void InsertIfNotNull<T>(this IList<T> @this, int index, T? value)
	{
		@this.ThrowIfNull();

		if (value is not null)
			@this.Insert(index, value);
	}
}
