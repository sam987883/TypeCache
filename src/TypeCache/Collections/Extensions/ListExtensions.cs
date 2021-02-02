// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Extensions;

namespace TypeCache.Collections.Extensions
{
	public static class ListExtensions
	{
		public static void Do<T>(this List<T>? @this, Action<T> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this?.Count ?? 0;
			for (var i = 0; i < count; ++i)
				action(@this![i]);
		}

		public static void Do<T>(this List<T>? @this, Action<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			var count = @this?.Count ?? 0;
			for (var i = 0; i < count; ++i)
				action(@this![i], i);
		}

		public static void Do<T>(this List<T>? @this, Action<T> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this?.Count ?? 0;
			if (count > 0)
			{
				action(@this![0]);
				for (var i = 1; i < count; ++i)
				{
					between();
					action(@this[i]);
				}
			}
		}

		public static void Do<T>(this List<T>? @this, Action<T, int> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			var count = @this?.Count ?? 0;
			if (count > 0)
			{
				var i = 0;
				action(@this![0], i);
				for (i = 1; i < count; ++i)
				{
					between();
					action(@this[i], i);
				}
			}
		}
	}
}
