// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class ServiceProviderExtensions
{
	extension(IServiceProvider @this)
	{
		/// <summary>
		/// <c>=&gt; @this.GetService&lt;<see cref="ILogger"/>&gt;();</c>
		/// </summary>
		public ILogger? GetLogger()
			=> @this.GetService<ILogger>();

		/// <summary>
		/// <c>=&gt; @this.GetService&lt;<see cref="ILogger{TCategoryName}"/>&gt;();</c>
		/// </summary>
		public ILogger<T>? GetLogger<T>()
			=> @this.GetService<ILogger<T>>();

		/// <summary>
		/// <c>=&gt; @this.GetService&lt;<see cref="TimeProvider"/>&gt;() ?? <see cref="TimeProvider.System"/>;</c>
		/// </summary>
		public TimeProvider GetTimeProvider()
			=> @this.GetService<TimeProvider>() ?? TimeProvider.System;
	}
}
