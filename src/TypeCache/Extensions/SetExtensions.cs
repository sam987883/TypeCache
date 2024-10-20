namespace TypeCache.Extensions;

public static class SetExtensions
{
	public static void ForEach<T>(this ISet<T> @this, Action<T> action)
	{
		action.ThrowIfNull();

		foreach (var item in @this)
			action(item);
	}

	public static void ForEach<T>(this ISet<T> @this, Action<T, int> action)
	{
		var i = -1;
		@this.ForEach(item => action(item, ++i));
	}
}
