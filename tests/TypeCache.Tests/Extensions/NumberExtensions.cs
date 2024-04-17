// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class NumberExtensions
{
	[Fact]
	public void Abs()
	{
		Assert.Equal(long.Abs(-66L), (-66L).Abs());
	}

	[Fact]
	public void BitDecrement()
	{
		Assert.Equal(double.BitDecrement(-66.66), (-66.66).BitDecrement());
	}

	[Fact]
	public void BitIncrement()
	{
		Assert.Equal(double.BitIncrement(-66.66), (-66.66).BitIncrement());
	}

	[Fact]
	public void Ceiling()
	{
		Assert.Equal(double.Ceiling(-66.66), (-66.66).Ceiling());
	}

	[Fact]
	public void Factorial()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => (-1).Factorial());
		Assert.Equal(1UL, 0.Factorial());
		Assert.Equal(1UL, 1.Factorial());
		Assert.Equal(2UL, 2.Factorial());
		Assert.Equal(6UL, 3.Factorial());
		Assert.Equal(24UL, 4.Factorial());
		Assert.Equal(120UL, 5.Factorial());
	}

	[Fact]
	public void Floor()
	{
		Assert.Equal(double.Floor(-66.66), (-66.66).Floor());
	}

	[Fact]
	public void Max()
	{
		Assert.Equal(double.Max(-66.66, 99.99), (-66.66, 99.99).Max());
		Assert.Equal(Index.FromStart(99), (Index.FromStart(66), Index.FromStart(99)).Max());
		Assert.Equal(Index.FromEnd(66), (Index.FromEnd(66), Index.FromEnd(99)).Max());
	}

	[Fact]
	public void Min()
	{
		Assert.Equal(double.Min(-66.66, 99.99), (-66.66, 99.99).Min());
		Assert.Equal(Index.FromStart(66), (Index.FromStart(66), Index.FromStart(99)).Min());
		Assert.Equal(Index.FromEnd(99), (Index.FromEnd(66), Index.FromEnd(99)).Min());
	}

	[Fact]
	public void Repeat()
	{
		Assert.Equal(7, 1.Repeat(7).Count());
	}

	[Fact]
	public void Round()
	{
		Assert.Equal(double.Round(111.11), 111.11.Round());
		Assert.Equal(double.Round(111.1123456, 3), 111.1123456.Round(3));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.AwayFromZero), 111.1123456.Round(MidpointRounding.AwayFromZero));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToEven), 111.1123456.Round(MidpointRounding.ToEven));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToNegativeInfinity), 111.1123456.Round(MidpointRounding.ToNegativeInfinity));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToPositiveInfinity), 111.1123456.Round(MidpointRounding.ToPositiveInfinity));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToZero), 111.1123456.Round(MidpointRounding.ToZero));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.AwayFromZero), 111.1123456.Round(5, MidpointRounding.AwayFromZero));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToEven), 111.1123456.Round(5, MidpointRounding.ToEven));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToNegativeInfinity), 111.1123456.Round(5, MidpointRounding.ToNegativeInfinity));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToPositiveInfinity), 111.1123456.Round(5, MidpointRounding.ToPositiveInfinity));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToZero), 111.1123456.Round(5, MidpointRounding.ToZero));
	}

	[Fact]
	public void Sign()
	{
		Assert.Equal(int.Sign(7), 7.Sign());
		Assert.Equal(int.Sign(-7), (-7).Sign());
	}

	[Fact]
	public void ToBytes()
	{
		Assert.Equal(decimal.GetBits((decimal)Math.Tau).SelectMany(_ => _.GetBytes()), ((decimal)Math.Tau).ToBytes());
	}
}
