// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class EnumerableOfValueTaskExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(_ =&gt; _.AsTask());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<Task> ToTasks(this IEnumerable<ValueTask> @this)
		=> @this.Map(_ => _.AsTask());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Map(_ =&gt; _.AsTask());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<Task<T>> ToTasks<T>(this IEnumerable<ValueTask<T>> @this)
		=> @this.Map(_ => _.AsTask());
}
