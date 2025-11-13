// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class GlobalExtensions
{
	[Fact]
	public void Between()
	{
		Assert.False((-1).Between(1, 3));
		Assert.True(1.Between(1, 3));
		Assert.True(2U.Between(1U, 3U));
		Assert.True(3L.Between(1L, 3L));
		Assert.False(4UL.Between(1UL, 3UL));
	}

	[Fact]
	public void Box()
	{
		var expected = 333333;
		var box = expected.Box();
		Assert.Equal(expected, (int)box);
	}

	[Fact]
	public void InBetween()
	{
		Assert.False((-1).InBetween(1, 3));
		Assert.False(1.InBetween(1, 3));
		Assert.True(2U.InBetween(1U, 3U));
		Assert.False(3L.InBetween(1L, 3L));
		Assert.False(4UL.InBetween(1UL, 3UL));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(10)]
	[InlineData(19)]
	public void Repeat(int expected)
	{
		Assert.Equal(expected, true.Repeat(expected).Count());
	}

	[Fact]
	public void ThrowIfEqual()
	{
		123456.ThrowIfEqual(12345);
		Assert.Throws<ArgumentOutOfRangeException>(() => 123.ThrowIfEqual(123));
	}

	[Fact]
	public void ThrowIfNotEqual()
	{
		123456.ThrowIfNotEqual(123456);
		Assert.Throws<ArgumentOutOfRangeException>(() => 123.ThrowIfNotEqual(456));
	}
}
