// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class TupleExtensionsTests
{
	[Fact]
	public void ToArray()
	{
		var tuple = (1, "two", 3.0);

		var array = tuple.ToArray();

		Assert.Equal(3, array.Length);
		Assert.Equal(1, array[0]);
		Assert.Equal("two", array[1]);
		Assert.Equal(3.0, array[2]);
	}

	[Fact]
	public void ToArray_SingleElement()
	{
		var tuple = Tuple.Create(42);

		var array = tuple.ToArray();

		Assert.Single(array);
		Assert.Equal(42, array[0]);
	}

	[Fact]
	public void ToArray_WithNull()
	{
		var tuple = (1, null as string, 3);

		var array = tuple.ToArray();

		Assert.Equal(3, array.Length);
		Assert.Null(array[1]);
	}
}
