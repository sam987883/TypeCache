// Copyright (c) 2021 Samuel Abraham

using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class SingletonTests
{
	private class TestClass
	{
		public static int InstanceCount { get; set; }

		public TestClass()
		{
			InstanceCount++;
		}
	}

	[Fact]
	public void Instance_ReturnsSingletonInstance()
	{
		var instance1 = Singleton<TestClass>.Instance;
		var instance2 = Singleton<TestClass>.Instance;

		Assert.Same(instance1, instance2);
	}

	[Fact]
	public void Instance_IsNotNull()
	{
		var instance = Singleton<TestClass>.Instance;

		Assert.NotNull(instance);
	}
}
