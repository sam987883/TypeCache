// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class SetExtensionsTests
{
	[Fact]
	public void ForEach()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var result = new List<int>();

		set.ForEach(item => result.Add(item));

		Assert.Equal(3, result.Count);
		Assert.Contains(1, result);
		Assert.Contains(2, result);
		Assert.Contains(3, result);
	}

	[Fact]
	public void ForEach_Empty()
	{
		var set = new HashSet<int>();
		var called = false;

		set.ForEach(_ => called = true);

		Assert.False(called);
	}
}
