// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypeCache.Collections.Extensions
{
	public static class EnumerableOfValueTaskExtensions
	{
		/// <summary>
		/// <c>@<paramref name="this"/>.To(<see cref="ValueTask.AsTask"/>)</c>
		/// </summary>
		public static IEnumerable<Task> ToTasks(this IEnumerable<ValueTask> @this)
			=> @this.To(valueTask => valueTask.AsTask());

		/// <summary>
		/// <c>@<paramref name="this"/>.To(<see cref="ValueTask{TResult}.AsTask"/>)</c>
		/// </summary>
		public static IEnumerable<Task<T>> ToTasks<T>(this IEnumerable<ValueTask<T>> @this)
			=> @this.To(valueTask => valueTask.AsTask());
	}
}
