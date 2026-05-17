// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class ArrayTests
{
	[Fact]
	public void Empty()
	{
		var empty = Array<int>.Empty;
		Assert.Empty(empty);
	}

	[Fact]
	public void Empty_IEnumerable()
	{
		var empty = Array<string>.Empty;
		Assert.NotNull(empty);
		Assert.Empty(empty);
	}

	[Fact]
	public void Empty_CanEnumerate()
	{
		var empty = Array<double>.Empty;
		var count = 0;
		foreach (var _ in empty)
			count++;

		Assert.Equal(0, count);
	}
}
