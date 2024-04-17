// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ComparableExtensions
{
	[Fact]
	public void EqualTo()
	{
		Assert.False(((IComparable)2).EqualTo(1));
		Assert.True(((IComparable)2).EqualTo(2));
		Assert.False(((IComparable)2).EqualTo(3));
		Assert.False(2.EqualTo(1));
		Assert.True(2.EqualTo(2));
		Assert.False(2.EqualTo(3));
	}

	[Fact]
	public void GreaterThan()
	{
		Assert.True(((IComparable)2).GreaterThan(1));
		Assert.False(((IComparable)2).GreaterThan(2));
		Assert.False(((IComparable)2).GreaterThan(3));
		Assert.True(2.GreaterThan(1));
		Assert.False(2.GreaterThan(2));
		Assert.False(2.GreaterThan(3));
	}

	[Fact]
	public void GreaterThanOrEqualTo()
	{
		Assert.True(((IComparable)2).GreaterThanOrEqualTo(1));
		Assert.True(((IComparable)2).GreaterThanOrEqualTo(2));
		Assert.False(((IComparable)2).GreaterThanOrEqualTo(3));
		Assert.True(2.GreaterThanOrEqualTo(1));
		Assert.True(2.GreaterThanOrEqualTo(2));
		Assert.False(2.GreaterThanOrEqualTo(3));
	}

	[Fact]
	public void LessThan()
	{
		Assert.False(((IComparable)2).LessThan(1));
		Assert.False(((IComparable)2).LessThan(2));
		Assert.True(((IComparable)2).LessThan(3));
		Assert.False(2.LessThan(1));
		Assert.False(2.LessThan(2));
		Assert.True(2.LessThan(3));
	}

	[Fact]
	public void LessThanOrEqualTo()
	{
		Assert.False(((IComparable)2).LessThanOrEqualTo(1));
		Assert.True(((IComparable)2).LessThanOrEqualTo(2));
		Assert.True(((IComparable)2).LessThanOrEqualTo(3));
		Assert.False(2.LessThanOrEqualTo(1));
		Assert.True(2.LessThanOrEqualTo(2));
		Assert.True(2.LessThanOrEqualTo(3));
	}
}
