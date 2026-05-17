// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TypeCache.Extensions;

public static class ServiceProviderExtensions
{
	extension(IServiceProvider @this)
	{
		/// <summary>
		/// <c>=&gt; @this.GetService&lt;<see cref="ILogger{TCategoryName}"/>&gt;();</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ILogger<T>? GetLogger<T>()
			=> @this.GetService<ILogger<T>>();

		/// <summary>
		/// Runs an <c><see cref="Action"/></c> passing in a scoped service provider.
		/// </summary>
		public void Scope(Action<IServiceProvider> scoped)
		{
			using var scope = @this.CreateScope();
			scoped(scope.ServiceProvider);
		}

		/// <summary>
		/// Runs a function passing in a scoped service provider.
		/// </summary>
		public T Scope<T>(Func<IServiceProvider, T> scoped)
		{
			using var scope = @this.CreateScope();
			return scoped(scope.ServiceProvider);
		}

		/// <summary>
		/// <c>=&gt; @this.GetService&lt;<see cref="TimeProvider"/>&gt;() ?? <see cref="TimeProvider.System"/>;</c>
		/// </summary>
		public TimeProvider TimeProvider
			=> @this.GetService<TimeProvider>() ?? TimeProvider.System;
	}
}
