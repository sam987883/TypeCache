// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions;

public class ComparableExtensions
{
	[Fact]
	public void Between()
	{
		Assert.False(((IComparable)(-1)).Between(1, 3));
		Assert.True(((IComparable)1).Between(1, 3));
		Assert.True(((IComparable)2).Between(1, 3));
		Assert.True(((IComparable)3).Between(1, 3));
		Assert.False(((IComparable)4).Between(1, 3));
		Assert.False(((IComparable<int>)(-1)).Between(1, 3));
		Assert.True(((IComparable<int>)1).Between(1, 3));
		Assert.True(((IComparable<int>)2).Between(1, 3));
		Assert.True(((IComparable<int>)3).Between(1, 3));
		Assert.False(((IComparable<int>)4).Between(1, 3));
	}

	[Fact]
	public void EqualTo()
	{
		Assert.False(((IComparable)2).EqualTo(1));
		Assert.True(((IComparable)2).EqualTo(2));
		Assert.False(((IComparable)2).EqualTo(3));
		Assert.False(((IComparable<int>)2).EqualTo(1));
		Assert.True(((IComparable<int>)2).EqualTo(2));
		Assert.False(((IComparable<int>)2).EqualTo(3));
	}

	[Fact]
	public void InBetween()
	{
		Assert.False(((IComparable)1).InBetween(1, 3));
		Assert.True(((IComparable)2).InBetween(1, 3));
		Assert.False(((IComparable)3).InBetween(1, 3));
		Assert.False(((IComparable<int>)1).InBetween(1, 3));
		Assert.True(((IComparable<int>)2).InBetween(1, 3));
		Assert.False(((IComparable<int>)3).InBetween(1, 3));
	}

	[Fact]
	public void LessThan()
	{
		Assert.False(((IComparable)2).LessThan(1));
		Assert.False(((IComparable)2).LessThan(2));
		Assert.True(((IComparable)2).LessThan(3));
		Assert.False(((IComparable<int>)2).LessThan(1));
		Assert.False(((IComparable<int>)2).LessThan(2));
		Assert.True(((IComparable<int>)2).LessThan(3));
	}

	[Fact]
	public void LessThanOrEqualTo()
	{
		Assert.False(((IComparable)2).LessThanOrEqualTo(1));
		Assert.True(((IComparable)2).LessThanOrEqualTo(2));
		Assert.True(((IComparable)2).LessThanOrEqualTo(3));
		Assert.False(((IComparable<int>)2).LessThanOrEqualTo(1));
		Assert.True(((IComparable<int>)2).LessThanOrEqualTo(2));
		Assert.True(((IComparable<int>)2).LessThanOrEqualTo(3));
	}

	[Fact]
	public void MoreThan()
	{
		Assert.True(((IComparable)2).GreaterThan(1));
		Assert.False(((IComparable)2).GreaterThan(2));
		Assert.False(((IComparable)2).GreaterThan(3));
		Assert.True(((IComparable<int>)2).GreaterThan(1));
		Assert.False(((IComparable<int>)2).GreaterThan(2));
		Assert.False(((IComparable<int>)2).GreaterThan(3));
	}

	[Fact]
	public void MoreThanOrEqualTo()
	{
		Assert.True(((IComparable)2).GreaterThanOrEqualTo(1));
		Assert.True(((IComparable)2).GreaterThanOrEqualTo(2));
		Assert.False(((IComparable)2).GreaterThanOrEqualTo(3));
		Assert.True(((IComparable<int>)2).GreaterThanOrEqualTo(1));
		Assert.True(((IComparable<int>)2).GreaterThanOrEqualTo(2));
		Assert.False(((IComparable<int>)2).GreaterThanOrEqualTo(3));
	}
}
