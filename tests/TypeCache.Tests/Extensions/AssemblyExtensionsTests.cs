// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class AssemblyExtensions
{
	[Fact]
	public void InformationalVersion()
	{
		var assembly = typeof(object).Assembly;
		var version = assembly.InformationalVersion;

		Assert.NotNull(version);
	}

	[Fact]
	public void NuGetVersion()
	{
		var assembly = typeof(object).Assembly;
		var version = assembly.NuGetVersion;

		Assert.NotNull(version);
	}
}
