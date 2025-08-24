// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class ServiceProviderExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetService&lt;<see cref="ILogger"/>&gt;();</c>
	/// </summary>
	public static ILogger? GetLogger(this IServiceProvider @this)
		=> @this.GetService<ILogger>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetService&lt;<see cref="ILogger{TCategoryName}"/>&gt;();</c>
	/// </summary>
	public static ILogger<T>? GetLogger<T>(this IServiceProvider @this)
		=> @this.GetService<ILogger<T>>();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetService&lt;<see cref="TimeProvider"/>&gt;() ?? <see cref="TimeProvider.System"/>;</c>
	/// </summary>
	public static TimeProvider GetTimeProvider(this IServiceProvider @this)
		=> @this.GetService<TimeProvider>() ?? TimeProvider.System;
}
