// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ServiceProviderExtensionsTests
{
	[Fact]
	public void GetLogger_Generic()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		var provider = services.BuildServiceProvider();

		var logger = provider.GetLogger<ServiceProviderExtensionsTests>();

		Assert.NotNull(logger);
	}

	[Fact]
	public void Scope_ExecutesWithScopedProvider()
	{
		var services = new ServiceCollection();
		services.AddScoped<ServiceProviderExtensionsTests>();
		var provider = services.BuildServiceProvider();

		var executed = false;
		provider.Scope(scopedProvider =>
		{
			var service = scopedProvider.GetService<ServiceProviderExtensionsTests>();
			executed = service != null;
		});

		Assert.True(executed);
	}
}
