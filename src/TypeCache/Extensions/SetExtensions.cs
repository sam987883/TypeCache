namespace TypeCache.Extensions;

public static class SetExtensions
{
	extension<T>(ISet<T> @this)
	{
		public void ForEach(Action<T> action)
		{
			action.ThrowIfNull();

			foreach (var item in @this)
				action(item);
		}
	}
}
