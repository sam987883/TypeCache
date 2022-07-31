// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
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
	public void Count()
	{
		Assert.Equal(17, (20..3).Length());
		Assert.Equal(0, (6..6).Length());
		Assert.Equal(0, (0..0).Length());
		Assert.Equal(7, (3..10).Length());
	}

	[Fact]
	public void Has()
	{
		Assert.True((20..3).Has(5));
		Assert.False((6..6).Has(6));
		Assert.False((0..0).Has(1));
		Assert.False((3..10).Has(11));

		Assert.False((20..3).Has(2..5));
		Assert.True((20..3).Has(15..7));
		Assert.True((20..3).Has(10..3));
		Assert.True((20..3).Has(20..10));
		Assert.False((20..3).Has(22..17));
		Assert.False((6..6).Has(6..6));
		Assert.False((0..0).Has(1..1));
		Assert.False((3..10).Has(2..5));
		Assert.True((3..10).Has(3..10));
		Assert.True((3..10).Has(3..5));
		Assert.True((3..10).Has(5..10));
		Assert.False((3..10).Has(8..12));
	}

	[Fact]
	public void IntersectWith()
	{
		Assert.Equal(20..3, (20..3).IntersectWith(20..3));
		Assert.Equal(10..3, (20..3).IntersectWith(10..3));
		Assert.Equal(10..3, (20..3).IntersectWith(10..1));
		Assert.Equal(10..5, (20..5).IntersectWith(10..3));
		Assert.Equal(20..10, (20..3).IntersectWith(20..10));
		Assert.Equal(20..10, (20..3).IntersectWith(25..10));
		Assert.Equal(10..5, (25..5).IntersectWith(10..3));

		Assert.Equal(3..20, (3..20).IntersectWith(3..20));
		Assert.Equal(3..10, (3..20).IntersectWith(3..10));
		Assert.Equal(3..10, (3..20).IntersectWith(1..10));
		Assert.Equal(5..10, (5..20).IntersectWith(3..10));
		Assert.Equal(10..20, (3..20).IntersectWith(10..20));
		Assert.Equal(10..20, (3..20).IntersectWith(10..25));
		Assert.Equal(5..10, (5..25).IntersectWith(3..10));

		Assert.Null((25..15).IntersectWith(15..3));
		Assert.Null((15..25).IntersectWith(3..15));
		Assert.Null((15..15).IntersectWith(15..15));
		Assert.Null((0..0).IntersectWith(1..1));
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
		Assert.Equal(null, (7..7).Maximum());
		Assert.Equal(null, (0..0).Maximum());
		Assert.Equal(9, (3..10).Maximum());
	}

	[Fact]
	public void Minimum()
	{
		Assert.Equal(4, (20..3).Minimum());
		Assert.Equal(null, (7..7).Minimum());
		Assert.Equal(null, (0..0).Minimum());
		Assert.Equal(3, (3..10).Minimum());
	}

	[Fact]
	public void Normalize()
	{
		Assert.Equal(5..15, new Range(Index.FromEnd(15), Index.FromEnd(5)).Normalize(20));
		Assert.Equal(5..15, new Range(Index.FromStart(5), Index.FromStart(15)).Normalize(20));
	}

	[Fact]
	public void UnionWith()
	{
		Assert.Equal(20..3, (20..3).UnionWith(20..3));
		Assert.Equal(20..3, (20..3).UnionWith(10..3));
		Assert.Equal(20..1, (20..3).UnionWith(10..1));
		Assert.Equal(20..3, (20..5).UnionWith(10..3));
		Assert.Equal(20..3, (20..3).UnionWith(20..10));
		Assert.Equal(25..3, (20..3).UnionWith(25..10));
		Assert.Equal(25..3, (25..5).UnionWith(10..3));

		Assert.Equal(3..20, (3..20).UnionWith(3..20));
		Assert.Equal(3..20, (3..20).UnionWith(3..10));
		Assert.Equal(1..20, (3..20).UnionWith(1..10));
		Assert.Equal(3..20, (5..20).UnionWith(3..10));
		Assert.Equal(3..20, (3..20).UnionWith(10..20));
		Assert.Equal(3..25, (3..20).UnionWith(10..25));
		Assert.Equal(3..25, (5..25).UnionWith(3..10));

		Assert.Equal(25..3, (25..15).UnionWith(15..3));
		Assert.Equal(3..25, (15..25).UnionWith(3..15));
		Assert.Null((15..15).UnionWith(15..15));
		Assert.Null((0..0).UnionWith(1..1));
	}

	[Fact]
	public void Values()
	{
		Assert.Equal(new[] { 5, 6, 7, 8 }, (5..8).Values().ToArray());
		Assert.Equal(new[] { 7, 6, 5, 4 }, (7..4).Values().ToArray());
		Assert.Equal(new[] { 0, 1, 2, 3, 4 }, (0..4).Values().ToArray());
		Assert.Equal(new[] { 4, 3, 2, 1, 0 }, (4..0).Values().ToArray());
	}
}
