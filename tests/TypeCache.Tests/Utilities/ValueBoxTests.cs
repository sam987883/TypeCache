// Copyright (c) 2021 Samuel Abraham

using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class ValueBoxTests
{
	[Fact]
	public void GetValue_IntValue()
	{
		var value = 42;
		var boxed = ValueBox<int>.GetValue(value);

		Assert.NotNull(boxed);
		Assert.IsType<int>(boxed);
		Assert.Equal(42, (int)boxed);
	}

	[Fact]
	public void GetValue_SameValueReturnsSameObject()
	{
		var boxed1 = ValueBox<int>.GetValue(42);
		var boxed2 = ValueBox<int>.GetValue(42);

		Assert.Same(boxed1, boxed2);
	}

	[Fact]
	public void GetValue_DifferentValuesReturnDifferentObjects()
	{
		var boxed1 = ValueBox<int>.GetValue(42);
		var boxed2 = ValueBox<int>.GetValue(43);

		Assert.NotSame(boxed1, boxed2);
	}

	[Fact]
	public void GetValue_BoolValue()
	{
		var boxed = ValueBox<bool>.GetValue(true);

		Assert.NotNull(boxed);
		Assert.True((bool)boxed);
	}

	[Fact]
	public void GetValue_CharValue()
	{
		var boxed = ValueBox<char>.GetValue('A');

		Assert.NotNull(boxed);
		Assert.Equal('A', (char)boxed);
	}

	[Fact]
	public void GetValue_DoubleValue()
	{
		var boxed = ValueBox<double>.GetValue(3.14);

		Assert.NotNull(boxed);
		Assert.Equal(3.14, (double)boxed);
	}
}
