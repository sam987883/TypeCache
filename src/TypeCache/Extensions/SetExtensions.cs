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

		public void ForEach(Action<T, int> action)
		{
			var i = -1;
			@this.ForEach(item => action(item, ++i));
		}
	}
}
