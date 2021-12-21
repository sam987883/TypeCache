// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TypeCache.Collections.Extensions;

public static class EnumerableOfValueTaskExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(valueTask =&gt; valueTask.AsTask());</c>
	/// </summary>
	public static IEnumerable<Task> ToTasks(this IEnumerable<ValueTask> @this)
		=> @this.To(valueTask => valueTask.AsTask());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(valueTask =&gt; valueTask.AsTask());</c>
	/// </summary>
	public static IEnumerable<Task<T>> ToTasks<T>(this IEnumerable<ValueTask<T>> @this)
		=> @this.To(valueTask => valueTask.AsTask());
}
