// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class RangeExtensions
{
	[Fact]
	public void Any()
	{
		Assert.True((20..3).Any());
		Assert.False((6..6).Any());
		Assert.False((0..0).Any());
		Assert.True((3..20).Any());
	}

	[Fact]
	public void Contains()
	{
		Assert.True((20..3).Contains(5));
		Assert.False((6..6).Contains(6));
		Assert.False((0..0).Contains(1));
		Assert.False((3..10).Contains(11));

		Assert.False((20..3).Contains(2..5));
		Assert.True((20..3).Contains(15..7));
		Assert.True((20..3).Contains(10..3));
		Assert.True((20..3).Contains(20..10));
		Assert.False((20..3).Contains(22..17));
		Assert.False((6..6).Contains(6..6));
		Assert.False((0..0).Contains(1..1));
		Assert.False((3..10).Contains(2..5));
		Assert.True((3..10).Contains(3..10));
		Assert.True((3..10).Contains(3..5));
		Assert.True((3..10).Contains(5..10));
		Assert.False((3..10).Contains(8..12));
	}

	[Fact]
	public void Count()
	{
		Assert.Equal(17, (20..3).Count());
		Assert.Equal(0, (6..6).Count());
		Assert.Equal(0, (0..0).Count());
		Assert.Equal(7, (3..10).Count());
	}

	[Fact]
	public void IsReverse()
	{
		Assert.True((20..3).IsReverse());
		Assert.False((6..6).IsReverse());
		Assert.False((0..0).IsReverse());
		Assert.False((3..20).IsReverse());
	}

	[Fact]
	public void Maximum()
	{
		Assert.Equal(20, (20..3).Maximum());
		Assert.Throws<ArgumentOutOfRangeException>(() => (7..7).Maximum());
		Assert.Throws<ArgumentOutOfRangeException>(() => (0..0).Maximum());
		Assert.Equal(9, (3..10).Maximum());
	}

	[Fact]
	public void Minimum()
	{
		Assert.Equal(4, (20..3).Minimum());
		Assert.Throws<ArgumentOutOfRangeException>(() => (7..7).Minimum());
		Assert.Throws<ArgumentOutOfRangeException>(() => (0..0).Minimum());
		Assert.Equal(3, (3..10).Minimum());
	}

	[Fact]
	public void Normalize()
	{
		Assert.Equal(5..15, new Range(Index.FromEnd(15), Index.FromEnd(5)).FromStart(20));
		Assert.Equal(5..15, new Range(Index.FromStart(5), Index.FromStart(15)).FromStart(20));
	}

	[Fact]
	public void ToEnumerable()
	{
		var expected1 = new[] { 0 };
		var expected2 = new[] { 0, 1 };
		var expected3 = new[] { 0, 1, 2 };

		Assert.Equal(expected1, (0..1).ToEnumerable().ToArray());
		Assert.Equal(expected2, (0..2).ToEnumerable().ToArray());
		Assert.Equal(expected3, (0..3).ToEnumerable().ToArray());

		Assert.Equal(expected1.AsEnumerable().Reverse(), (0..1).Reverse().ToEnumerable());
		Assert.Equal(expected2.AsEnumerable().Reverse(), (0..2).Reverse().ToEnumerable());
		Assert.Equal(expected3.AsEnumerable().Reverse(), (0..3).Reverse().ToEnumerable());
	}
}
