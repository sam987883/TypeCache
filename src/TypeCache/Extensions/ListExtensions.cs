// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class ListExtensions
{
	public static void AddIfNotBlank(this IList<string> @this, string? value)
	{
		@this.ThrowIfNull();

		if (value.IsNotBlank())
			@this.Add(value);
	}

	public static void AddIfNotNull<T>(this IList<T> @this, T? value)
	{
		@this.ThrowIfNull();

		if (value is not null)
			@this.Add(value);
	}

	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static async Task ForEachAsync<T>(this IList<T> @this, Action<T> action, CancellationToken token = default)
	{
		@this.ThrowIfNull();
		action.ThrowIfNull();

		foreach (var item in @this)
		{
			if (token.IsCancellationRequested)
			{
				await Task.FromCanceled(token);
				return;
			}

			action(item);
		}

		await Task.CompletedTask;
	}

	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static async Task ForEachAsync<T>(this IList<T> @this, Action<T, int> action, CancellationToken token = default)
	{
		@this.ThrowIfNull();
		action.ThrowIfNull();

		var i = -1;
		foreach (var item in @this)
		{
			if (token.IsCancellationRequested)
			{
				await Task.FromCanceled(token);
				return;
			}

			action(item, ++i);
		}

		await Task.CompletedTask;
	}

	public static void InsertIfNotBlank(this IList<string> @this, int index, string? value)
	{
		@this.ThrowIfNull();

		if (value.IsNotBlank())
			@this.Insert(index, value);
	}

	public static void InsertIfNotNull<T>(this IList<T> @this, int index, T? value)
	{
		@this.ThrowIfNull();

		if (value is not null)
			@this.Insert(index, value);
	}
}
